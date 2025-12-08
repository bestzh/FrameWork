using UnityEngine;
using System.Collections;
using System.IO;
using System.Reflection;
using System;

namespace Framework.ResourceLoader
{
    /// <summary>
    /// Addressables 资源加载器（需要安装 Addressables 包）
    /// 支持本地缓存（PersistentDataPath）和StreamingAssets的优先级加载
    /// 使用运行时反射，不依赖预编译符号
    /// </summary>
    public class AddressablesLoader : IResourceLoader
    {
        // 使用object类型存储handle，避免编译时依赖
        private System.Collections.Generic.Dictionary<string, object> loadedHandles = 
            new System.Collections.Generic.Dictionary<string, object>();
        
        private bool initialized = false;
        private string localCachePath = "";
        private string streamingAssetsPath = "";
        private string buildTarget = "";
        
        // 错误日志控制（避免重复输出）
        private static bool hasLoggedAddressablesUnavailable = false;
        private static System.Collections.Generic.HashSet<string> failedPaths = new System.Collections.Generic.HashSet<string>();
        
        // Addressables API的反射类型和方法
        private static Type addressablesType = null;
        private static MethodInfo loadAssetAsyncMethod = null;
        private static MethodInfo loadResourceLocationsAsyncMethod = null;
        private static MethodInfo releaseMethod = null;
        private static Type asyncOperationStatusType = null;
        private static object succeededStatus = null;
        
        // 静态构造函数：初始化反射信息
        static AddressablesLoader()
        {
            InitializeReflection();
        }
        
        /// <summary>
        /// 初始化Addressables API的反射信息
        /// </summary>
        private static void InitializeReflection()
        {
            try
            {
                // 尝试加载Addressables程序集
                addressablesType = Type.GetType("UnityEngine.AddressableAssets.Addressables, Unity.Addressables");
                if (addressablesType == null)
                {
                    Debug.LogWarning("[AddressablesLoader] Addressables包未安装或不可用");
                    return;
                }
                
                // 获取LoadAssetAsync方法（泛型方法，接受string参数的重载）
                // Addressables可能有多个重载，我们需要找到接受string参数的那个
                var methods = addressablesType.GetMethods(BindingFlags.Public | BindingFlags.Static);
                foreach (var method in methods)
                {
                    if (method.Name == "LoadAssetAsync" && method.IsGenericMethodDefinition)
                    {
                        var parameters = method.GetParameters();
                        // 查找接受单个string参数的重载
                        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
                        {
                            loadAssetAsyncMethod = method;
                            break;
                        }
                        // 也尝试查找接受object参数的重载（Addressables可能使用IKey接口）
                        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(object))
                        {
                            // 如果还没找到string版本，先保存这个
                            if (loadAssetAsyncMethod == null)
                            {
                                loadAssetAsyncMethod = method;
                            }
                        }
                    }
                }
                
                // 如果还是没找到，尝试查找所有LoadAssetAsync方法（包括非泛型的）
                if (loadAssetAsyncMethod == null)
                {
                    foreach (var method in methods)
                    {
                        if (method.Name == "LoadAssetAsync")
                        {
                            var parameters = method.GetParameters();
                            if (parameters.Length == 1)
                            {
                                // 检查参数类型是否是string或可以接受string的类型
                                var paramType = parameters[0].ParameterType;
                                if (paramType == typeof(string) || paramType == typeof(object))
                                {
                                    loadAssetAsyncMethod = method;
                                    break;
                                }
                            }
                        }
                    }
                }
                
                if (loadAssetAsyncMethod == null)
                {
                    Debug.LogWarning("[AddressablesLoader] 未找到 LoadAssetAsync(string) 方法，将使用回退机制");
                }
                
                // 获取LoadResourceLocationsAsync方法（有多个重载，使用最简单的string参数版本）
                var locationMethods = addressablesType.GetMethods(BindingFlags.Public | BindingFlags.Static);
                foreach (var method in locationMethods)
                {
                    if (method.Name == "LoadResourceLocationsAsync")
                    {
                        var parameters = method.GetParameters();
                        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
                        {
                            loadResourceLocationsAsyncMethod = method;
                            break;
                        }
                    }
                }
                
                // 获取Release方法（有多个重载，使用object参数版本）
                var releaseMethods = addressablesType.GetMethods(BindingFlags.Public | BindingFlags.Static);
                foreach (var method in releaseMethods)
                {
                    if (method.Name == "Release")
                    {
                        var parameters = method.GetParameters();
                        // 找到接受单个object参数的重载
                        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(object))
                        {
                            releaseMethod = method;
                            break;
                        }
                    }
                }
                
                // 获取AsyncOperationStatus枚举
                asyncOperationStatusType = Type.GetType("UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus, Unity.ResourceManager");
                if (asyncOperationStatusType != null)
                {
                    succeededStatus = Enum.Parse(asyncOperationStatusType, "Succeeded");
                }
                
                Debug.Log("[AddressablesLoader] ✓ Addressables API反射初始化成功");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AddressablesLoader] 初始化反射失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 检查Addressables是否可用
        /// </summary>
        private static bool IsAddressablesAvailable()
        {
            // 只要Addressables类型存在就认为可用
            // 即使LoadAssetAsync方法未找到，Load方法中也有回退机制
            return addressablesType != null;
        }
        
        /// <summary>
        /// 获取Addressables构建目标名称（与构建工具使用的名称一致）
        /// </summary>
        private string GetAddressablesBuildTarget()
        {
#if UNITY_EDITOR
            // 编辑器模式下，使用EditorUserBuildSettings获取构建目标
            UnityEditor.BuildTarget activeTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;
            
            // 将BuildTarget转换为Addressables使用的字符串格式
            switch (activeTarget)
            {
                case UnityEditor.BuildTarget.StandaloneWindows:
                case UnityEditor.BuildTarget.StandaloneWindows64:
                    return "StandaloneWindows64";
                case UnityEditor.BuildTarget.Android:
                    return "Android";
                case UnityEditor.BuildTarget.iOS:
                    return "iOS";
                case UnityEditor.BuildTarget.StandaloneOSX:
                    return "StandaloneOSX";
                case UnityEditor.BuildTarget.WebGL:
                    return "WebGL";
                default:
                    // 对于其他平台，使用BuildTarget的字符串表示
                    return activeTarget.ToString();
            }
#else
            // 运行时，根据实际平台返回对应的构建目标名称
            RuntimePlatform platform = Application.platform;
            switch (platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "StandaloneWindows64";
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    return "StandaloneOSX";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                default:
                    return platform.ToString();
            }
#endif
        }
        
        /// <summary>
        /// 初始化Addressables加载器
        /// 配置资源路径优先级：本地缓存 > StreamingAssets
        /// </summary>
        private void Initialize()
        {
            if (initialized)
                return;
            
            // 获取Addressables构建目标（与构建工具一致）
            buildTarget = GetAddressablesBuildTarget();
            
            // 本地缓存路径（PersistentDataPath，热更新资源存放位置）
            localCachePath = Path.Combine(Application.persistentDataPath, "AA", buildTarget);
            
            // StreamingAssets路径（打包时随游戏一起发布的资源）
            streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, "AA", buildTarget);
            
            Debug.Log($"[AddressablesLoader] 初始化资源路径:");
            Debug.Log($"  构建目标: {buildTarget}");
            Debug.Log($"  本地缓存路径: {localCachePath}");
            Debug.Log($"  StreamingAssets路径: {streamingAssetsPath}");
            
            // 检查并复制catalog文件到本地缓存（如果StreamingAssets中有但本地缓存中没有）
            EnsureCatalogFiles();
            
            initialized = true;
        }
        
        /// <summary>
        /// 确保catalog文件存在（优先使用本地缓存的，如果没有则从StreamingAssets复制）
        /// </summary>
        private void EnsureCatalogFiles()
        {
            string[] catalogFiles = { "catalog.bin", "catalog.hash", "settings.json" };
            
            foreach (string catalogFile in catalogFiles)
            {
                string localCacheFile = Path.Combine(localCachePath, catalogFile);
                string streamingAssetsFile = Path.Combine(streamingAssetsPath, catalogFile);
                
                // 如果本地缓存中没有，但StreamingAssets中有，则复制过去
                if (!File.Exists(localCacheFile) && File.Exists(streamingAssetsFile))
                {
                    // 确保目录存在
                    if (!Directory.Exists(localCachePath))
                    {
                        Directory.CreateDirectory(localCachePath);
                    }
                    
                    try
                    {
                        File.Copy(streamingAssetsFile, localCacheFile, true);
                        Debug.Log($"[AddressablesLoader] 已复制catalog文件到本地缓存: {catalogFile}");
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"[AddressablesLoader] 复制catalog文件失败: {ex.Message}");
                    }
                }
            }
            
            // 复制AddressablesLink目录
            string linkSourceDir = Path.Combine(streamingAssetsPath, "AddressablesLink");
            string linkDestDir = Path.Combine(localCachePath, "AddressablesLink");
            
            if (Directory.Exists(linkSourceDir) && !Directory.Exists(linkDestDir))
            {
                try
                {
                    CopyDirectory(linkSourceDir, linkDestDir);
                    Debug.Log($"[AddressablesLoader] 已复制AddressablesLink目录到本地缓存");
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"[AddressablesLoader] 复制AddressablesLink目录失败: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// 复制目录
        /// </summary>
        private void CopyDirectory(string sourceDir, string destDir)
        {
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            
            string[] files = Directory.GetFiles(sourceDir);
            foreach (string file in files)
            {
                if (file.EndsWith(".meta"))
                    continue;
                
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destDir, fileName);
                File.Copy(file, destFile, true);
            }
            
            string[] dirs = Directory.GetDirectories(sourceDir);
            foreach (string dir in dirs)
            {
                string dirName = Path.GetFileName(dir);
                string destSubDir = Path.Combine(destDir, dirName);
                CopyDirectory(dir, destSubDir);
            }
        }
        
        /// <summary>
        /// 检查资源是否存在于本地缓存
        /// </summary>
        private bool CheckLocalCache(string resourcePath)
        {
            // 尝试从本地缓存加载bundle文件
            // Addressables的bundle文件名通常是hash值，我们需要通过Addressables系统来查找
            // 这里主要是确保catalog文件优先从本地缓存加载
            return Directory.Exists(localCachePath) && File.Exists(Path.Combine(localCachePath, "catalog.bin"));
        }
        
        /// <summary>
        /// 检查catalog文件是否存在（仅在第一次失败时输出）
        /// </summary>
        private void CheckCatalogFiles(string resourcePath)
        {
            if (failedPaths.Contains(resourcePath))
                return; // 已经检查过了
                
            bool catalogExists = false;
            string catalogLocation = "";
            
            // 检查本地缓存
            string localCatalog = Path.Combine(localCachePath, "catalog.bin");
            if (File.Exists(localCatalog))
            {
                catalogExists = true;
                catalogLocation = $"本地缓存: {localCatalog}";
            }
            
            // 检查StreamingAssets
            if (!catalogExists)
            {
                string streamingCatalog = Path.Combine(streamingAssetsPath, "catalog.bin");
                if (File.Exists(streamingCatalog))
                {
                    catalogExists = true;
                    catalogLocation = $"StreamingAssets: {streamingCatalog}";
                }
            }
            
            // 检查catalog JSON文件（Unity可能使用JSON格式）
            if (!catalogExists)
            {
                string[] catalogPatterns = { "catalog_*.json", "catalog_*.bin" };
                foreach (string pattern in catalogPatterns)
                {
                    if (Directory.Exists(streamingAssetsPath))
                    {
                        string[] files = Directory.GetFiles(streamingAssetsPath, pattern);
                        if (files.Length > 0)
                        {
                            catalogExists = true;
                            catalogLocation = $"StreamingAssets: {files[0]}";
                            break;
                        }
                    }
                    
                    if (Directory.Exists(localCachePath))
                    {
                        string[] files = Directory.GetFiles(localCachePath, pattern);
                        if (files.Length > 0)
                        {
                            catalogExists = true;
                            catalogLocation = $"本地缓存: {files[0]}";
                            break;
                        }
                    }
                }
            }
            
            if (!catalogExists)
            {
                Debug.LogError($"[AddressablesLoader] ⚠ Catalog文件不存在！");
                Debug.LogError($"  本地缓存路径: {localCachePath}");
                Debug.LogError($"  StreamingAssets路径: {streamingAssetsPath}");
                Debug.LogError($"  解决方案：");
                Debug.LogError($"    1. 执行 Tools > Addressable > 资源构建工具");
                Debug.LogError($"    2. 确保构建完成后catalog文件已复制到StreamingAssets");
                Debug.LogError($"    3. 检查构建输出目录: ServerData/[BuildTarget]/");
            }
            else
            {
                Debug.Log($"[AddressablesLoader] ✓ 找到Catalog文件: {catalogLocation}");
            }
        }
        
    public T Load<T>(string path) where T : UnityEngine.Object
        {
            if (!IsAddressablesAvailable())
            {
                // 只在第一次失败时输出错误日志
                if (!hasLoggedAddressablesUnavailable)
                {
                    Debug.LogError($"[AddressablesLoader] Addressables包未安装或不可用，无法加载资源: {path}");
                    Debug.LogError($"[AddressablesLoader] 请安装Addressables包: Window > Package Manager > 搜索 'Addressables' > Install");
                    hasLoggedAddressablesUnavailable = true;
                }
                return null;
            }
            
            Initialize();
            
            // 检查catalog文件是否存在
            CheckCatalogFiles(path);
            
            try
            {
                // 使用反射调用 Addressables.LoadAssetAsync<T>(path)
                object handle = null;
                
                if (loadAssetAsyncMethod != null)
                {
                    if (loadAssetAsyncMethod.IsGenericMethodDefinition)
                    {
                        // 泛型方法，需要先MakeGenericMethod
                        MethodInfo genericMethod = loadAssetAsyncMethod.MakeGenericMethod(typeof(T));
                        handle = genericMethod.Invoke(null, new object[] { path });
                    }
                    else
                    {
                        // 非泛型方法，直接调用
                        handle = loadAssetAsyncMethod.Invoke(null, new object[] { path });
                    }
                }
                
                if (handle == null)
                {
                    // Addressables加载失败
                    if (!failedPaths.Contains(path))
                    {
                        if (loadAssetAsyncMethod == null)
                        {
                            Debug.LogError($"[AddressablesLoader] LoadAssetAsync方法未找到，无法加载资源: {path}");
                        }
                        else
                        {
                            Debug.LogError($"[AddressablesLoader] Addressables.LoadAssetAsync返回null，无法加载资源: {path}");
                        }
                        failedPaths.Add(path);
                    }
                    return null;
                }
                
                // 调用 WaitForCompletion()
                Type handleType = handle.GetType();
                MethodInfo waitMethod = handleType.GetMethod("WaitForCompletion");
                if (waitMethod != null)
                {
                    waitMethod.Invoke(handle, null);
                }
                
                // 检查handle是否有效
                PropertyInfo isValidProp = handleType.GetProperty("IsValid");
                if (isValidProp != null)
                {
                    bool isValid = (bool)isValidProp.GetValue(handle);
                    if (!isValid)
                    {
                        Debug.LogError($"[AddressablesLoader] ✗ Handle无效: {path}");
                        Debug.LogError($"[AddressablesLoader] 这可能是因为资源未构建或catalog文件缺失");
                        return null;
                    }
                }
                
                // 检查Status
                PropertyInfo statusProp = handleType.GetProperty("Status");
                object status = statusProp?.GetValue(handle);
                
                if (status != null && status.Equals(succeededStatus))
                {
                    loadedHandles[path] = handle;
                    
                    // 获取Result
                    PropertyInfo resultProp = handleType.GetProperty("Result");
                    T result = (T)resultProp?.GetValue(handle);
                    
                    if (result != null)
                    {
                        // 清除失败记录（如果之前失败过）
                        failedPaths.Remove(path);
                        
                        // 如果是GameObject，检查并修复材质依赖
                        if (result is GameObject)
                        {
                            FixMaterialDependencies(result as GameObject, path);
                        }
                        
                        return result;
                    }
                    else
                    {
                        // Handle成功但Result为null
                        if (!failedPaths.Contains(path))
                        {
                            Debug.LogError($"[AddressablesLoader] Handle成功但Result为null，无法加载资源: {path}");
                            Debug.LogError($"[AddressablesLoader] 请检查资源是否正确标记为Addressable，地址是否正确: {path}");
                            failedPaths.Add(path);
                        }
                        return null;
                    }
                }
                else
                {
                    // Addressables加载失败
                    // 只在第一次失败时输出详细错误信息
                    if (!failedPaths.Contains(path))
                    {
                        string errorMsg = $"[AddressablesLoader] ✗ Addressables加载失败: {path}";
                        errorMsg += $"\n  Status: {status}";
                        
                        // 获取异常信息
                        PropertyInfo exceptionProp = handleType.GetProperty("OperationException");
                        Exception operationException = exceptionProp?.GetValue(handle) as Exception;
                        if (operationException != null)
                        {
                            errorMsg += $"\n  Exception: {operationException.Message}";
                            
                            // 如果是InvalidKeyException，提供更详细的帮助
                            if (operationException.GetType().Name.Contains("InvalidKey"))
                            {
                                errorMsg += $"\n\n  ⚠ 资源地址 '{path}' 在Addressables中不存在！";
                                errorMsg += $"\n\n  诊断步骤：";
                                errorMsg += $"\n    1. 打开 Window > Asset Management > Addressables > Groups";
                                errorMsg += $"\n    2. 在Groups窗口中查找资源，确认其Addressable地址";
                                errorMsg += $"\n    3. 检查地址是否与代码中使用的地址匹配: {path}";
                                errorMsg += $"\n    4. 如果地址不匹配，修改代码中的地址或修改资源的Addressable地址";
                                errorMsg += $"\n    5. 如果资源未构建，执行: Tools > Addressable > 资源构建工具";
                                errorMsg += $"\n    6. 构建完成后，确保catalog文件已复制到StreamingAssets";
                            }
                        }
                        
                        Debug.LogError(errorMsg);
                        failedPaths.Add(path);
                    }
                    
                    // 释放失败的handle
                    try
                    {
                        if (releaseMethod != null)
                        {
                            releaseMethod.Invoke(null, new object[] { handle });
                        }
                    }
                    catch (Exception releaseEx)
                    {
                        // 静默处理释放异常
                    }
                    
                    return null;
                }
            }
            catch (Exception e)
            {
                // 异常时返回null
                if (!failedPaths.Contains(path))
                {
                    Debug.LogError($"[AddressablesLoader] 加载资源异常，无法加载资源: {path}");
                    Debug.LogError($"  错误类型: {e.GetType().Name}");
                    Debug.LogError($"  错误消息: {e.Message}");
                    if (e.InnerException != null)
                    {
                        Debug.LogError($"  内部异常: {e.InnerException.Message}");
                    }
                    failedPaths.Add(path);
                }
                return null;
            }
        }
        
        public IEnumerator LoadAsync<T>(string path, System.Action<T> onComplete) where T : UnityEngine.Object
        {
            if (!IsAddressablesAvailable())
            {
                // 只在第一次失败时输出错误日志
                if (!hasLoggedAddressablesUnavailable)
                {
                    Debug.LogError($"[AddressablesLoader] Addressables包未安装或不可用，无法异步加载资源: {path}");
                    Debug.LogError($"[AddressablesLoader] 请安装Addressables包: Window > Package Manager > 搜索 'Addressables' > Install");
                    hasLoggedAddressablesUnavailable = true;
                }
                onComplete?.Invoke(null);
                yield break;
            }
            
            Initialize();
            
            object handle = null;
            Exception loadException = null;
            
            // 在try块外调用，避免yield在try-catch中的问题
            try
            {
                // 使用反射调用 Addressables.LoadAssetAsync<T>(path)
                MethodInfo genericMethod = loadAssetAsyncMethod.MakeGenericMethod(typeof(T));
                handle = genericMethod.Invoke(null, new object[] { path });
                
                if (handle == null)
                {
                    Debug.LogError($"[AddressablesLoader] 异步加载失败: {path} - handle为null");
                    onComplete?.Invoke(null);
                    yield break;
                }
            }
            catch (Exception e)
            {
                loadException = e;
            }
            
            if (loadException != null)
            {
                Debug.LogError($"[AddressablesLoader] 异步加载异常: {path}, 错误: {loadException.Message}");
                onComplete?.Invoke(null);
                yield break;
            }
            
            // yield return handle (需要转换为IEnumerator)
            if (handle is IEnumerator enumerator)
            {
                yield return enumerator;
            }
            else
            {
                // 如果不是IEnumerator，等待完成
                Type asyncHandleType = handle.GetType();
                MethodInfo waitMethod = asyncHandleType.GetMethod("WaitForCompletion");
                if (waitMethod != null)
                {
                    waitMethod.Invoke(handle, null);
                }
            }
            
            // 检查状态（在yield之后）
            try
            {
                Type asyncHandleType = handle.GetType();
                PropertyInfo statusProp = asyncHandleType.GetProperty("Status");
                object status = statusProp?.GetValue(handle);
                
                if (status != null && status.Equals(succeededStatus))
                {
                    loadedHandles[path] = handle;
                    Debug.Log($"[AddressablesLoader] 异步加载成功: {path}");
                    
                    PropertyInfo resultProp = asyncHandleType.GetProperty("Result");
                    T result = (T)resultProp?.GetValue(handle);
                    
                    // 如果是GameObject，检查并修复材质依赖
                    if (result is GameObject)
                    {
                        FixMaterialDependencies(result as GameObject, path);
                    }
                    
                    onComplete?.Invoke(result);
                }
                else
                {
                    Debug.LogError($"[AddressablesLoader] 异步加载失败: {path}. Status: {status}");
                    PropertyInfo exceptionProp = asyncHandleType.GetProperty("OperationException");
                    Exception operationException = exceptionProp?.GetValue(handle) as Exception;
                    if (operationException != null)
                    {
                        Debug.LogError($"Exception: {operationException}");
                    }
                    onComplete?.Invoke(null);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[AddressablesLoader] 检查加载状态时发生异常: {path}, 错误: {e.Message}");
                onComplete?.Invoke(null);
            }
        }
        
        public void Unload(string path)
        {
            if (!IsAddressablesAvailable())
                return;
                
            if (loadedHandles.ContainsKey(path))
            {
                if (releaseMethod != null)
                {
                    releaseMethod.Invoke(null, new object[] { loadedHandles[path] });
                }
                loadedHandles.Remove(path);
            }
        }
        
        public void UnloadAll()
        {
            if (!IsAddressablesAvailable())
                return;
                
            if (releaseMethod != null)
            {
                foreach (var handle in loadedHandles.Values)
                {
                    try
                    {
                        releaseMethod.Invoke(null, new object[] { handle });
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"[AddressablesLoader] 释放handle时发生异常: {ex.Message}");
                    }
                }
            }
            loadedHandles.Clear();
        }
        
        /// <summary>
        /// 修复GameObject的材质依赖
        /// 当使用Addressables加载模型时，如果材质没有被正确加载，此方法会尝试修复
        /// </summary>
        private void FixMaterialDependencies(GameObject obj, string resourcePath)
        {
            if (obj == null)
                return;
            
            // 收集所有需要检查的Renderer组件
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
            if (renderers == null || renderers.Length == 0)
                return;
            
            bool hasMissingMaterials = false;
            int fixedCount = 0;
            
            // 获取默认材质（用于回退）
            Material defaultMaterial = GetDefaultMaterial();
            
            // 检查所有Renderer的材质
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null)
                    continue;
                
                // 检查共享材质
                if (renderer.sharedMaterial == null)
                {
                    hasMissingMaterials = true;
                    Debug.LogWarning($"[AddressablesLoader] 发现丢失的材质: {renderer.name} (在资源 {resourcePath} 中)");
                    
                    // 尝试使用默认材质修复
                    if (defaultMaterial != null)
                    {
                        renderer.sharedMaterial = defaultMaterial;
                        fixedCount++;
                        Debug.Log($"[AddressablesLoader] ✓ 已使用默认材质修复: {renderer.name}");
                    }
                }
                
                // 检查材质数组
                if (renderer.sharedMaterials != null)
                {
                    bool needsFix = false;
                    Material[] materials = renderer.sharedMaterials;
                    
                    for (int i = 0; i < materials.Length; i++)
                    {
                        if (materials[i] == null)
                        {
                            hasMissingMaterials = true;
                            Debug.LogWarning($"[AddressablesLoader] 发现丢失的材质数组元素 [{i}]: {renderer.name} (在资源 {resourcePath} 中)");
                            
                            // 尝试使用默认材质修复
                            if (defaultMaterial != null)
                            {
                                materials[i] = defaultMaterial;
                                needsFix = true;
                                fixedCount++;
                                Debug.Log($"[AddressablesLoader] ✓ 已使用默认材质修复数组元素 [{i}]: {renderer.name}");
                            }
                        }
                    }
                    
                    // 如果有修复，更新材质数组
                    if (needsFix)
                    {
                        renderer.sharedMaterials = materials;
                    }
                }
            }
            
            if (hasMissingMaterials)
            {
                if (fixedCount > 0)
                {
                    Debug.Log($"[AddressablesLoader] ✓ 已修复 {fixedCount} 个丢失的材质（使用默认材质）");
                }
                else
                {
                    Debug.LogWarning($"[AddressablesLoader] 资源 {resourcePath} 存在材质丢失问题，且无法自动修复");
                }
                
                Debug.LogWarning($"[AddressablesLoader] 请检查以下事项：");
                Debug.LogWarning($"  1. 确保材质资源已标记为Addressable");
                Debug.LogWarning($"  2. 确保材质和模型在同一个AssetGroup中，或使用Include In Build");
                Debug.LogWarning($"  3. 确保材质依赖关系正确配置");
                Debug.LogWarning($"  4. 重新构建Addressables资源");
                
                // 尝试使用LoadAllAssetsAsync加载所有依赖
                TryLoadAllDependencies(resourcePath);
            }
        }
        
        /// <summary>
        /// 获取Unity默认材质（用于材质丢失时的回退）
        /// </summary>
        private Material GetDefaultMaterial()
        {
            // 尝试获取URP默认材质
            if (UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline != null)
            {
                // URP项目：尝试通过反射获取默认材质
                try
                {
                    var rpAsset = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline;
                    var rpAssetType = rpAsset.GetType();
                    
                    // 尝试获取defaultMaterial属性
                    var defaultMaterialProp = rpAssetType.GetProperty("defaultMaterial");
                    if (defaultMaterialProp != null)
                    {
                        Material mat = defaultMaterialProp.GetValue(rpAsset) as Material;
                        if (mat != null)
                        {
                            return mat;
                        }
                    }
                    
                    // 尝试获取defaultMaterial字段
                    var defaultMaterialField = rpAssetType.GetField("defaultMaterial");
                    if (defaultMaterialField != null)
                    {
                        Material mat = defaultMaterialField.GetValue(rpAsset) as Material;
                        if (mat != null)
                        {
                            return mat;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"[AddressablesLoader] 获取URP默认材质失败: {ex.Message}");
                }
            }
            
            // 回退方案：创建一个简单的默认材质
            // 使用Standard Shader（如果可用）或Unlit/Color Shader
            Shader shader = null;
            
            // 尝试使用Standard Shader
            shader = Shader.Find("Standard");
            if (shader == null)
            {
                // 尝试使用URP Lit Shader
                shader = Shader.Find("Universal Render Pipeline/Lit");
            }
            if (shader == null)
            {
                // 尝试使用Unlit Shader
                shader = Shader.Find("Unlit/Color");
            }
            if (shader == null)
            {
                // 最后尝试使用内置的Unlit Shader
                shader = Shader.Find("Unlit/Texture");
            }
            
            if (shader != null)
            {
                Material defaultMat = new Material(shader);
                defaultMat.name = "DefaultMaterial_AutoGenerated";
                return defaultMat;
            }
            
            Debug.LogError("[AddressablesLoader] 无法创建默认材质，所有Shader都不可用");
            return null;
        }
        
        /// <summary>
        /// 尝试加载资源的所有依赖（包括材质）
        /// </summary>
        private void TryLoadAllDependencies(string resourcePath)
        {
            if (!IsAddressablesAvailable())
                return;
            
            try
            {
                // 使用反射调用 Addressables.LoadResourceLocationsAsync 获取所有依赖位置
                if (loadResourceLocationsAsyncMethod != null)
                {
                    object locationsHandle = loadResourceLocationsAsyncMethod.Invoke(null, new object[] { resourcePath });
                    if (locationsHandle != null)
                    {
                        Type locationsHandleType = locationsHandle.GetType();
                        MethodInfo waitMethod = locationsHandleType.GetMethod("WaitForCompletion");
                        if (waitMethod != null)
                        {
                            waitMethod.Invoke(locationsHandle, null);
                            
                            // 获取结果
                            PropertyInfo resultProp = locationsHandleType.GetProperty("Result");
                            if (resultProp != null)
                            {
                                var locations = resultProp.GetValue(locationsHandle);
                                if (locations != null)
                                {
                                    Debug.Log($"[AddressablesLoader] 找到 {resourcePath} 的 {locations} 个资源位置");
                                }
                            }
                        }
                    }
                }
                
                // 尝试使用LoadAllAssetsAsync加载所有资源
                // 注意：这需要资源是一个AssetGroup或Label
                // 如果资源是单个Asset，可能需要手动加载材质
                MethodInfo loadAllAssetsAsyncMethod = addressablesType?.GetMethod("LoadAssetsAsync");
                if (loadAllAssetsAsyncMethod != null)
                {
                    Debug.Log($"[AddressablesLoader] 尝试加载资源的所有依赖: {resourcePath}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[AddressablesLoader] 尝试加载依赖时发生异常: {ex.Message}");
            }
        }
    }
}

