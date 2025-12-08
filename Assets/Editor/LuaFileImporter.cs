using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Lua文件导入器 - 确保.lua文件被正确识别为TextAsset
/// </summary>
public class LuaFileImporter : AssetPostprocessor
{
    /// <summary>
    /// 当资源导入时调用
    /// </summary>
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            // 检查是否是.lua文件
            if (assetPath.EndsWith(".lua"))
            {
                // 验证文件是否被正确识别为TextAsset
                TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                if (asset == null)
                {
                    // 如果未识别为TextAsset，尝试重新导入
                    Debug.LogWarning($"Lua文件未识别为TextAsset，尝试重新导入: {assetPath}");
                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                }
            }
        }
    }
}

