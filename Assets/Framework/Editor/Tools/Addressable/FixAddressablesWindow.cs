using UnityEngine;
using UnityEditor;
using System.IO;

namespace Tools.Addressable
{
    /// <summary>
    /// Addressables窗口修复工具
    /// 用于修复Addressables窗口的常见问题
    /// </summary>
    public class FixAddressablesWindow : EditorWindow
    {
        [MenuItem("Tools/Addressable/修复Addressables窗口")]
        public static void ShowWindow()
        {
            GetWindow<FixAddressablesWindow>("修复Addressables窗口");
        }
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Addressables窗口修复工具", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "如果Addressables Groups窗口打开时报错，可以使用以下工具修复：",
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            // 方案1：清理缓存
            EditorGUILayout.LabelField("方案1：清理缓存（推荐）", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "清理Addressables缓存文件，然后重新打开窗口。\n" +
                "这不会删除你的Addressables配置。",
                MessageType.None);
            
            if (GUILayout.Button("清理缓存并重新打开窗口", GUILayout.Height(30)))
            {
                CleanCache();
            }
            
            EditorGUILayout.Space();
            
            // 方案2：检查设置文件
            EditorGUILayout.LabelField("方案2：检查设置文件", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "检查Addressables设置文件是否存在且有效。",
                MessageType.None);
            
            if (GUILayout.Button("检查设置文件", GUILayout.Height(30)))
            {
                CheckSettings();
            }
            
            EditorGUILayout.Space();
            
            // 方案3：重新初始化
            EditorGUILayout.LabelField("方案3：重新初始化（谨慎使用）", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "⚠️ 警告：这会删除所有Addressables配置！\n" +
                "请确保已备份 AddressableAssetsData 文件夹。",
                MessageType.Warning);
            
            GUI.color = Color.red;
            if (GUILayout.Button("删除配置并重新初始化（危险）", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("确认删除",
                    "确定要删除所有Addressables配置吗？\n\n" +
                    "这将删除 AddressableAssetsData 文件夹，\n" +
                    "所有已标记的Addressable资源需要重新标记！\n\n" +
                    "建议先备份项目。",
                    "确定删除", "取消"))
                {
                    Reinitialize();
                }
            }
            GUI.color = Color.white;
        }
        
        /// <summary>
        /// 清理缓存
        /// </summary>
        private void CleanCache()
        {
            try
            {
                bool cleaned = false;
                
                // 清理Library/Addressables
                string addressablesCache = Path.Combine(Application.dataPath, "../Library/Addressables");
                if (Directory.Exists(addressablesCache))
                {
                    Directory.Delete(addressablesCache, true);
                    Debug.Log("已删除: Library/Addressables");
                    cleaned = true;
                }
                
                // 清理Library/com.unity.addressables
                string unityAddressablesCache = Path.Combine(Application.dataPath, "../Library/com.unity.addressables");
                if (Directory.Exists(unityAddressablesCache))
                {
                    Directory.Delete(unityAddressablesCache, true);
                    Debug.Log("已删除: Library/com.unity.addressables");
                    cleaned = true;
                }
                
                if (cleaned)
                {
                    AssetDatabase.Refresh();
                    EditorUtility.DisplayDialog("完成", 
                        "缓存已清理！\n\n" +
                        "请关闭并重新打开Addressables Groups窗口。\n" +
                        "如果问题仍然存在，请重启Unity Editor。",
                        "确定");
                    
                    // 尝试重新打开窗口
                    EditorApplication.delayCall += () =>
                    {
                        System.Type windowType = System.Type.GetType("UnityEditor.AddressableAssets.GUI.AddressableAssetsWindow, Unity.Addressables.Editor");
                        if (windowType != null)
                        {
                            EditorWindow.GetWindow(windowType);
                        }
                    };
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", 
                        "未找到缓存文件，可能已经清理过了。\n\n" +
                        "如果问题仍然存在，请尝试重启Unity Editor。",
                        "确定");
                }
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("错误", 
                    $"清理缓存失败：\n{ex.Message}\n\n" +
                    "请手动删除以下文件夹：\n" +
                    "- Library/Addressables\n" +
                    "- Library/com.unity.addressables",
                    "确定");
            }
        }
        
        /// <summary>
        /// 检查设置文件
        /// </summary>
        private void CheckSettings()
        {
            string settingsPath = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
            
            if (File.Exists(settingsPath))
            {
                var settings = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(settingsPath);
                if (settings != null)
                {
                    EditorUtility.DisplayDialog("检查结果", 
                        "✓ Addressables设置文件存在且有效\n\n" +
                        "如果窗口仍然报错，请尝试：\n" +
                        "1. 清理缓存\n" +
                        "2. 重启Unity Editor",
                        "确定");
                }
                else
                {
                    EditorUtility.DisplayDialog("检查结果", 
                        "⚠️ 设置文件存在但无法加载\n\n" +
                        "建议重新初始化Addressables。",
                        "确定");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("检查结果", 
                    "✗ Addressables设置文件不存在\n\n" +
                    "需要初始化Addressables：\n" +
                    "Window > Asset Management > Addressables > Groups\n" +
                    "然后点击 'Create Addressables Settings'",
                    "确定");
            }
        }
        
        /// <summary>
        /// 重新初始化
        /// </summary>
        private void Reinitialize()
        {
            try
            {
                // 先关闭所有Addressables窗口
                CloseAllAddressablesWindows();
                
                string addressableDataPath = "Assets/AddressableAssetsData";
                
                if (Directory.Exists(addressableDataPath))
                {
                    // 备份（可选）
                    string backupPath = addressableDataPath + "_backup_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    FileUtil.CopyFileOrDirectory(addressableDataPath, backupPath);
                    Debug.Log($"已备份到: {backupPath}");
                    
                    // 删除
                    Directory.Delete(addressableDataPath, true);
                    if (File.Exists(addressableDataPath + ".meta"))
                    {
                        File.Delete(addressableDataPath + ".meta");
                    }
                    
                    AssetDatabase.Refresh();
                    
                    // 延迟初始化
                    EditorApplication.delayCall += () =>
                    {
                        // 等待一帧后再初始化
                        EditorApplication.delayCall += () =>
                        {
                            InitializeAddressablesSettings();
                        };
                    };
                    
                    EditorUtility.DisplayDialog("完成", 
                        "Addressables配置已删除！\n\n" +
                        "备份位置：\n" + backupPath + "\n\n" +
                        "正在自动初始化Addressables...\n" +
                        "如果自动初始化失败，请手动：\n" +
                        "1. 关闭并重新打开Unity Editor\n" +
                        "2. 打开 Window > Asset Management > Addressables > Groups\n" +
                        "3. 点击 'Create Addressables Settings'",
                        "确定");
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", 
                        "Addressables配置不存在，无需删除。\n\n" +
                        "正在尝试初始化Addressables...",
                        "确定");
                    
                    EditorApplication.delayCall += () =>
                    {
                        InitializeAddressablesSettings();
                    };
                }
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("错误", 
                    $"删除配置失败：\n{ex.Message}\n\n" +
                    "建议：\n" +
                    "1. 关闭Unity Editor\n" +
                    "2. 手动删除 Assets/AddressableAssetsData 文件夹\n" +
                    "3. 重新打开Unity Editor\n" +
                    "4. 手动初始化Addressables",
                    "确定");
            }
        }
        
        /// <summary>
        /// 关闭所有Addressables窗口
        /// </summary>
        private void CloseAllAddressablesWindows()
        {
            try
            {
                System.Type windowType = System.Type.GetType("UnityEditor.AddressableAssets.GUI.AddressableAssetsWindow, Unity.Addressables.Editor");
                if (windowType != null)
                {
                    var windows = Resources.FindObjectsOfTypeAll(windowType);
                    foreach (var window in windows)
                    {
                        if (window is EditorWindow editorWindow)
                        {
                            editorWindow.Close();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"关闭Addressables窗口失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 初始化Addressables设置
        /// </summary>
        private void InitializeAddressablesSettings()
        {
            try
            {
                // 使用反射调用Addressables的初始化方法
                System.Type settingsType = System.Type.GetType("UnityEditor.AddressableAssets.Settings.AddressableAssetSettingsDefaultObject, Unity.Addressables.Editor");
                if (settingsType != null)
                {
                    var method = settingsType.GetMethod("GetSettings", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                    if (method != null)
                    {
                        var settings = method.Invoke(null, new object[] { true });
                        if (settings != null)
                        {
                            Debug.Log("Addressables设置已自动创建");
                            AssetDatabase.Refresh();
                            return;
                        }
                    }
                }
                
                Debug.LogWarning("无法自动初始化Addressables，请手动初始化");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"自动初始化Addressables失败: {ex.Message}");
            }
        }
    }
}

