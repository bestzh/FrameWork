using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 强制重新导入Lua文件工具
/// 如果Lua文件无法被正确加载，可以使用此工具强制重新导入
/// </summary>
public class ForceReimportLuaFiles : EditorWindow
{
    [MenuItem("Tools/XLua/重新导入所有Lua文件")]
    static void ReimportAllLuaFiles()
    {
        string luaFolder = "Assets/Resources/lua";
        
        if (!Directory.Exists(luaFolder))
        {
            Debug.LogWarning($"Lua文件夹不存在: {luaFolder}");
            return;
        }
        
        // 查找所有.lua文件
        string[] luaFiles = Directory.GetFiles(luaFolder, "*.lua", SearchOption.AllDirectories);
        
        int count = 0;
        foreach (string file in luaFiles)
        {
            string relativePath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
            
            // 重新导入文件
            AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);
            count++;
        }
        
        Debug.Log($"已重新导入 {count} 个Lua文件");
        
        // 刷新资源数据库
        AssetDatabase.Refresh();
    }
    
    [MenuItem("Tools/XLua/检查Lua文件")]
    static void CheckLuaFiles()
    {
        string luaFolder = "Assets/Resources/lua";
        
        if (!Directory.Exists(luaFolder))
        {
            Debug.LogWarning($"Lua文件夹不存在: {luaFolder}");
            return;
        }
        
        // 查找所有.lua和.lua.txt文件
        string[] luaFiles = Directory.GetFiles(luaFolder, "*.lua", SearchOption.AllDirectories);
        string[] luaTxtFiles = Directory.GetFiles(luaFolder, "*.lua.txt", SearchOption.AllDirectories);
        
        Debug.Log($"找到 {luaFiles.Length} 个.lua文件, {luaTxtFiles.Length} 个.lua.txt文件:");
        
        foreach (string file in luaFiles)
        {
            string relativePath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(relativePath);
            
            if (asset != null)
            {
                Debug.Log($"✓ {relativePath} - 已识别为TextAsset ({asset.bytes.Length} bytes)");
            }
            else
            {
                Debug.LogWarning($"✗ {relativePath} - 未识别为TextAsset！建议重命名为.lua.txt");
            }
        }
        
        foreach (string file in luaTxtFiles)
        {
            string relativePath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(relativePath);
            
            if (asset != null)
            {
                Debug.Log($"✓ {relativePath} - 已识别为TextAsset ({asset.bytes.Length} bytes)");
            }
            else
            {
                Debug.LogWarning($"✗ {relativePath} - 未识别为TextAsset！");
            }
        }
    }
    
    [MenuItem("Tools/XLua/批量重命名Lua文件为.lua.txt")]
    static void RenameLuaFilesToLuaTxt()
    {
        string luaFolder = "Assets/Resources/lua";
        
        if (!Directory.Exists(luaFolder))
        {
            Debug.LogWarning($"Lua文件夹不存在: {luaFolder}");
            return;
        }
        
        // 查找所有.lua文件（排除已经是.lua.txt的文件）
        string[] luaFiles = Directory.GetFiles(luaFolder, "*.lua", SearchOption.AllDirectories);
        
        // 过滤掉已经是.lua.txt的文件
        System.Collections.Generic.List<string> filesToRename = new System.Collections.Generic.List<string>();
        foreach (string file in luaFiles)
        {
            if (!file.EndsWith(".lua.txt"))
            {
                filesToRename.Add(file);
            }
        }
        
        if (filesToRename.Count == 0)
        {
            Debug.Log("没有需要重命名的.lua文件（所有文件都已经是.lua.txt格式）");
            return;
        }
        
        // 确认操作
        if (!EditorUtility.DisplayDialog("确认重命名", 
            $"准备将 {filesToRename.Count} 个.lua文件重命名为.lua.txt\n\n" +
            "这将使Unity能够正确识别这些文件为TextAsset。\n\n" +
            "注意：重命名后，Lua加载路径会自动适配。\n\n" +
            "是否继续？", 
            "确定", "取消"))
        {
            return;
        }
        
        int successCount = 0;
        int failCount = 0;
        
        foreach (string file in filesToRename)
        {
            try
            {
                string relativePath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
                string newRelativePath = relativePath + ".txt";
                
                // 检查目标文件是否已存在
                if (File.Exists(newRelativePath.Replace("Assets/", Application.dataPath + "/")))
                {
                    Debug.LogWarning($"目标文件已存在，跳过: {newRelativePath}");
                    failCount++;
                    continue;
                }
                
                // 使用AssetDatabase移动/重命名文件
                string error = AssetDatabase.MoveAsset(relativePath, newRelativePath);
                if (string.IsNullOrEmpty(error))
                {
                    successCount++;
                    Debug.Log($"✓ 重命名成功: {relativePath} -> {newRelativePath}");
                }
                else
                {
                    Debug.LogError($"✗ 重命名失败: {relativePath} - {error}");
                    failCount++;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"✗ 重命名失败: {file} - {ex.Message}");
                failCount++;
            }
        }
        
        // 刷新资源数据库
        AssetDatabase.Refresh();
        
        Debug.Log($"重命名完成: 成功 {successCount} 个, 失败 {failCount} 个");
        
        if (successCount > 0)
        {
            EditorUtility.DisplayDialog("重命名完成", 
                $"已成功重命名 {successCount} 个文件\n\n" +
                $"失败 {failCount} 个文件\n\n" +
                "现在这些文件应该能被Unity正确识别为TextAsset了。", 
                "确定");
        }
    }
}

