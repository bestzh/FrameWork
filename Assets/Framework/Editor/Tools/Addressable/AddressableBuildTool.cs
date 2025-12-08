using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace Tools.Addressable
{
    /// <summary>
    /// Addressable资源构建工具
    /// </summary>
    public class AddressableBuildTool : EditorWindow
    {
        private string buildOutputPath = "ServerData";
        private BuildTarget currentBuildTarget = BuildTarget.StandaloneWindows64;
        private bool clearBuildCache = false;
        private bool copyToStreamingAssets = true;  // 默认自动复制到StreamingAssets
        
        /// <summary>
        /// 检查Addressables包是否已安装
        /// </summary>
        private static bool IsAddressablesInstalled()
        {
            // 方法1：检查类型是否存在
            System.Type settingsType = System.Type.GetType("UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject, Unity.Addressables.Editor");
            if (settingsType != null)
                return true;
            
            // 方法2：检查程序集是否加载
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.FullName.Contains("Unity.Addressables"))
                {
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// 获取AddressableAssetSettings对象
        /// </summary>
        private static object GetAddressableSettings()
        {
            if (!IsAddressablesInstalled())
                return null;
            
            // 尝试使用直接API
            try
            {
                var settings = AddressableAssetSettingsDefaultObject.Settings;
                if (settings != null)
                    return settings;
            }
            catch { }
            
            // 回退到反射方法
            System.Type settingsType = System.Type.GetType("UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject, Unity.Addressables.Editor");
            if (settingsType != null)
            {
                var property = settingsType.GetProperty("Settings");
                return property?.GetValue(null);
            }
            
            return null;
        }
        
        [MenuItem("Tools/Addressable/资源构建工具")]
        public static void ShowWindow()
        {
            AddressableBuildTool window = GetWindow<AddressableBuildTool>("Addressable构建工具");
            window.minSize = new Vector2(500, 400);
            
            // 确保StreamingAssets目录存在
            window.EnsureStreamingAssetsExists();
        }
        
        private void OnGUI()
        {
            try
            {
                EditorGUILayout.LabelField("Addressable资源构建工具", EditorStyles.boldLabel);
                EditorGUILayout.Space();
            
            // 检查Addressables是否安装
            bool addressablesInstalled = IsAddressablesInstalled();
            
            if (!addressablesInstalled)
            {
                EditorGUILayout.HelpBox(
                    "Addressables包未安装！\n" +
                    "请先安装Addressables包：Window > Package Manager > 搜索 'Addressables' > Install",
                    MessageType.Error);
                return;
            }
            
            // 使用反射访问Addressables API
            var settings = GetAddressableSettings();
            if (settings == null)
            {
                EditorGUILayout.HelpBox(
                    "Addressables未初始化！\n" +
                    "请先初始化Addressables：Tools > Addressable > 统一批量标注工具",
                    MessageType.Warning);
                
                if (GUILayout.Button("打开标注工具", GUILayout.Height(30)))
                {
                    UnifiedAddressableTool.ShowWindow();
                }
                return;
            }
            
            // 获取groups和entries
            int groupCount = 0;
            int entryCount = 0;
            
            try
            {
                // 尝试使用直接API
                if (settings is AddressableAssetSettings addressableSettings)
                {
                    foreach (var group in addressableSettings.groups)
                    {
                        if (group == null)
                            continue;
                        
                        groupCount++;
                        try
                        {
                            // 安全地访问entries
                            if (group.entries != null)
                            {
                                entryCount += group.entries.Count;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogWarning($"无法访问Group '{group.name}' 的entries: {ex.Message}");
                        }
                    }
                }
                else
                {
                    // 回退到反射方法
                    var groupsProperty = settings.GetType().GetProperty("groups");
                    var groups = groupsProperty?.GetValue(settings) as System.Collections.IEnumerable;
                    
                    if (groups != null)
                    {
                        foreach (var group in groups)
                        {
                            if (group == null)
                                continue;
                            
                            groupCount++;
                            try
                            {
                                var entriesProperty = group.GetType().GetProperty("entries");
                                var entries = entriesProperty?.GetValue(group) as System.Collections.IEnumerable;
                                if (entries != null)
                                {
                                    foreach (var entry in entries)
                                    {
                                        if (entry != null)
                                            entryCount++;
                                    }
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogWarning($"无法访问Group的entries: {ex.Message}");
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"获取groups和entries失败: {ex.Message}");
            }
            
            // 显示当前配置信息
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("当前配置", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("资源组数量: " + groupCount.ToString());
            EditorGUILayout.LabelField("资源条目数量: " + entryCount.ToString());
            
            // 显示目录状态
            EditorGUILayout.Space();
            string streamingAssetsPath = Application.streamingAssetsPath;
            bool streamingAssetsExists = Directory.Exists(streamingAssetsPath);
            string aaPath = Path.Combine(streamingAssetsPath,currentBuildTarget.ToString());
            bool aaPathExists = Directory.Exists(aaPath);
            
            EditorGUILayout.LabelField("目录状态:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"StreamingAssets: {(streamingAssetsExists ? "✓ 存在" : "✗ 不存在")}");
            EditorGUILayout.LabelField($"{currentBuildTarget}: {(aaPathExists ? "✓ 存在" : "✗ 不存在")}");
            
            if (!streamingAssetsExists)
            {
                if (GUILayout.Button("创建StreamingAssets目录", GUILayout.Height(25)))
                {
                    EnsureStreamingAssetsExists();
                    EditorUtility.DisplayDialog("完成", "StreamingAssets目录已创建", "确定");
                }
            }
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space();
            
            // 构建配置
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("构建配置", EditorStyles.boldLabel);
            
            currentBuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("构建平台:", currentBuildTarget);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("输出路径:", GUILayout.Width(80));
            buildOutputPath = EditorGUILayout.TextField(buildOutputPath);
            if (GUILayout.Button("浏览", GUILayout.Width(60)))
            {
                string selectedPath = EditorUtility.SaveFolderPanel("选择输出目录", buildOutputPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    buildOutputPath = selectedPath;
                }
            }
            EditorGUILayout.EndHorizontal();
            
            clearBuildCache = EditorGUILayout.Toggle("清理构建缓存", clearBuildCache);
            copyToStreamingAssets = EditorGUILayout.Toggle("自动复制到StreamingAssets", copyToStreamingAssets);
            
            if (copyToStreamingAssets)
            {
                EditorGUILayout.HelpBox(
                    "构建完成后，资源将自动复制到 StreamingAssets/AA/[BuildTarget] 目录",
                    MessageType.Info);
            }
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space();
            
            // 构建按钮
            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = entryCount > 0;
            if (GUILayout.Button("构建Addressables资源", GUILayout.Height(40)))
            {
                try
                {
                    BuildAddressables();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"构建过程中发生错误: {ex.Message}\n{ex.StackTrace}");
                    EditorUtility.DisplayDialog("构建错误", $"构建失败: {ex.Message}", "确定");
                }
            }
            
            if (GUILayout.Button("使用Unity菜单构建", GUILayout.Height(40)))
            {
                // 打开Groups窗口并提示用户
                EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");
                EditorUtility.DisplayDialog("提示", 
                    "已打开Addressables Groups窗口\n\n" +
                    "请执行以下步骤：\n" +
                    "1. 点击 Build 菜单\n" +
                    "2. 选择 New Build > Default Build Script\n" +
                    "3. 等待构建完成\n\n" +
                    "构建完成后，如果启用了自动复制，工具会自动将资源复制到StreamingAssets",
                    "确定");
            }
            
            if (GUILayout.Button("清理构建缓存", GUILayout.Height(40)))
            {
                CleanBuildCache();
            }
            
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // 说明信息
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("说明", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "1. 构建完成后，资源文件会输出到配置的路径\n" +
                "2. 默认输出路径：ServerData/[BuildTarget]\n" +
                "3. 如果启用自动复制，资源会自动复制到StreamingAssets/AA/[BuildTarget]\n" +
                "4. 如果自动构建失败，请使用\"使用Unity菜单构建\"按钮",
                MessageType.Info);
            EditorGUILayout.EndVertical();
            
            // 故障排除
            if (entryCount == 0)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("故障排除", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "没有找到资源条目！\n\n" +
                    "请确保：\n" +
                    "1. 已使用标注工具标记资源\n" +
                    "2. 资源已正确添加到Addressable组中\n" +
                    "3. 点击\"打开标注工具\"检查资源状态",
                    MessageType.Warning);
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.Space();
            
            // 快速操作
            EditorGUILayout.LabelField("快速操作", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("打开Groups窗口", GUILayout.Height(30)))
            {
                EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");
            }
            
            if (GUILayout.Button("打开标注工具", GUILayout.Height(30)))
            {
                UnifiedAddressableTool.ShowWindow();
            }
            
            EditorGUILayout.EndHorizontal();
            }
            catch (System.Exception ex)
            {
                // 确保GUI状态恢复
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                
                Debug.LogError($"OnGUI错误: {ex.Message}\n{ex.StackTrace}");
                EditorGUILayout.HelpBox($"GUI错误: {ex.Message}\n\n请关闭并重新打开窗口。", MessageType.Error);
            }
        }
        
        /// <summary>
        /// 检查并重新导入Lua文件
        /// </summary>
        private void CheckAndReimportLuaFiles()
        {
            try
            {
                // 使用Application.dataPath来获取完整路径
                string luaFolder = Path.Combine(Application.dataPath, "Resources", "lua");
                if (!Directory.Exists(luaFolder))
                {
                    Debug.Log($"Lua文件夹不存在: {luaFolder}");
                    return;
                }
                
                // 查找所有.lua文件（排除.lua.txt）
                string[] luaFiles = Directory.GetFiles(luaFolder, "*.lua", SearchOption.AllDirectories);
                System.Collections.Generic.List<string> filesToCheck = new System.Collections.Generic.List<string>();
                
                foreach (string file in luaFiles)
                {
                    if (!file.EndsWith(".lua.txt"))
                    {
                        filesToCheck.Add(file);
                    }
                }
                
                int unrecognizedCount = 0;
                int recognizedCount = 0;
                
                foreach (string file in filesToCheck)
                {
                    string relativePath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
                    TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(relativePath);
                    
                    if (asset == null)
                    {
                        unrecognizedCount++;
                        Debug.LogWarning($"Lua文件未识别为TextAsset: {relativePath}");
                        Debug.LogWarning($"建议：使用 Tools > XLua > 批量重命名Lua文件为.lua.txt 来修复");
                    }
                    else
                    {
                        recognizedCount++;
                    }
                }
                
                // 检查.lua.txt文件
                string[] luaTxtFiles = Directory.GetFiles(luaFolder, "*.lua.txt", SearchOption.AllDirectories);
                int luaTxtCount = 0;
                int luaTxtUnrecognizedCount = 0;
                
                Debug.Log($"找到 {luaTxtFiles.Length} 个.lua.txt文件");
                
                foreach (string file in luaTxtFiles)
                {
                    string relativePath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
                    TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(relativePath);
                    if (asset != null)
                    {
                        luaTxtCount++;
                    }
                    else
                    {
                        luaTxtUnrecognizedCount++;
                        Debug.LogWarning($"  .lua.txt文件未识别为TextAsset: {relativePath}");
                    }
                }
                
                Debug.Log($"Lua文件检查完成:");
                Debug.Log($"  .lua文件: {recognizedCount} 个已识别, {unrecognizedCount} 个未识别");
                Debug.Log($"  .lua.txt文件: {luaTxtCount} 个已识别, {luaTxtUnrecognizedCount} 个未识别");
                Debug.Log($"  总计: {recognizedCount + luaTxtCount} 个已识别的Lua文件");
                
                if (unrecognizedCount > 0)
                {
                    Debug.LogWarning($"发现 {unrecognizedCount} 个未识别的.lua文件，建议重命名为.lua.txt格式");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"检查Lua文件失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 使用直接API修复Group的Schema
        /// </summary>
        private bool FixGroupSchemasDirect(AddressableAssetSettings settings)
        {
            try
            {
                bool hasFixed = false;
                
                foreach (var group in settings.groups)
                {
                    if (group == null || group.name == "Default Local Group")
                        continue;
                    
                    bool hasBundledSchema = false;
                    bool hasContentUpdateSchema = false;
                    
                    // 检查现有Schema
                    foreach (var schema in group.Schemas)
                    {
                        if (schema is BundledAssetGroupSchema)
                            hasBundledSchema = true;
                        if (schema is ContentUpdateGroupSchema)
                            hasContentUpdateSchema = true;
                    }
                    
                    if (!hasBundledSchema || !hasContentUpdateSchema)
                    {
                        Debug.Log($"修复Group Schema: {group.name} (Bundled: {hasBundledSchema}, ContentUpdate: {hasContentUpdateSchema})");
                        
                        if (!hasBundledSchema)
                        {
                            try
                            {
                                var bundledSchema = group.AddSchema<BundledAssetGroupSchema>();
                                if (bundledSchema != null)
                                {
                                    hasFixed = true;
                                    Debug.Log($"  ✓ 已添加 BundledAssetGroupSchema");
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogError($"  添加 BundledAssetGroupSchema 失败: {ex.Message}");
                            }
                        }
                        
                        if (!hasContentUpdateSchema)
                        {
                            try
                            {
                                var contentUpdateSchema = group.AddSchema<ContentUpdateGroupSchema>();
                                if (contentUpdateSchema != null)
                                {
                                    hasFixed = true;
                                    Debug.Log($"  ✓ 已添加 ContentUpdateGroupSchema");
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogError($"  添加 ContentUpdateGroupSchema 失败: {ex.Message}");
                            }
                        }
                    }
                }
                
                if (hasFixed)
                {
                    EditorUtility.SetDirty(settings);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Debug.Log("✓ Group Schema修复完成，已保存设置");
                }
                else
                {
                    Debug.Log("所有Group的Schema都已正确配置");
                }
                
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"修复Group Schema失败: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
        
        /// <summary>
        /// 检查并修复Group的Schema
        /// </summary>
        private bool FixGroupSchemas(object settings)
        {
            try
            {
                // 尝试使用直接API
                Debug.Log($"FixGroupSchemas: settings类型 = {settings?.GetType().FullName}");
                if (settings is AddressableAssetSettings addressableSettings)
                {
                    Debug.Log("使用直接API修复Schema");
                    return FixGroupSchemasDirect(addressableSettings);
                }
                else
                {
                    Debug.LogWarning($"settings不是AddressableAssetSettings类型，使用反射方法");
                }
                
                // 回退到反射方法
                var groupsProperty = settings.GetType().GetProperty("groups");
                var groups = groupsProperty?.GetValue(settings) as System.Collections.IEnumerable;
                
                if (groups == null)
                    return true;
                
                // 获取Schema类型
                System.Type bundledSchemaType = System.Type.GetType("UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema, Unity.Addressables.Editor");
                System.Type contentUpdateSchemaType = System.Type.GetType("UnityEditor.AddressableAssets.Settings.GroupSchemas.ContentUpdateGroupSchema, Unity.Addressables.Editor");
                
                if (bundledSchemaType == null || contentUpdateSchemaType == null)
                {
                    Debug.LogWarning("无法找到Schema类型，跳过Schema修复");
                    Debug.LogWarning($"BundledSchemaType: {bundledSchemaType}, ContentUpdateSchemaType: {contentUpdateSchemaType}");
                    return true;
                }
                
                bool hasFixed = false;
                foreach (var group in groups)
                {
                    var groupNameProperty = group.GetType().GetProperty("Name");
                    string groupName = groupNameProperty?.GetValue(group)?.ToString() ?? "Unknown";
                    
                    // 跳过Default Local Group（它已经有Schema了）
                    if (groupName == "Default Local Group")
                        continue;
                    
                    var schemasProperty = group.GetType().GetProperty("Schemas");
                    var schemas = schemasProperty?.GetValue(group) as System.Collections.IList;
                    
                    if (schemas == null)
                    {
                        Debug.LogWarning($"Group {groupName} 的Schemas属性为null");
                        continue;
                    }
                    
                    // 检查是否有BundledAssetGroupSchema
                    bool hasBundledSchema = false;
                    bool hasContentUpdateSchema = false;
                    
                    foreach (var schema in schemas)
                    {
                        if (schema != null)
                        {
                            var schemaType = schema.GetType();
                            if (schemaType == bundledSchemaType || schemaType.IsSubclassOf(bundledSchemaType))
                                hasBundledSchema = true;
                            if (schemaType == contentUpdateSchemaType || schemaType.IsSubclassOf(contentUpdateSchemaType))
                                hasContentUpdateSchema = true;
                        }
                    }
                    
                    // 如果没有Schema，添加默认Schema
                    if (!hasBundledSchema || !hasContentUpdateSchema)
                    {
                        Debug.Log($"修复Group Schema: {groupName} (Bundled: {hasBundledSchema}, ContentUpdate: {hasContentUpdateSchema})");
                        
                        if (!hasBundledSchema)
                        {
                            try
                            {
                                // 方法1: 尝试查找所有AddSchema方法
                                var allMethods = group.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
                                MethodInfo addSchemaMethod = null;
                                
                                foreach (var method in allMethods)
                                {
                                    if (method.Name == "AddSchema")
                                    {
                                        var parameters = method.GetParameters();
                                        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(System.Type))
                                        {
                                            addSchemaMethod = method;
                                            break;
                                        }
                                        else if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(bundledSchemaType))
                                        {
                                            addSchemaMethod = method;
                                            break;
                                        }
                                    }
                                }
                                
                                if (addSchemaMethod != null)
                                {
                                    var result = addSchemaMethod.Invoke(group, new[] { bundledSchemaType });
                                    if (result != null)
                                    {
                                        hasFixed = true;
                                        Debug.Log($"  ✓ 已添加 BundledAssetGroupSchema");
                                    }
                                    else
                                    {
                                        Debug.LogWarning($"  ✗ AddSchema返回null，但可能已成功，尝试其他方法验证...");
                                        
                                        // 验证Schema是否真的添加了
                                        var verifySchemas = schemasProperty?.GetValue(group) as System.Collections.IList;
                                        bool verified = false;
                                        if (verifySchemas != null)
                                        {
                                            foreach (var schema in verifySchemas)
                                            {
                                                if (schema != null)
                                                {
                                                    var schemaType = schema.GetType();
                                                    if (schemaType == bundledSchemaType || schemaType.IsSubclassOf(bundledSchemaType))
                                                    {
                                                        verified = true;
                                                        hasFixed = true;
                                                        Debug.Log($"  ✓ 验证成功：BundledAssetGroupSchema已添加");
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        
                                        if (!verified)
                                        {
                                            // 方法2: 尝试使用CreateDefaultSchema方法
                                            var createSchemaMethod = group.GetType().GetMethod("CreateDefaultSchema", new[] { typeof(System.Type) });
                                            if (createSchemaMethod != null)
                                            {
                                                result = createSchemaMethod.Invoke(group, new[] { bundledSchemaType });
                                                if (result != null)
                                                {
                                                    hasFixed = true;
                                                    Debug.Log($"  ✓ 使用CreateDefaultSchema添加 BundledAssetGroupSchema");
                                                }
                                            }
                                            
                                            // 方法3: 尝试从Default Local Group复制Schema
                                            if (!hasFixed)
                                            {
                                                var defaultGroup = groupsProperty?.GetValue(settings) as System.Collections.IEnumerable;
                                                if (defaultGroup != null)
                                                {
                                                    foreach (var defaultGrp in defaultGroup)
                                                    {
                                                        var defaultGroupNameProp = defaultGrp.GetType().GetProperty("Name");
                                                        if (defaultGroupNameProp?.GetValue(defaultGrp)?.ToString() == "Default Local Group")
                                                        {
                                                            var defaultSchemasProp = defaultGrp.GetType().GetProperty("Schemas");
                                                            var defaultSchemas = defaultSchemasProp?.GetValue(defaultGrp) as System.Collections.IList;
                                                            if (defaultSchemas != null)
                                                            {
                                                                foreach (var defaultSchema in defaultSchemas)
                                                                {
                                                                    if (defaultSchema != null)
                                                                    {
                                                                        var defaultSchemaType = defaultSchema.GetType();
                                                                        if (defaultSchemaType == bundledSchemaType || defaultSchemaType.IsSubclassOf(bundledSchemaType))
                                                                        {
                                                                            // 尝试使用AddSchema(object)方法
                                                                            foreach (var method in allMethods)
                                                                            {
                                                                                if (method.Name == "AddSchema" && method.GetParameters().Length == 1)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        var copyResult = method.Invoke(group, new[] { defaultSchema });
                                                                                        if (copyResult != null)
                                                                                        {
                                                                                            hasFixed = true;
                                                                                            Debug.Log($"  ✓ 从Default Local Group复制 BundledAssetGroupSchema");
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                    catch { }
                                                                                }
                                                                            }
                                                                            if (hasFixed) break;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning($"  无法找到AddSchema方法，尝试直接使用AddressableAssetGroup API");
                                    // 如果反射失败，提示用户手动添加
                                    Debug.LogWarning($"  请手动为Group '{groupName}' 添加Schema：在Addressables Groups窗口中右键点击Group > Add Schema");
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogError($"  添加 BundledAssetGroupSchema 失败: {ex.Message}\n{ex.StackTrace}");
                            }
                        }
                        
                        if (!hasContentUpdateSchema)
                        {
                            try
                            {
                                // 方法1: 尝试查找所有AddSchema方法
                                var allMethods = group.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
                                MethodInfo addSchemaMethod = null;
                                
                                foreach (var method in allMethods)
                                {
                                    if (method.Name == "AddSchema")
                                    {
                                        var parameters = method.GetParameters();
                                        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(System.Type))
                                        {
                                            addSchemaMethod = method;
                                            break;
                                        }
                                        else if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(contentUpdateSchemaType))
                                        {
                                            addSchemaMethod = method;
                                            break;
                                        }
                                    }
                                }
                                
                                if (addSchemaMethod != null)
                                {
                                    var result = addSchemaMethod.Invoke(group, new[] { contentUpdateSchemaType });
                                    if (result != null)
                                    {
                                        hasFixed = true;
                                        Debug.Log($"  ✓ 已添加 ContentUpdateGroupSchema");
                                    }
                                    else
                                    {
                                        Debug.LogWarning($"  ✗ AddSchema返回null，但可能已成功，尝试其他方法验证...");
                                        
                                        // 验证Schema是否真的添加了
                                        var verifySchemas = schemasProperty?.GetValue(group) as System.Collections.IList;
                                        bool verified = false;
                                        if (verifySchemas != null)
                                        {
                                            foreach (var schema in verifySchemas)
                                            {
                                                if (schema != null)
                                                {
                                                    var schemaType = schema.GetType();
                                                    if (schemaType == contentUpdateSchemaType || schemaType.IsSubclassOf(contentUpdateSchemaType))
                                                    {
                                                        verified = true;
                                                        hasFixed = true;
                                                        Debug.Log($"  ✓ 验证成功：ContentUpdateGroupSchema已添加");
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        
                                        if (!verified)
                                        {
                                            // 方法2: 尝试使用CreateDefaultSchema方法
                                            var createSchemaMethod = group.GetType().GetMethod("CreateDefaultSchema", new[] { typeof(System.Type) });
                                            if (createSchemaMethod != null)
                                            {
                                                result = createSchemaMethod.Invoke(group, new[] { contentUpdateSchemaType });
                                                if (result != null)
                                                {
                                                    hasFixed = true;
                                                    Debug.Log($"  ✓ 使用CreateDefaultSchema添加 ContentUpdateGroupSchema");
                                                }
                                            }
                                            
                                            // 方法3: 尝试从Default Local Group复制Schema
                                            if (!hasFixed)
                                            {
                                                var defaultGroup = groupsProperty?.GetValue(settings) as System.Collections.IEnumerable;
                                                if (defaultGroup != null)
                                                {
                                                    foreach (var defaultGrp in defaultGroup)
                                                    {
                                                        var defaultGroupNameProp = defaultGrp.GetType().GetProperty("Name");
                                                        if (defaultGroupNameProp?.GetValue(defaultGrp)?.ToString() == "Default Local Group")
                                                        {
                                                            var defaultSchemasProp = defaultGrp.GetType().GetProperty("Schemas");
                                                            var defaultSchemas = defaultSchemasProp?.GetValue(defaultGrp) as System.Collections.IList;
                                                            if (defaultSchemas != null)
                                                            {
                                                                foreach (var defaultSchema in defaultSchemas)
                                                                {
                                                                    if (defaultSchema != null)
                                                                    {
                                                                        var defaultSchemaType = defaultSchema.GetType();
                                                                        if (defaultSchemaType == contentUpdateSchemaType || defaultSchemaType.IsSubclassOf(contentUpdateSchemaType))
                                                                        {
                                                                            // 尝试使用AddSchema(object)方法
                                                                            foreach (var method in allMethods)
                                                                            {
                                                                                if (method.Name == "AddSchema" && method.GetParameters().Length == 1)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        var copyResult = method.Invoke(group, new[] { defaultSchema });
                                                                                        if (copyResult != null)
                                                                                        {
                                                                                            hasFixed = true;
                                                                                            Debug.Log($"  ✓ 从Default Local Group复制 ContentUpdateGroupSchema");
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                    catch { }
                                                                                }
                                                                            }
                                                                            if (hasFixed) break;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning($"  无法找到AddSchema方法，尝试直接使用AddressableAssetGroup API");
                                    // 如果反射失败，提示用户手动添加
                                    Debug.LogWarning($"  请手动为Group '{groupName}' 添加Schema：在Addressables Groups窗口中右键点击Group > Add Schema");
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogError($"  添加 ContentUpdateGroupSchema 失败: {ex.Message}\n{ex.StackTrace}");
                            }
                        }
                        
                        // 验证Schema是否真的添加成功
                        if (hasFixed)
                        {
                            schemas = schemasProperty?.GetValue(group) as System.Collections.IList;
                            int schemaCount = schemas?.Count ?? 0;
                            Debug.Log($"  验证: Group {groupName} 现在有 {schemaCount} 个Schema");
                        }
                    }
                }
                
                if (hasFixed)
                {
                    // 保存设置
                    try
                    {
                        var setDirtyMethod = settings.GetType().GetMethod("SetDirty", System.Type.EmptyTypes);
                        if (setDirtyMethod != null)
                        {
                            setDirtyMethod.Invoke(settings, null);
                        }
                        
                        // 强制标记为dirty
                        if (settings is UnityEngine.Object settingsObj)
                        {
                            EditorUtility.SetDirty(settingsObj);
                        }
                        
                        // 保存所有资源
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        
                        Debug.Log("✓ Group Schema修复完成，已保存设置");
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"保存设置失败: {ex.Message}");
                    }
                }
                else
                {
                    Debug.Log("所有Group的Schema都已正确配置");
                }
                
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"修复Group Schema失败: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
        
        /// <summary>
        /// 构建Addressables资源
        /// </summary>
        private void BuildAddressables()
        {
            var settings = GetAddressableSettings();
            if (settings == null)
            {
                EditorUtility.DisplayDialog("错误", "Addressables未初始化！", "确定");
                return;
            }
            
            // 检查是否有资源
            int entryCount = 0;
            
            try
            {
                // 尝试使用直接API
                if (settings is AddressableAssetSettings addressableSettings)
                {
                    foreach (var group in addressableSettings.groups)
                    {
                        if (group == null)
                            continue;
                        
                        try
                        {
                            // 安全地访问entries
                            if (group.entries != null)
                            {
                                entryCount += group.entries.Count;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogWarning($"无法访问Group '{group.name}' 的entries: {ex.Message}");
                        }
                    }
                }
                else
                {
                    // 回退到反射方法
                    var groupsProperty = settings.GetType().GetProperty("groups");
                    var groups = groupsProperty?.GetValue(settings) as System.Collections.IEnumerable;
                    
                    if (groups != null)
                    {
                        foreach (var group in groups)
                        {
                            if (group == null)
                                continue;
                            
                            try
                            {
                                var entriesProperty = group.GetType().GetProperty("entries");
                                var entries = entriesProperty?.GetValue(group) as System.Collections.IEnumerable;
                                if (entries != null)
                                {
                                    foreach (var entry in entries)
                                    {
                                        if (entry != null)
                                            entryCount++;
                                    }
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogWarning($"无法访问Group的entries: {ex.Message}");
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"获取资源条目数失败: {ex.Message}");
            }
            
            if (entryCount == 0)
            {
                EditorUtility.DisplayDialog("提示", "没有找到需要构建的资源！\n请先使用标注工具标记资源。", "确定");
                return;
            }
            
            // 确认构建
            if (!EditorUtility.DisplayDialog("确认构建",
                $"准备构建 {entryCount} 个资源条目到平台: {currentBuildTarget}\n\n" +
                $"输出路径: {buildOutputPath}\n\n" +
                $"是否继续？",
                "构建", "取消"))
            {
                return;
            }
            
            try
            {
                // 清理缓存（如果需要）
                if (clearBuildCache)
                {
                    CleanBuildCache();
                }
                
                // 检查并重新导入Lua文件（确保它们被识别为TextAsset）
                Debug.Log("=== 检查Lua文件 ===");
                CheckAndReimportLuaFiles();
                
                // 修复Group Schema
                Debug.Log("=== 检查并修复Group Schema ===");
                bool schemaFixed = false;
                
                // 尝试直接使用API
                if (settings is AddressableAssetSettings addressableSettings)
                {
                    Debug.Log("检测到AddressableAssetSettings类型，使用直接API");
                    schemaFixed = FixGroupSchemasDirect(addressableSettings);
                }
                else
                {
                    Debug.LogWarning($"settings类型不匹配: {settings?.GetType().FullName}，使用反射方法");
                    schemaFixed = FixGroupSchemas(settings);
                }
                
                // 验证所有Group的Schema状态
                if (settings is AddressableAssetSettings verifySettings)
                {
                    Debug.Log("=== 验证所有Group的Schema状态 ===");
                    int groupsWithSchema = 0;
                    int groupsWithoutSchema = 0;
                    
                    foreach (var group in verifySettings.groups)
                    {
                        if (group == null || group.name == "Default Local Group")
                            continue;
                        
                        bool hasBundled = false;
                        bool hasContentUpdate = false;
                        
                        foreach (var schema in group.Schemas)
                        {
                            if (schema is BundledAssetGroupSchema)
                                hasBundled = true;
                            if (schema is ContentUpdateGroupSchema)
                                hasContentUpdate = true;
                        }
                        
                        if (hasBundled && hasContentUpdate)
                        {
                            groupsWithSchema++;
                            Debug.Log($"✓ {group.name}: Schema完整");
                        }
                        else
                        {
                            groupsWithoutSchema++;
                            Debug.LogWarning($"✗ {group.name}: 缺少Schema (Bundled: {hasBundled}, ContentUpdate: {hasContentUpdate})");
                        }
                    }
                    
                    Debug.Log($"Schema状态总结: {groupsWithSchema} 个Group有完整Schema, {groupsWithoutSchema} 个Group缺少Schema");
                    
                    if (groupsWithoutSchema > 0)
                    {
                        Debug.LogWarning($"警告: {groupsWithoutSchema} 个Group缺少Schema，这些Group的资源不会被构建！");
                        Debug.LogWarning("请手动添加Schema或在Addressables Groups窗口中右键点击Group > Add Schema");
                    }
                }
                
                // 声明构建相关变量（提前声明以便在构建目标切换时使用）
                object buildResult = null;
                string buildError = null;
                bool buildSuccess = false;
                
                // 设置构建目标
                BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(currentBuildTarget);
                BuildTarget currentActiveTarget = EditorUserBuildSettings.activeBuildTarget;
                
                Debug.Log($"当前构建目标: {currentActiveTarget}");
                Debug.Log($"目标构建目标: {currentBuildTarget}");
                Debug.Log($"目标构建组: {targetGroup}");
                
                if (currentActiveTarget != currentBuildTarget)
                {
                    Debug.Log($"切换构建目标: {currentActiveTarget} -> {currentBuildTarget}");
                    bool switchResult = EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, currentBuildTarget);
                    if (!switchResult)
                    {
                        buildError = $"无法切换到构建目标: {currentBuildTarget}";
                        Debug.LogError(buildError);
                        EditorApplication.delayCall += () =>
                        {
                            EditorUtility.DisplayDialog("构建失败", 
                                $"无法切换到构建目标: {currentBuildTarget}\n\n" +
                                "请检查：\n" +
                                "1. 构建目标是否已安装必要的模块\n" +
                                "2. File > Build Settings 中是否正确配置\n" +
                                "3. 尝试手动切换构建目标后再构建", 
                                "确定");
                        };
                        return;
                    }
                    
                    // 等待切换完成
                    System.Threading.Thread.Sleep(100);
                }
                else
                {
                    Debug.Log("构建目标已匹配，无需切换");
                }
                
                // 执行构建
                EditorUtility.DisplayProgressBar("构建Addressables", "正在构建资源...", 0.5f);
                
                Debug.Log("=== 开始构建Addressables资源 ===");
                Debug.Log($"构建平台: {currentBuildTarget}");
                Debug.Log($"资源条目数: {entryCount}");
                
                // 使用反射调用构建方法
                System.Type settingsType = settings.GetType();
                
                // 方法1: 尝试使用ContentPipeline.BuildAssetBundles（更底层的方法）
                try
                {
                    Debug.Log("尝试使用ContentPipeline.BuildAssetBundles方法...");
                    System.Type contentPipelineType = System.Type.GetType("UnityEditor.AddressableAssets.Build.ContentPipeline, Unity.Addressables.Editor");
                    if (contentPipelineType != null)
                    {
                        MethodInfo buildMethod = contentPipelineType.GetMethod("BuildAssetBundles", 
                            BindingFlags.Public | BindingFlags.Static);
                        
                        if (buildMethod != null)
                        {
                            Debug.Log("找到ContentPipeline.BuildAssetBundles方法");
                            
                            // 检查参数类型
                            var parameters = buildMethod.GetParameters();
                            Debug.Log($"BuildAssetBundles参数数量: {parameters.Length}");
                            
                            if (parameters.Length >= 2)
                            {
                                // 尝试创建AddressableAssetsBuildContext
                                System.Type contextType = System.Type.GetType("UnityEditor.AddressableAssets.Build.AddressableAssetsBuildContext, Unity.Addressables.Editor");
                                if (contextType != null)
                                {
                                    object context = System.Activator.CreateInstance(contextType, new[] { settings });
                                    buildResult = buildMethod.Invoke(null, new[] { settings, context, false, false });
                                    buildSuccess = buildResult != null;
                                    Debug.Log($"ContentPipeline.BuildAssetBundles调用完成，结果: {(buildResult != null ? "成功" : "null")}");
                                }
                            }
                        }
                    }
                }
                catch (System.Exception ex1)
                {
                    Debug.LogWarning($"ContentPipeline.BuildAssetBundles失败: {ex1.Message}");
                }
                
                // 方法2: 如果方法1失败，尝试使用BuildPlayerContent
                if (!buildSuccess)
                {
                    try
                    {
                        Debug.Log("尝试使用AddressableAssetSettings.BuildPlayerContent方法...");
                        
                        // 首先尝试实例方法（需要传入settings）
                        MethodInfo buildMethod = settingsType.GetMethod("BuildPlayerContent", 
                            BindingFlags.Public | BindingFlags.Instance, 
                            null, 
                            System.Type.EmptyTypes, 
                            null);
                        
                            if (buildMethod != null)
                            {
                                Debug.Log("找到BuildPlayerContent实例方法（无参数）");
                                buildResult = buildMethod.Invoke(settings, null);
                                
                                // 检查返回类型
                                if (buildMethod.ReturnType == typeof(void))
                                {
                                    Debug.Log("BuildPlayerContent返回void，构建可能已完成");
                                    buildSuccess = true;
                                    buildResult = new object();
                                }
                                else
                                {
                                    buildSuccess = buildResult != null;
                                    Debug.Log($"BuildPlayerContent调用完成，返回类型: {buildMethod.ReturnType.Name}, 结果: {(buildResult != null ? "成功" : "null")}");
                                }
                            }
                        else
                        {
                            // 尝试静态方法
                            buildMethod = settingsType.GetMethod("BuildPlayerContent", 
                                BindingFlags.Public | BindingFlags.Static, 
                                null, 
                                System.Type.EmptyTypes, 
                                null);
                            
                        if (buildMethod != null)
                        {
                            Debug.Log("找到BuildPlayerContent静态方法（无参数）");
                            try
                            {
                                buildResult = buildMethod.Invoke(null, null);
                                
                                // 检查返回类型
                                if (buildMethod.ReturnType == typeof(void))
                                {
                                    Debug.Log("BuildPlayerContent返回void，构建可能已完成");
                                    buildSuccess = true; // void方法通常表示同步完成
                                    buildResult = new object(); // 创建一个占位符对象
                                }
                                else
                                {
                                    buildSuccess = buildResult != null;
                                    Debug.Log($"BuildPlayerContent调用完成，返回类型: {buildMethod.ReturnType.Name}, 结果: {(buildResult != null ? "成功" : "null")}");
                                }
                            }
                            catch (System.Reflection.TargetInvocationException tiex)
                            {
                                // 捕获反射调用时的异常
                                var innerEx = tiex.InnerException;
                                if (innerEx != null)
                                {
                                    buildError = $"构建失败: {innerEx.GetType().Name}: {innerEx.Message}";
                                    Debug.LogError(buildError);
                                    
                                    // 检查是否是Build Settings配置问题
                                    if (innerEx is System.InvalidOperationException)
                                    {
                                        Debug.LogError("Build Settings配置错误，请检查：");
                                        Debug.LogError($"  当前构建目标: {EditorUserBuildSettings.activeBuildTarget}");
                                        Debug.LogError($"  尝试切换到的目标: {currentBuildTarget}");
                                        Debug.LogError("建议：在File > Build Settings中检查构建配置");
                                    }
                                }
                                else
                                {
                                    buildError = $"构建失败: {tiex.Message}";
                                    Debug.LogError(buildError);
                                }
                                buildSuccess = false;
                                buildResult = null;
                            }
                        }
                            else
                            {
                                // 尝试查找所有BuildPlayerContent方法
                                MethodInfo[] allMethods = settingsType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                                foreach (var method in allMethods)
                                {
                                    if (method.Name == "BuildPlayerContent")
                                    {
                                        var parameters = method.GetParameters();
                                        Debug.Log($"找到BuildPlayerContent方法，参数数量: {parameters.Length}, 静态: {method.IsStatic}");
                                        
                                        if (parameters.Length == 0)
                                        {
                                            try
                                            {
                                                if (method.IsStatic)
                                                {
                                                    buildResult = method.Invoke(null, null);
                                                }
                                                else
                                                {
                                                    buildResult = method.Invoke(settings, null);
                                                }
                                                
                                                // 检查返回类型
                                                if (method.ReturnType == typeof(void))
                                                {
                                                    Debug.Log("BuildPlayerContent返回void，构建可能已完成");
                                                    buildSuccess = true;
                                                    buildResult = new object();
                                                }
                                                else
                                                {
                                                    buildSuccess = buildResult != null;
                                                    Debug.Log($"BuildPlayerContent调用完成，返回类型: {method.ReturnType.Name}, 结果: {(buildResult != null ? "成功" : "null")}");
                                                }
                                            }
                                            catch (System.Reflection.TargetInvocationException tiex)
                                            {
                                                // 捕获反射调用时的异常
                                                var innerEx = tiex.InnerException;
                                                if (innerEx != null)
                                                {
                                                    buildError = $"构建失败: {innerEx.GetType().Name}: {innerEx.Message}";
                                                    Debug.LogError(buildError);
                                                    
                                                    // 检查是否是Build Settings配置问题
                                                    if (innerEx is System.InvalidOperationException)
                                                    {
                                                        Debug.LogError("Build Settings配置错误，请检查：");
                                                        Debug.LogError($"  当前构建目标: {EditorUserBuildSettings.activeBuildTarget}");
                                                        Debug.LogError($"  尝试切换到的目标: {currentBuildTarget}");
                                                        Debug.LogError("建议：在File > Build Settings中检查构建配置");
                                                    }
                                                }
                                                else
                                                {
                                                    buildError = $"构建失败: {tiex.Message}";
                                                    Debug.LogError(buildError);
                                                }
                                                buildSuccess = false;
                                                buildResult = null;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Exception ex2)
                    {
                        buildError = $"调用BuildPlayerContent失败: {ex2.Message}";
                        Debug.LogError(buildError);
                        Debug.LogError($"详细错误: {ex2}");
                    }
                }
                
                // 方法3: 如果都失败，使用Unity菜单命令（最可靠）
                if (!buildSuccess && buildResult == null)
                {
                    Debug.LogWarning("反射调用失败，尝试使用Unity菜单命令构建...");
                    
                    EditorUtility.ClearProgressBar();
                    
                    // 使用延迟调用来避免GUI布局错误
                    EditorApplication.delayCall += () =>
                    {
                        try
                        {
                            // 尝试通过EditorApplication发送菜单命令
                            EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");
                            
                            EditorUtility.DisplayDialog("提示", 
                                "已打开Addressables Groups窗口\n\n" +
                                "请执行以下步骤：\n" +
                                "1. 点击窗口中的 Build 菜单\n" +
                                "2. 选择 New Build > Default Build Script\n" +
                                "3. 等待构建完成\n\n" +
                                "构建完成后，如果启用了自动复制，工具会自动将资源复制到StreamingAssets",
                                "确定");
                        }
                        catch (System.Exception ex3)
                        {
                            Debug.LogError($"打开Groups窗口失败: {ex3.Message}");
                        }
                    };
                    
                    return; // 提前返回，不继续执行
                }
                
                EditorUtility.ClearProgressBar();
                
                // 如果返回null但构建可能成功，检查构建输出目录
                if (buildResult == null && !buildSuccess)
                {
                    Debug.Log("构建返回null，检查构建输出目录...");
                    string checkPath = GetBuildOutputPath(settings, currentBuildTarget);
                    if (!string.IsNullOrEmpty(checkPath) && Directory.Exists(checkPath))
                    {
                        string[] files = Directory.GetFiles(checkPath, "*", SearchOption.AllDirectories);
                        if (files.Length > 0)
                        {
                            Debug.Log($"发现构建输出文件 {files.Length} 个，构建可能已成功");
                            buildSuccess = true;
                            buildResult = new object(); // 创建占位符
                        }
                        else
                        {
                            Debug.LogWarning($"构建输出目录存在但为空: {checkPath}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"构建输出目录不存在: {checkPath}");
                    }
                }
                
                // 处理构建结果
                if (buildResult != null || buildSuccess)
                {
                    // 检查构建结果类型（可能是IAsyncOperation或AddressablesPlayerBuildResult）
                    var resultType = buildResult.GetType();
                    
                    // 检查是否有Error属性
                    var errorProperty = resultType.GetProperty("Error");
                    var error = errorProperty?.GetValue(buildResult) as string;
                    
                    // 检查其他可能的错误属性
                    if (string.IsNullOrEmpty(error))
                    {
                        var exceptionProperty = resultType.GetProperty("Exception");
                        var exception = exceptionProperty?.GetValue(buildResult) as System.Exception;
                        if (exception != null)
                        {
                            error = exception.Message;
                        }
                    }
                    
                    // 检查是否有IsDone属性（异步操作）
                    var isDoneProperty = resultType.GetProperty("IsDone");
                    bool isDone = true;
                    if (isDoneProperty != null)
                    {
                        isDone = (bool)(isDoneProperty.GetValue(buildResult) ?? true);
                    }
                    
                    Debug.Log($"构建结果类型: {resultType.Name}");
                    Debug.Log($"构建结果错误信息: {(string.IsNullOrEmpty(error) ? "无错误" : error)}");
                    Debug.Log($"构建是否完成: {isDone}");
                    
                    // 检查构建输出目录是否存在文件，作为构建成功的验证
                    bool hasOutputFiles = false;
                    string checkOutputPath = GetBuildOutputPath(settings, currentBuildTarget);
                    if (!string.IsNullOrEmpty(checkOutputPath) && Directory.Exists(checkOutputPath))
                    {
                        string[] outputFiles = Directory.GetFiles(checkOutputPath, "*", SearchOption.AllDirectories);
                        int bundleFileCount = 0;
                        foreach (var file in outputFiles)
                        {
                            if (file.EndsWith(".bundle") && !file.EndsWith(".meta"))
                                bundleFileCount++;
                        }
                        hasOutputFiles = bundleFileCount > 0;
                        Debug.Log($"构建输出目录检查: {checkOutputPath}, Bundle文件数: {bundleFileCount}");
                    }
                    
                    // 如果构建成功且没有错误，并且有输出文件
                    if (isDone && string.IsNullOrEmpty(error) && string.IsNullOrEmpty(buildError) && hasOutputFiles)
                    {
                        // 获取输出路径
                        var profileSettingsProperty = settingsType.GetProperty("profileSettings");
                        var profileSettings = profileSettingsProperty?.GetValue(settings);
                        var activeProfileIdProperty = settingsType.GetProperty("activeProfileId");
                        var activeProfileId = activeProfileIdProperty?.GetValue(settings);
                        
                        string outputPathTemplate = "ServerData/[BuildTarget]";
                        string actualOutputPath = "";
                        
                        if (profileSettings != null && activeProfileId != null)
                        {
                            var getValueMethod = profileSettings.GetType().GetMethod("GetValueByName");
                            var evaluateMethod = profileSettings.GetType().GetMethod("EvaluateString");
                            
                            if (getValueMethod != null)
                            {
                                var localPath = getValueMethod.Invoke(profileSettings, new[] { activeProfileId, "Local.BuildPath" });
                                outputPathTemplate = localPath?.ToString() ?? outputPathTemplate;
                                
                                // 解析路径变量
                                if (evaluateMethod != null)
                                {
                                    actualOutputPath = evaluateMethod.Invoke(profileSettings, new[] { activeProfileId, outputPathTemplate })?.ToString() ?? "";
                                }
                            }
                        }
                        
                        // 如果无法解析，使用默认路径
                        if (string.IsNullOrEmpty(actualOutputPath))
                        {
                            actualOutputPath = outputPathTemplate.Replace("[BuildTarget]", currentBuildTarget.ToString());
                            if (!Path.IsPathRooted(actualOutputPath))
                            {
                                actualOutputPath = Path.Combine(Application.dataPath, "..", actualOutputPath);
                            }
                        }
                        
                        string finalOutputPath = actualOutputPath;
                        
                        // 如果启用自动复制到StreamingAssets
                        if (copyToStreamingAssets)
                        {
                            CopyToStreamingAssets(actualOutputPath, currentBuildTarget);
                            finalOutputPath = $"StreamingAssets/AA/{currentBuildTarget}";
                        }
                        
                        // 使用延迟调用来避免GUI布局错误
                        string finalMsg = $"构建成功！\n\n" +
                            $"输出路径: {actualOutputPath}\n" +
                            (copyToStreamingAssets ? $"已复制到: {finalOutputPath}\n" : "") +
                            $"构建的资源条目数: {entryCount}";
                        
                        EditorApplication.delayCall += () =>
                        {
                            EditorUtility.DisplayDialog("构建完成", finalMsg, "确定");
                        };
                        
                        Debug.Log($"Addressables构建成功！输出路径: {actualOutputPath}");
                        if (copyToStreamingAssets)
                        {
                            Debug.Log($"资源已复制到: {finalOutputPath}");
                        }
                    }
                    else
                    {
                        // 构建有错误或未完成
                        string errorMsg = !string.IsNullOrEmpty(error) ? error : buildError ?? "未知错误";
                        if (!isDone)
                        {
                            errorMsg = "构建未完成或仍在进行中";
                        }
                        else if (!hasOutputFiles && string.IsNullOrEmpty(error))
                        {
                            // 构建完成但没有输出文件，可能是构建失败
                            errorMsg = "构建完成但没有生成资源文件。可能的原因：\n" +
                                "1. Lua文件(.lua)不是Unity支持的文件类型，需要自定义AssetImporter\n" +
                                "2. 某些资源文件格式不支持\n" +
                                "3. 请检查Console窗口中的错误信息";
                        }
                        
                        Debug.LogError($"=== Addressables构建失败 ===");
                        Debug.LogError($"错误信息: {errorMsg}");
                        if (!hasOutputFiles)
                        {
                            Debug.LogError($"构建输出目录: {checkOutputPath}");
                            Debug.LogError($"未找到Bundle文件，构建可能失败");
                        }
                        
                        // 使用延迟调用来避免GUI布局错误
                        string errorDialogMsg = $"构建过程中出现错误：\n\n{errorMsg}\n\n请查看Console窗口获取详细信息。\n\n建议：\n1. 检查资源是否正确标注\n2. 检查Group是否有Schema（已自动修复）\n3. 尝试使用Unity菜单手动构建\n4. 查看Console窗口的详细错误";
                        
                        EditorApplication.delayCall += () =>
                        {
                            if (EditorUtility.DisplayDialog("构建失败", errorDialogMsg, "确定"))
                            {
                                // 提供手动构建的选项
                                if (EditorUtility.DisplayDialog("构建失败", 
                                    $"构建失败：{errorMsg}\n\n是否打开Addressables Groups窗口手动构建？", 
                                    "打开", "取消"))
                                {
                                    EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");
                                }
                            }
                        };
                    }
                }
                else if (buildResult == null)
                {
                    // buildResult为null，构建方法调用失败
                    string nullError = buildError ?? "构建返回null或构建方法调用失败";
                    Debug.LogError($"=== Addressables构建失败 ===");
                    Debug.LogError($"错误: {nullError}");
                    Debug.LogError("建议使用Unity菜单手动构建");
                    
                    // 使用延迟调用来避免GUI布局错误
                    EditorApplication.delayCall += () =>
                    {
                        EditorUtility.DisplayDialog("构建失败", 
                            $"{nullError}\n\n请查看Console窗口获取详细信息。\n\n建议：\n1. 使用\"使用Unity菜单构建\"按钮\n2. 或手动打开 Window > Asset Management > Addressables > Groups > Build > New Build", 
                            "确定");
                    };
                }
            }
            catch (System.Exception ex)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError($"Addressables构建失败: {ex.Message}\n{ex.StackTrace}");
                
                // 使用延迟调用来避免GUI布局错误
                string errorMsg = ex.Message;
                EditorApplication.delayCall += () =>
                {
                    EditorUtility.DisplayDialog("构建错误", $"构建失败: {errorMsg}", "确定");
                };
            }
        }
        
        /// <summary>
        /// 获取构建输出路径（检查多个可能的位置）
        /// </summary>
        private string GetBuildOutputPath(object settings, BuildTarget buildTarget)
        {
            try
            {
                System.Type settingsType = settings.GetType();
                var profileSettingsProperty = settingsType.GetProperty("profileSettings");
                var profileSettings = profileSettingsProperty?.GetValue(settings);
                var activeProfileIdProperty = settingsType.GetProperty("activeProfileId");
                var activeProfileId = activeProfileIdProperty?.GetValue(settings);
                
                string outputPathTemplate = "ServerData/[BuildTarget]";
                string actualOutputPath = "";
                
                if (profileSettings != null && activeProfileId != null)
                {
                    var getValueMethod = profileSettings.GetType().GetMethod("GetValueByName");
                    var evaluateMethod = profileSettings.GetType().GetMethod("EvaluateString");
                    
                    if (getValueMethod != null)
                    {
                        var localPath = getValueMethod.Invoke(profileSettings, new[] { activeProfileId, "Local.BuildPath" });
                        outputPathTemplate = localPath?.ToString() ?? outputPathTemplate;
                        
                        // 解析路径变量
                        if (evaluateMethod != null)
                        {
                            actualOutputPath = evaluateMethod.Invoke(profileSettings, new[] { activeProfileId, outputPathTemplate })?.ToString() ?? "";
                        }
                    }
                }
                
                // 如果无法解析，使用默认路径
                if (string.IsNullOrEmpty(actualOutputPath))
                {
                    actualOutputPath = outputPathTemplate.Replace("[BuildTarget]", buildTarget.ToString());
                    if (!Path.IsPathRooted(actualOutputPath))
                    {
                        actualOutputPath = Path.Combine(Application.dataPath, "..", actualOutputPath);
                    }
                }
                
                string resolvedPath = Path.GetFullPath(actualOutputPath);
                
                // 检查路径是否存在，如果不存在，尝试查找其他可能的位置
                if (!Directory.Exists(resolvedPath))
                {
                    Debug.LogWarning($"构建输出路径不存在: {resolvedPath}，尝试查找其他位置...");
                    
                    // 尝试多个可能的路径
                    string[] possiblePaths = new[]
                    {
                        Path.Combine(Application.dataPath, "..", "ServerData", buildTarget.ToString()),
                        Path.Combine(Application.dataPath, "ServerData", buildTarget.ToString()),
                        Path.Combine(Application.dataPath, "..", "Library", "com.unity.addressables", "aa", buildTarget.ToString(), buildTarget.ToString()),
                        Path.Combine(Application.dataPath, "..", "Library", "com.unity.addressables", "aa", buildTarget.ToString()),
                    };
                    
                    foreach (var path in possiblePaths)
                    {
                        string fullPath = Path.GetFullPath(path);
                        if (Directory.Exists(fullPath))
                        {
                            Debug.Log($"找到构建输出目录: {fullPath}");
                            return fullPath;
                        }
                    }
                    
                    Debug.LogWarning($"所有可能的构建路径都不存在，使用配置的路径: {resolvedPath}");
                }
                
                return resolvedPath;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"获取构建输出路径失败: {ex.Message}");
                // 返回默认路径
                string defaultPath = Path.Combine(Application.dataPath, "..", "ServerData", buildTarget.ToString());
                return Path.GetFullPath(defaultPath);
            }
        }
        
        /// <summary>
        /// 确保StreamingAssets目录存在
        /// </summary>
        private void EnsureStreamingAssetsExists()
        {
            string streamingAssetsPath = Application.streamingAssetsPath;
            if (!Directory.Exists(streamingAssetsPath))
            {
                Debug.Log($"创建StreamingAssets目录: {streamingAssetsPath}");
                Directory.CreateDirectory(streamingAssetsPath);
                AssetDatabase.Refresh();
            }
        }
        
        /// <summary>
        /// 复制资源到StreamingAssets目录
        /// </summary>
        private void CopyToStreamingAssets(string buildPath, BuildTarget buildTarget)
        {
            try
            {
                // 确保StreamingAssets目录存在
                EnsureStreamingAssetsExists();
                
                // buildPath已经是解析后的绝对路径
                string actualBuildPath = buildPath;
                
                // 确保路径存在
                if (!Directory.Exists(actualBuildPath))
                {
                    Debug.LogWarning($"构建路径不存在: {actualBuildPath}，尝试查找...");
                    
                    // 尝试在多个可能的位置查找
                    string projectRoot = Application.dataPath.Replace("/Assets", "").Replace("\\Assets", "");
                    string[] possiblePaths = new[]
                    {
                        Path.Combine(projectRoot, "ServerData", buildTarget.ToString()),
                        Path.Combine(Application.dataPath, "..", "ServerData", buildTarget.ToString()),
                        Path.Combine(Application.dataPath, "ServerData", buildTarget.ToString()),
                        Path.Combine(projectRoot, "Library", "com.unity.addressables", "aa", buildTarget.ToString(), buildTarget.ToString()),
                        Path.Combine(projectRoot, "Library", "com.unity.addressables", "aa", buildTarget.ToString()),
                        Path.Combine(Application.dataPath, "..", "Library", "com.unity.addressables", "aa", buildTarget.ToString(), buildTarget.ToString()),
                        Path.Combine(Application.dataPath, "..", "Library", "com.unity.addressables", "aa", buildTarget.ToString()),
                    };
                    
                    bool found = false;
                    foreach (var path in possiblePaths)
                    {
                        string fullPath = Path.GetFullPath(path);
                        if (Directory.Exists(fullPath))
                        {
                            // 检查目录中是否有文件（排除.meta文件）
                            string[] checkFiles = Directory.GetFiles(fullPath, "*", SearchOption.AllDirectories);
                            int checkFileCount = 0;
                            foreach (var checkFile in checkFiles)
                            {
                                if (!checkFile.EndsWith(".meta"))
                                    checkFileCount++;
                            }
                            
                            if (checkFileCount > 0)
                            {
                                actualBuildPath = fullPath;
                                found = true;
                                Debug.Log($"找到构建输出目录: {fullPath} (包含 {checkFileCount} 个文件)");
                                break;
                            }
                        }
                    }
                    
                    if (!found)
                    {
                        Debug.LogError($"无法找到构建输出目录，请检查构建是否成功");
                        Debug.LogError($"已检查的路径:");
                        foreach (var path in possiblePaths)
                        {
                            Debug.LogError($"  - {Path.GetFullPath(path)}");
                        }
                        
                        EditorApplication.delayCall += () =>
                        {
                            EditorUtility.DisplayDialog("错误", 
                                $"无法找到构建输出目录：{actualBuildPath}\n\n" +
                                $"可能的原因：\n" +
                                $"1. Group没有Schema，导致没有资源被构建\n" +
                                $"2. 构建输出路径配置不正确\n" +
                                $"3. 请先手动添加Schema后再构建", 
                                "确定");
                        };
                        return;
                    }
                }
                
                // StreamingAssets目标路径
                string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, "AA", buildTarget.ToString());
                
                Debug.Log($"准备复制资源到: {streamingAssetsPath}");
                
                // 确保目标目录存在（会自动创建父目录）
                if (Directory.Exists(streamingAssetsPath))
                {
                    Debug.Log($"目标目录已存在，删除旧文件: {streamingAssetsPath}");
                    Directory.Delete(streamingAssetsPath, true);
                }
                
                // 创建目录（包括所有父目录）
                Directory.CreateDirectory(streamingAssetsPath);
                Debug.Log($"已创建目录: {streamingAssetsPath}");
                
                // 复制所有文件（包括bundle文件）
                string[] files = Directory.GetFiles(actualBuildPath, "*", SearchOption.AllDirectories);
                int fileCount = 0;
                
                EditorUtility.DisplayProgressBar("复制到StreamingAssets", "正在复制文件...", 0);
                
                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];
                    
                    // 跳过.meta文件
                    if (file.EndsWith(".meta"))
                        continue;
                    
                    string relativePath = file.Substring(actualBuildPath.Length).TrimStart(Path.DirectorySeparatorChar, '/');
                    string destPath = Path.Combine(streamingAssetsPath, relativePath);
                    
                    // 确保目标目录存在
                    string destDir = Path.GetDirectoryName(destPath);
                    if (!Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }
                    
                    File.Copy(file, destPath, true);
                    fileCount++;
                    
                    EditorUtility.DisplayProgressBar("复制到StreamingAssets", 
                        $"正在复制文件... ({fileCount}/{files.Length})", 
                        (float)fileCount / files.Length);
                }
                
                // 检查并复制catalog文件（资源目录清单，用于热更新）
                // catalog文件通常在父目录中（如 aa/Android/ 而不是 aa/Android/Android/）
                string parentBuildPath = Path.GetDirectoryName(actualBuildPath);
                if (Directory.Exists(parentBuildPath))
                {
                    // 查找catalog相关文件
                    string[] catalogFiles = new[]
                    {
                        "catalog.bin",
                        "catalog.hash",
                        "settings.json"
                    };
                    
                    foreach (string catalogFile in catalogFiles)
                    {
                        string catalogSourcePath = Path.Combine(parentBuildPath, catalogFile);
                        if (File.Exists(catalogSourcePath))
                        {
                            string catalogDestPath = Path.Combine(streamingAssetsPath, catalogFile);
                            File.Copy(catalogSourcePath, catalogDestPath, true);
                            fileCount++;
                            Debug.Log($"已复制catalog文件: {catalogFile}");
                        }
                    }
                    
                    // 复制AddressablesLink目录（如果存在）
                    string linkSourceDir = Path.Combine(parentBuildPath, "AddressablesLink");
                    if (Directory.Exists(linkSourceDir))
                    {
                        string linkDestDir = Path.Combine(streamingAssetsPath, "AddressablesLink");
                        if (Directory.Exists(linkDestDir))
                        {
                            Directory.Delete(linkDestDir, true);
                        }
                        Directory.CreateDirectory(linkDestDir);
                        
                        string[] linkFiles = Directory.GetFiles(linkSourceDir, "*", SearchOption.AllDirectories);
                        foreach (string linkFile in linkFiles)
                        {
                            if (linkFile.EndsWith(".meta"))
                                continue;
                            
                            string relativePath = linkFile.Substring(linkSourceDir.Length).TrimStart(Path.DirectorySeparatorChar, '/');
                            string destPath = Path.Combine(linkDestDir, relativePath);
                            
                            string destDir = Path.GetDirectoryName(destPath);
                            if (!Directory.Exists(destDir))
                            {
                                Directory.CreateDirectory(destDir);
                            }
                            
                            File.Copy(linkFile, destPath, true);
                            fileCount++;
                        }
                        Debug.Log($"已复制AddressablesLink目录");
                    }
                }
                
                EditorUtility.ClearProgressBar();
                
                // 刷新AssetDatabase
                AssetDatabase.Refresh();
                
                Debug.Log($"已复制 {fileCount} 个文件到 StreamingAssets/AA/{buildTarget}（包括catalog文件）");
            }
            catch (System.Exception ex)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError($"复制到StreamingAssets失败: {ex.Message}\n{ex.StackTrace}");
            }
        }
        
        /// <summary>
        /// 清理构建缓存
        /// </summary>
        private void CleanBuildCache()
        {
            var settings = GetAddressableSettings();
            if (settings == null)
            {
                EditorUtility.DisplayDialog("清理失败", "无法获取AddressableAssetSettings。", "确定");
                return;
            }
            
            if (EditorUtility.DisplayDialog("确认清理",
                "确定要清理Addressables构建缓存吗？\n\n" +
                "这将删除所有已构建的资源文件。",
                "清理", "取消"))
            {
                try
                {
                    bool cleaned = false;
                    
                    // 方法1: 尝试使用反射调用静态方法CleanPlayerContent
                    if (!cleaned)
                    {
                        try
                        {
                            System.Type settingsType = settings.GetType();
                            // 尝试查找静态方法CleanPlayerContent
                            MethodInfo cleanMethod = settingsType.GetMethod("CleanPlayerContent", 
                                BindingFlags.Public | BindingFlags.Static,
                                null,
                                new System.Type[] { typeof(AddressableAssetSettings), typeof(UnityEditor.AddressableAssets.Build.IDataBuilder) },
                                null);
                            
                            if (cleanMethod != null)
                            {
                                // 需要获取IDataBuilder，通常使用DefaultDataBuilder
                                System.Type dataBuilderType = System.Type.GetType("UnityEditor.AddressableAssets.Build.DataBuilders.DefaultDataBuilder, Unity.Addressables.Editor");
                                if (dataBuilderType != null)
                                {
                                    object dataBuilder = System.Activator.CreateInstance(dataBuilderType);
                                    cleanMethod.Invoke(null, new[] { settings, dataBuilder });
                                    cleaned = true;
                                    Debug.Log("使用静态方法CleanPlayerContent清理缓存");
                                }
                            }
                            
                            // 如果找不到带参数的静态方法，尝试查找无参数的静态方法
                            if (!cleaned)
                            {
                                cleanMethod = settingsType.GetMethod("CleanPlayerContent", 
                                    BindingFlags.Public | BindingFlags.Static,
                                    null,
                                    System.Type.EmptyTypes,
                                    null);
                                
                                if (cleanMethod != null)
                                {
                                    cleanMethod.Invoke(null, null);
                                    cleaned = true;
                                    Debug.Log("使用静态方法CleanPlayerContent(无参数)清理缓存");
                                }
                            }
                        }
                        catch (System.Exception ex1)
                        {
                            Debug.LogWarning($"反射调用CleanPlayerContent失败: {ex1.Message}，尝试其他方法...");
                        }
                    }
                    
                    // 方法3: 尝试使用ContentPipeline.CleanContent
                    if (!cleaned)
                    {
                        try
                        {
                            System.Type contentPipelineType = System.Type.GetType("UnityEditor.AddressableAssets.Build.ContentPipeline, Unity.Addressables.Editor");
                            if (contentPipelineType != null)
                            {
                                MethodInfo cleanMethod = contentPipelineType.GetMethod("CleanContent", BindingFlags.Public | BindingFlags.Static);
                                if (cleanMethod != null)
                                {
                                    cleanMethod.Invoke(null, new[] { settings });
                                    cleaned = true;
                                    Debug.Log("使用ContentPipeline.CleanContent清理缓存");
                                }
                            }
                        }
                        catch (System.Exception ex3)
                        {
                            Debug.LogWarning($"ContentPipeline.CleanContent失败: {ex3.Message}");
                        }
                    }
                    
                    // 方法4: 手动删除构建输出目录
                    if (!cleaned)
                    {
                        try
                        {
                            // 获取构建输出路径
                            string buildPath = GetBuildOutputPath(settings, currentBuildTarget);
                            if (!string.IsNullOrEmpty(buildPath) && Directory.Exists(buildPath))
                            {
                                Directory.Delete(buildPath, true);
                                Debug.Log($"手动删除构建输出目录: {buildPath}");
                                cleaned = true;
                            }
                            
                            // 也删除StreamingAssets中的资源
                            string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, "AA", currentBuildTarget.ToString());
                            if (Directory.Exists(streamingAssetsPath))
                            {
                                Directory.Delete(streamingAssetsPath, true);
                                Debug.Log($"手动删除StreamingAssets资源目录: {streamingAssetsPath}");
                            }
                            
                            // 删除所有平台的构建输出
                            string serverDataPath = Path.Combine(Application.dataPath, "..", "ServerData");
                            if (Directory.Exists(serverDataPath))
                            {
                                string[] platformDirs = Directory.GetDirectories(serverDataPath);
                                foreach (string platformDir in platformDirs)
                                {
                                    try
                                    {
                                        Directory.Delete(platformDir, true);
                                        Debug.Log($"删除平台构建目录: {platformDir}");
                                    }
                                    catch (System.Exception ex4)
                                    {
                                        Debug.LogWarning($"删除平台目录失败: {platformDir}, {ex4.Message}");
                                    }
                                }
                            }
                        }
                        catch (System.Exception ex4)
                        {
                            Debug.LogWarning($"手动删除构建目录失败: {ex4.Message}");
                        }
                    }
                    
                    if (cleaned)
                    {
                        AssetDatabase.Refresh();
                        EditorUtility.DisplayDialog("清理完成", "构建缓存已清理。", "确定");
                        Debug.Log("✓ Addressables构建缓存清理完成");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("清理失败", 
                            "无法找到清理方法。\n\n" +
                            "请手动清理：\n" +
                            "1. 删除 ServerData 目录\n" +
                            "2. 删除 StreamingAssets/AA 目录\n" +
                            "3. 或在Addressables Groups窗口中点击 Tools > Clean Build > Clean All", 
                            "确定");
                        Debug.LogWarning("所有清理方法都失败了，请手动清理构建缓存");
                    }
                }
                catch (System.Exception ex)
                {
                    EditorUtility.DisplayDialog("清理失败", $"清理缓存失败: {ex.Message}", "确定");
                    Debug.LogError($"清理Addressables缓存失败: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }
    }
}

