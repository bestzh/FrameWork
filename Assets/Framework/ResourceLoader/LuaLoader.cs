using UnityEngine;
using System.Collections;
using System.IO;
using XLua;

namespace Framework.ResourceLoader
{
    /// <summary>
    /// Lua脚本加载器 - 统一使用资源加载框架的格式
    /// </summary>
    public class LuaLoader : IResourceLoader
    {
        /// <summary>
        /// Lua脚本根目录（相对于Resources）
        /// </summary>
        private const string LuaScriptRoot = "lua/";
        
        /// <summary>
        /// 热更新Lua脚本根目录
        /// </summary>
        private string hotUpdateLuaRoot = "";
        
        /// <summary>
        /// 资源加载器（用于加载TextAsset）
        /// </summary>
        private IResourceLoader resourceLoader;
        
        public LuaLoader()
        {
            // 使用ResManager的资源加载器（ResManager在全局命名空间）
            resourceLoader = global::ResManager.GetResourceLoader();
            InitializeHotUpdatePath();
        }
        
        /// <summary>
        /// 初始化热更新路径
        /// </summary>
        private void InitializeHotUpdatePath()
        {
            if (string.IsNullOrEmpty(hotUpdateLuaRoot))
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                hotUpdateLuaRoot = Application.persistentDataPath + "/LuaHotUpdate/";
#elif UNITY_IOS && !UNITY_EDITOR
                hotUpdateLuaRoot = Application.persistentDataPath + "/LuaHotUpdate/";
#else
                hotUpdateLuaRoot = Application.persistentDataPath + "/LuaHotUpdate/";
                // 编辑器模式下也检查StreamingAssets
                if (!Directory.Exists(hotUpdateLuaRoot))
                {
                    string streamingPath = Application.streamingAssetsPath + "/LuaHotUpdate/";
                    if (Directory.Exists(streamingPath))
                    {
                        hotUpdateLuaRoot = streamingPath;
                    }
                }
#endif
            }
        }
        
        /// <summary>
        /// 同步加载Lua脚本
        /// </summary>
        public T Load<T>(string path) where T : Object
        {
            // 移除扩展名（统一格式）
            string resourcePath = global::ResManager.GetResourcesName(path);
            
            // 优先级1: 从热更新目录加载
            if (!string.IsNullOrEmpty(hotUpdateLuaRoot) && Directory.Exists(hotUpdateLuaRoot))
            {
                string relativePath = resourcePath.Replace("lua/", "").Replace('/', Path.DirectorySeparatorChar) + ".lua";
                string hotUpdatePath = Path.Combine(hotUpdateLuaRoot, relativePath);
                
                if (File.Exists(hotUpdatePath))
                {
                    // 读取文件并创建TextAsset
                    byte[] bytes = File.ReadAllBytes(hotUpdatePath);
                    TextAsset textAsset = new TextAsset(System.Text.Encoding.UTF8.GetString(bytes));
                    textAsset.name = Path.GetFileNameWithoutExtension(hotUpdatePath);
                    // 设置HideFlags，避免Unity编辑器尝试序列化运行时创建的对象
                    textAsset.hideFlags = HideFlags.DontSave;
                    return textAsset as T;
                }
            }
            
            // 优先级2: 从Resources目录加载（使用资源加载器）
            return resourceLoader.Load<T>(resourcePath);
        }
        
        /// <summary>
        /// 异步加载Lua脚本
        /// </summary>
        public IEnumerator LoadAsync<T>(string path, System.Action<T> onComplete) where T : Object
        {
            // 移除扩展名（统一格式）
            string resourcePath = global::ResManager.GetResourcesName(path);
            
            // 优先级1: 从热更新目录加载
            if (!string.IsNullOrEmpty(hotUpdateLuaRoot) && Directory.Exists(hotUpdateLuaRoot))
            {
                string relativePath = resourcePath.Replace("lua/", "").Replace('/', Path.DirectorySeparatorChar) + ".lua";
                string hotUpdatePath = Path.Combine(hotUpdateLuaRoot, relativePath);
                
                if (File.Exists(hotUpdatePath))
                {
                    // 异步读取文件
                    yield return null; // 模拟异步
                    byte[] bytes = File.ReadAllBytes(hotUpdatePath);
                    TextAsset textAsset = new TextAsset(System.Text.Encoding.UTF8.GetString(bytes));
                    textAsset.name = Path.GetFileNameWithoutExtension(hotUpdatePath);
                    // 设置HideFlags，避免Unity编辑器尝试序列化运行时创建的对象
                    textAsset.hideFlags = HideFlags.DontSave;
                    onComplete?.Invoke(textAsset as T);
                    yield break;
                }
            }
            
            // 优先级2: 从Resources目录加载（使用资源加载器）
            yield return resourceLoader.LoadAsync<T>(resourcePath, onComplete);
        }
        
        /// <summary>
        /// 卸载Lua脚本（Lua脚本通常不需要手动卸载）
        /// </summary>
        public void Unload(string path)
        {
            // Lua脚本由LuaEnv管理，不需要手动卸载
            resourceLoader?.Unload(path);
        }
        
        /// <summary>
        /// 卸载所有Lua脚本
        /// </summary>
        public void UnloadAll()
        {
            resourceLoader?.UnloadAll();
        }
        
        /// <summary>
        /// 获取Lua脚本的字节数组（用于XLua的CustomLoader）
        /// </summary>
        public byte[] LoadLuaBytes(string moduleName)
        {
            // 优先级1: 从热更新目录加载
            if (!string.IsNullOrEmpty(hotUpdateLuaRoot) && Directory.Exists(hotUpdateLuaRoot))
            {
                string relativePath = moduleName.Replace('.', Path.DirectorySeparatorChar) + ".lua";
                string hotUpdatePath = Path.Combine(hotUpdateLuaRoot, relativePath);
                
                if (File.Exists(hotUpdatePath))
                {
                    Debug.Log($"[LuaLoader] 从热更新目录加载: {hotUpdatePath}");
                    return File.ReadAllBytes(hotUpdatePath);
                }
            }
            
            // 优先级2: 从资源加载器加载（Addressables）
            // 尝试多种地址格式，因为Addressables的地址可能不同
            string[] addressFormats = {
                LuaScriptRoot + moduleName.Replace('.', '/'),  // lua/ui/LuaMain
                moduleName.Replace('.', '/'),                    // ui/LuaMain
                moduleName,                                      // ui.LuaMain
                LuaScriptRoot + moduleName.Replace('.', '/') + ".lua",  // lua/ui/LuaMain.lua
                moduleName.Replace('.', '/') + ".lua",          // ui/LuaMain.lua
            };
            
            foreach (string address in addressFormats)
            {
                TextAsset luaScript = resourceLoader.Load<TextAsset>(address);
                if (luaScript != null)
                {
                    Debug.Log($"[LuaLoader] 通过资源加载器加载成功: {address}");
                    return luaScript.bytes;
                }
            }
            
            // 所有加载方式都失败
            Debug.LogError($"[LuaLoader] 无法加载Lua模块: {moduleName}");
            Debug.LogError($"[LuaLoader] 已尝试的地址格式:");
            foreach (string address in addressFormats)
            {
                Debug.LogError($"  - {address}");
            }
            Debug.LogError($"[LuaLoader] 请检查：");
            Debug.LogError($"  1. Lua脚本是否已标记为Addressable");
            Debug.LogError($"  2. Addressable地址是否正确（应该与上述地址之一匹配）");
            Debug.LogError($"  3. Addressables资源是否已构建（Tools > Addressable > 资源构建工具）");
            Debug.LogError($"  4. Catalog文件是否存在（StreamingAssets/AA/[BuildTarget]/catalog_*.json）");
            Debug.LogError($"  5. 热更新目录是否存在: {hotUpdateLuaRoot}");
            return null;
        }
    }
}

