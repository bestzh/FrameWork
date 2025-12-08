using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace Tools.Addressable
{
    /// <summary>
    /// 统一Addressable批量标注工具 - 自动扫描Resources目录下的所有资源
    /// </summary>
    public class UnifiedAddressableTool : EditorWindow
    {
        /// <summary>
        /// 资源信息
        /// </summary>
        private class AssetInfo
        {
            public string path;
            public string address;  // Addressable地址（相对于Resources目录，无扩展名）
            public string groupName;
            public string type;
            public string extension;
            public bool isSelected;
            public bool isAlreadyAddressable;
            public Object asset;
            
            public AssetInfo(string assetPath, string resourcesRoot)
            {
                path = assetPath;
                
                // 特殊处理.lua.txt文件
                if (assetPath.ToLower().EndsWith(".lua.txt"))
                {
                    extension = ".lua.txt";
                }
                else
                {
                    extension = Path.GetExtension(assetPath).ToLower();
                }
                
                type = GetResourceType(extension);
                groupName = GetGroupName(extension);
                
                // 生成地址：相对于Resources目录，移除扩展名
                // Assets/Resources/Table/AddDeviceProgess.csv -> Table/AddDeviceProgess
                // Assets/Resources/lua/ui/ui_helper.lua.txt -> lua/ui/ui_helper
                string relativePath = assetPath;
                if (relativePath.Contains("Resources/"))
                {
                    int resourcesIndex = relativePath.IndexOf("Resources/");
                    relativePath = relativePath.Substring(resourcesIndex + "Resources/".Length);
                }
                else if (relativePath.StartsWith("Assets/"))
                {
                    relativePath = relativePath.Substring("Assets/".Length);
                }
                
                // 移除扩展名
                if (extension == ".lua.txt")
                {
                    // 对于.lua.txt文件，移除.lua.txt，保留.lua部分
                    address = relativePath.Replace(".lua.txt", "");
                }
                else
                {
                    address = Path.ChangeExtension(relativePath, null);
                }
                address = address.Replace('\\', '/');
                
                asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                
                // 检测是否已经是Addressable
                if (IsAddressablesInstalled())
                {
                    try
                    {
                        var settings = GetAddressableSettings();
                        if (settings != null)
                        {
                            string guid = AssetDatabase.AssetPathToGUID(assetPath);
                            var entry = settings.FindAssetEntry(guid);
                            isAlreadyAddressable = entry != null;
                        }
                    }
                    catch
                    {
                        isAlreadyAddressable = false;
                    }
                }
            }
            
            private static string GetResourceType(string extension)
            {
                switch (extension)
                {
                    case ".lua":
                    case ".lua.txt": return "Lua脚本";
                    case ".csv": return "Table CSV";
                    case ".bytes": return "TableBin Bytes";
                    case ".prefab": return "Prefab";
                    case ".png":
                    case ".jpg":
                    case ".jpeg":
                    case ".tga":
                    case ".tif":
                    case ".tiff": return "图片";
                    case ".mp3":
                    case ".wav":
                    case ".ogg": return "音频";
                    case ".txt": return "文本";
                    case ".json": return "文本";
                    case ".mat": return "材质";
                    case ".shader": return "着色器";
                    case ".anim": return "动画";
                    case ".controller": return "动画控制器";
                    default: return "其他";
                }
            }
            
            private static string GetGroupName(string extension)
            {
                switch (extension)
                {
                    case ".lua":
                    case ".lua.txt": return "Lua_Group";
                    case ".csv": return "Table_Group";
                    case ".bytes": return "TableBin_Group";
                    case ".prefab": return "Prefab_Group";
                    case ".png":
                    case ".jpg":
                    case ".jpeg":
                    case ".tga":
                    case ".tif":
                    case ".tiff": return "Texture_Group";
                    case ".mp3":
                    case ".wav":
                    case ".ogg": return "Audio_Group";
                    case ".txt": return "Text_Group";
                    case ".json": return "Text_Group";
                    case ".mat": return "Material_Group";
                    case ".shader": return "Shader_Group";
                    case ".anim": return "Animation_Group";
                    case ".controller": return "Animator_Group";
                    default: return "Other_Group";
                }
            }
            
            private static bool IsAddressablesInstalled()
            {
                System.Type settingsType = System.Type.GetType("UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject, Unity.Addressables.Editor");
                return settingsType != null;
            }
            
            private static AddressableAssetSettings GetAddressableSettings()
            {
                if (!IsAddressablesInstalled())
                    return null;
                return AddressableAssetSettingsDefaultObject.Settings;
            }
        }
        
        private Vector2 scrollPosition;
        private List<AssetInfo> allAssetInfos = new List<AssetInfo>();
        private Dictionary<string, List<AssetInfo>> assetsByType = new Dictionary<string, List<AssetInfo>>();
        
        // UI状态
        private bool selectAll = true;
        private string filterType = "全部";
        private string resourcesPath = "Assets/Resources";
        
        // 支持的扩展名
        private static readonly string[] SupportedExtensions = new[]
        {
            ".lua", ".lua.txt", ".csv", ".bytes", ".prefab",
            ".png", ".jpg", ".jpeg", ".tga", ".tif", ".tiff",
            ".mp3", ".wav", ".ogg",
            ".txt", ".json",
            ".mat", ".shader", ".anim", ".controller"
        };
        
        /// <summary>
        /// 检查Addressables包是否已安装
        /// </summary>
        private static bool IsAddressablesInstalled()
        {
            System.Type settingsType = System.Type.GetType("UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject, Unity.Addressables.Editor");
            if (settingsType != null)
                return true;
            
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
        private static AddressableAssetSettings GetAddressableSettings()
        {
            if (!IsAddressablesInstalled())
                return null;
            
            return AddressableAssetSettingsDefaultObject.Settings;
        }
        
        /// <summary>
        /// 初始化Addressables
        /// </summary>
        private static bool InitializeAddressables()
        {
            if (!IsAddressablesInstalled())
            {
                Debug.LogError("Addressables包未安装！");
                return false;
            }
            
            try
            {
                var existingSettings = AddressableAssetSettingsDefaultObject.Settings;
                if (existingSettings != null)
                {
                    Debug.Log("Addressables已经初始化！");
                    return true;
                }
                
                string dataFolder = "Assets/AddressableAssetsData";
                if (!Directory.Exists(dataFolder))
                {
                    Directory.CreateDirectory(dataFolder);
                    AssetDatabase.Refresh();
                }
                
                var settings = AddressableAssetSettings.Create(dataFolder, "AddressableAssetSettings", true, true);
                
                if (settings != null)
                {
                    Debug.Log("Addressables初始化成功！");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    
                    var verifySettings = AddressableAssetSettingsDefaultObject.Settings;
                    return verifySettings != null;
                }
                
                return false;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"初始化Addressables时出错: {ex.Message}");
                return false;
            }
        }
        
        [MenuItem("Tools/Addressable/统一批量标注工具")]
        public static void ShowWindow()
        {
            UnifiedAddressableTool window = GetWindow<UnifiedAddressableTool>("Addressable统一工具");
            window.minSize = new Vector2(1000, 600);
            window.ScanAllAssets();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Addressable统一批量标注工具", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // 检查Addressables状态
            bool addressablesInstalled = IsAddressablesInstalled();
            bool addressablesInitialized = addressablesInstalled && GetAddressableSettings() != null;
            
            if (!addressablesInstalled)
            {
                EditorGUILayout.HelpBox(
                    "Addressables包未安装！\n" +
                    "请先安装Addressables包：Window > Package Manager > 搜索 'Addressables' > Install",
                    MessageType.Error);
            }
            else if (!addressablesInitialized)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.HelpBox(
                    "Addressables未初始化！\n" +
                    "点击下方按钮初始化Addressables",
                    MessageType.Warning);
                
                if (GUILayout.Button("初始化Addressables", GUILayout.Height(30)))
                {
                    if (InitializeAddressables())
                    {
                        EditorUtility.DisplayDialog("成功", "Addressables初始化成功！", "确定");
                        ScanAllAssets(); // 重新扫描
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("失败",
                            "初始化失败，请手动初始化：\n" +
                            "1. Window > Asset Management > Addressables > Groups\n" +
                            "2. 如果提示初始化，点击 'Create Addressables Settings'",
                            "确定");
                    }
                }
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.Space();
            
            // Resources路径配置
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Resources路径:", GUILayout.Width(100));
            resourcesPath = EditorGUILayout.TextField(resourcesPath);
            if (GUILayout.Button("浏览", GUILayout.Width(60)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("选择Resources目录", resourcesPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        resourcesPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // 操作按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("自动扫描Resources目录", GUILayout.Height(30)))
            {
                ScanAllAssets();
            }
            
            if (GUILayout.Button("全选/取消全选", GUILayout.Height(30)))
            {
                selectAll = !selectAll;
                foreach (var info in allAssetInfos)
                {
                    if (!info.isAlreadyAddressable)
                        info.isSelected = selectAll;
                }
            }
            
            GUI.enabled = allAssetInfos.Count(info => info.isSelected && !info.isAlreadyAddressable) > 0 
                && addressablesInstalled && addressablesInitialized;
            if (GUILayout.Button("一键标注所有选中资源", GUILayout.Height(30)))
            {
                BatchMarkAsAddressable();
            }
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // 统计信息
            int selectedCount = allAssetInfos.Count(info => info.isSelected && !info.isAlreadyAddressable);
            int alreadyAddressableCount = allAssetInfos.Count(info => info.isAlreadyAddressable);
            int totalCount = allAssetInfos.Count;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"总计: {totalCount}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"已标记: {alreadyAddressableCount}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"待标记: {selectedCount}", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            
            // 类型过滤
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("类型过滤:", GUILayout.Width(80));
            List<string> typeOptions = new List<string> { "全部" };
            typeOptions.AddRange(assetsByType.Keys.OrderBy(k => k));
            int currentIndex = typeOptions.IndexOf(filterType);
            if (currentIndex < 0) currentIndex = 0;
            int newIndex = EditorGUILayout.Popup(currentIndex, typeOptions.ToArray());
            filterType = typeOptions[newIndex];
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // 资源列表
            EditorGUILayout.LabelField("资源列表:", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            var displayAssets = filterType == "全部" 
                ? allAssetInfos 
                : assetsByType.ContainsKey(filterType) ? assetsByType[filterType] : new List<AssetInfo>();
            
            // 表头
            EditorGUILayout.BeginHorizontal("box", GUILayout.Height(20));
            EditorGUILayout.LabelField("", GUILayout.Width(20)); // 复选框列
            EditorGUILayout.LabelField("类型", GUILayout.Width(80));
            EditorGUILayout.LabelField("文件名", GUILayout.Width(250));
            EditorGUILayout.LabelField("组名", GUILayout.Width(120));
            EditorGUILayout.LabelField("Addressable地址", GUILayout.Width(300));
            EditorGUILayout.LabelField("状态", GUILayout.Width(80));
            EditorGUILayout.LabelField("操作", GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            
            foreach (var info in displayAssets)
            {
                EditorGUILayout.BeginHorizontal("box");
                
                GUI.enabled = !info.isAlreadyAddressable;
                info.isSelected = EditorGUILayout.Toggle(info.isSelected, GUILayout.Width(20));
                GUI.enabled = true;
                
                EditorGUILayout.LabelField(info.type, GUILayout.Width(80));
                EditorGUILayout.LabelField(Path.GetFileName(info.path), GUILayout.Width(250));
                EditorGUILayout.LabelField(info.groupName, GUILayout.Width(120));
                EditorGUILayout.LabelField(info.address, GUILayout.Width(300));
                
                if (info.isAlreadyAddressable)
                {
                    EditorGUILayout.LabelField("✓ 已标记", EditorStyles.boldLabel, GUILayout.Width(80));
                }
                else
                {
                    EditorGUILayout.LabelField("未标记", GUILayout.Width(80));
                }
                
                if (GUILayout.Button("定位", GUILayout.Width(50)))
                {
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = info.asset;
                    EditorGUIUtility.PingObject(info.asset);
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        /// <summary>
        /// 扫描所有资源
        /// </summary>
        private void ScanAllAssets()
        {
            allAssetInfos.Clear();
            assetsByType.Clear();
            
            if (!Directory.Exists(resourcesPath))
            {
                EditorUtility.DisplayDialog("错误", $"Resources目录不存在: {resourcesPath}", "确定");
                return;
            }
            
            // 先扫描.lua.txt文件（需要特殊处理，避免被.txt扫描到）
            string[] luaTxtFiles = Directory.GetFiles(resourcesPath, "*.lua.txt", SearchOption.AllDirectories);
            foreach (string filePath in luaTxtFiles)
            {
                string assetPath = filePath.Replace('\\', '/');
                if (assetPath.StartsWith(Application.dataPath))
                {
                    assetPath = "Assets" + assetPath.Substring(Application.dataPath.Length);
                }
                
                // 跳过.meta文件
                if (assetPath.EndsWith(".meta"))
                    continue;
                
                var info = new AssetInfo(assetPath, resourcesPath);
                allAssetInfos.Add(info);
                
                if (!assetsByType.ContainsKey(info.type))
                {
                    assetsByType[info.type] = new List<AssetInfo>();
                }
                assetsByType[info.type].Add(info);
            }
            
            // 然后扫描其他支持的文件（排除.lua.txt，避免重复）
            foreach (var extension in SupportedExtensions)
            {
                // 跳过.lua.txt，因为已经单独处理了
                if (extension == ".lua.txt")
                    continue;
                
                string[] files = Directory.GetFiles(resourcesPath, "*" + extension, SearchOption.AllDirectories);
                
                // 对于.txt文件，需要排除.lua.txt文件（避免重复）
                if (extension == ".txt")
                {
                    files = files.Where(f => !f.ToLower().EndsWith(".lua.txt")).ToArray();
                }
                
                foreach (string filePath in files)
                {
                    string assetPath = filePath.Replace('\\', '/');
                    if (assetPath.StartsWith(Application.dataPath))
                    {
                        assetPath = "Assets" + assetPath.Substring(Application.dataPath.Length);
                    }
                    
                    // 跳过.meta文件
                    if (assetPath.EndsWith(".meta"))
                        continue;
                    
                    var info = new AssetInfo(assetPath, resourcesPath);
                    allAssetInfos.Add(info);
                    
                    if (!assetsByType.ContainsKey(info.type))
                    {
                        assetsByType[info.type] = new List<AssetInfo>();
                    }
                    assetsByType[info.type].Add(info);
                }
            }
            
            // 按路径排序
            allAssetInfos = allAssetInfos.OrderBy(info => info.path).ToList();
            
            Debug.Log($"扫描完成，找到 {allAssetInfos.Count} 个资源");
        }
        
        /// <summary>
        /// 批量标记为Addressable
        /// </summary>
        private void BatchMarkAsAddressable()
        {
            if (!IsAddressablesInstalled())
            {
                EditorUtility.DisplayDialog("错误",
                    "Addressables包未安装，请先安装Addressables包。",
                    "确定");
                return;
            }
            
            var settings = GetAddressableSettings();
            if (settings == null)
            {
                bool initResult = EditorUtility.DisplayDialog("Addressables未初始化",
                    "Addressable设置未找到，是否自动初始化Addressables？",
                    "是，自动初始化",
                    "取消");
                
                if (initResult)
                {
                    if (InitializeAddressables())
                    {
                        settings = GetAddressableSettings();
                        if (settings == null)
                        {
                            EditorUtility.DisplayDialog("错误", "初始化失败", "确定");
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            
            // 按组分组处理
            Dictionary<string, AddressableAssetGroup> groups = new Dictionary<string, AddressableAssetGroup>();
            Dictionary<string, List<AssetInfo>> assetsByGroup = new Dictionary<string, List<AssetInfo>>();
            
            foreach (var info in allAssetInfos)
            {
                if (!info.isSelected || info.isAlreadyAddressable)
                    continue;
                
                if (!assetsByGroup.ContainsKey(info.groupName))
                {
                    assetsByGroup[info.groupName] = new List<AssetInfo>();
                }
                assetsByGroup[info.groupName].Add(info);
            }
            
            int totalSuccess = 0;
            int totalFail = 0;
            
            // 为每个组创建组并标记资源
            foreach (var groupPair in assetsByGroup)
            {
                string groupName = groupPair.Key;
                var assets = groupPair.Value;
                
                // 获取或创建组（使用postEvent=false避免触发窗口刷新）
                AddressableAssetGroup group = settings.FindGroup(groupName);
                if (group == null)
                {
                    try
                    {
                        // 使用postEvent=false避免立即触发窗口刷新，减少NullReferenceException
                        group = settings.CreateGroup(groupName, false, false, false, null);
                        if (group != null)
                        {
                            Debug.Log($"创建Addressable组: {groupName}");
                        }
                        else
                        {
                            Debug.LogError($"创建组失败: {groupName}");
                            totalFail += assets.Count;
                            continue;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"创建组异常: {groupName}, 错误: {ex.Message}");
                        totalFail += assets.Count;
                        continue;
                    }
                }
                
                int successCount = 0;
                int failCount = 0;
                
                foreach (var info in assets)
                {
                    try
                    {
                        string guid = AssetDatabase.AssetPathToGUID(info.path);
                        if (string.IsNullOrEmpty(guid))
                        {
                            Debug.LogWarning($"无法获取GUID: {info.path}");
                            failCount++;
                            continue;
                        }
                        
                        // 检查是否已存在
                        var existingEntry = settings.FindAssetEntry(guid);
                        if (existingEntry != null)
                        {
                            existingEntry.address = info.address;
                            existingEntry.SetAddress(info.address, false);
                            if (existingEntry.parentGroup != group)
                            {
                                settings.MoveEntry(existingEntry, group);
                            }
                            successCount++;
                            continue;
                        }
                        
                        // 添加到Addressables（使用postEvent=false避免触发窗口刷新）
                        var entry = settings.CreateOrMoveEntry(guid, group, false, false);
                        if (entry != null)
                        {
                            entry.address = info.address;
                            entry.SetAddress(info.address, false);
                            successCount++;
                        }
                        else
                        {
                            Debug.LogWarning($"创建Entry失败: {info.path}");
                            failCount++;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"标记失败: {info.path}, 错误: {ex.Message}");
                        failCount++;
                    }
                }
                
                totalSuccess += successCount;
                totalFail += failCount;
            }
            
            // 保存设置（延迟保存，避免窗口刷新问题）
            EditorApplication.delayCall += () =>
            {
                try
                {
                    EditorUtility.SetDirty(settings);
                    AssetDatabase.SaveAssets();
                    
                    // 重新扫描以更新状态
                    ScanAllAssets();
                    
                    EditorUtility.DisplayDialog("完成",
                        $"批量标记完成！\n\n" +
                        $"成功: {totalSuccess}\n" +
                        $"失败: {totalFail}",
                        "确定");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"保存设置失败: {ex.Message}");
                    EditorUtility.DisplayDialog("警告",
                        $"批量标记完成，但保存时出现问题：\n{ex.Message}\n\n" +
                        $"成功: {totalSuccess}\n" +
                        $"失败: {totalFail}",
                        "确定");
                }
            };
        }
    }
}
