using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Framework.ResourceLoader;
using UI.Performance;
using UI.Config;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI Manager Settings")]
        [SerializeField] private Transform uiRoot;
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private int maxPoolSize = 10;
        
        /// <summary>
        /// 获取UI根节点（用于外部访问）
        /// </summary>
        public Transform UIRoot
        {
            get
            {
                if (uiRoot == null)
                {
                    Initialize();
                }
                return uiRoot;
            }
        }
        
        /// <summary>
        /// 获取主Canvas（用于外部访问，UI应该挂载到这个Canvas下）
        /// </summary>
        public Canvas MainCanvas
        {
            get
            {
                if (mainCanvas == null)
                {
                    Initialize();
                }
                return mainCanvas;
            }
        }
        
        private static UIManager instance;
        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<UIManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("UIManager");
                        instance = go.AddComponent<UIManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        // 资源加载器
        private IResourceLoader resourceLoader;
        
        // 性能分析器缓存（避免重复空检查）
        private UIPerformanceProfiler perfProfiler;
        
        // UI缓存字典
        private Dictionary<string, UIBase> uiCache = new Dictionary<string, UIBase>();
        private Dictionary<string, GameObject> uiPrefabCache = new Dictionary<string, GameObject>();
        
        // UI栈管理（使用List替代Stack，支持任意位置移除）
        private List<UIBase> uiStack = new List<UIBase>();
        private HashSet<UIBase> activeUIs = new HashSet<UIBase>(); // 使用HashSet提升Contains性能
        
        // 事件系统
        public event Action<string> OnUILoaded;
        public event Action<string> OnUIUnloaded;
        public event Action<string> OnUIShown;
        public event Action<string> OnUIHidden;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void Initialize()
        {
            // 初始化资源加载器
            InitializeResourceLoader();
            
            // 初始化性能分析器（确保已创建）
            perfProfiler = UIPerformanceProfiler.Instance;
            
            // 初始化配置管理器（确保已创建，配置会在Start时自动加载）
            _ = UIConfigManager.Instance;
            
            if (uiRoot == null)
            {
                // 先尝试查找场景中的Canvas作为UI根节点
                Canvas sceneCanvas = FindFirstObjectByType <Canvas>();
                if (sceneCanvas != null)
                {
                    // 使用场景Canvas作为UI根节点
                    uiRoot = sceneCanvas.transform;
                    mainCanvas = sceneCanvas;
                    Debug.Log($"使用场景Canvas作为UI根节点: {sceneCanvas.name}");
                }
                else
                {
                    // 如果没有场景Canvas，创建新的
                    GameObject root = new GameObject("UI Root");
                    root.transform.SetParent(transform);
                    uiRoot = root.transform;
                }
            }
            
            if (mainCanvas == null)
            {
                // 再次尝试查找场景Canvas
                Canvas sceneCanvas = FindFirstObjectByType <Canvas>();
                if (sceneCanvas != null)
                {
                    mainCanvas = sceneCanvas;
                    // 确保场景Canvas有GraphicRaycaster
                    if (sceneCanvas.GetComponent<GraphicRaycaster>() == null)
                    {
                        sceneCanvas.gameObject.AddComponent<GraphicRaycaster>();
                    }
                    Debug.Log($"使用场景Canvas: {sceneCanvas.name}");
                }
                else
                {
                    // 创建新的Canvas
                    GameObject canvasObj = new GameObject("Main Canvas");
                    canvasObj.transform.SetParent(uiRoot);
                    mainCanvas = canvasObj.AddComponent<Canvas>();
                    mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    mainCanvas.sortingOrder = 0;
                    
                    CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1920, 1080);
                    scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    scaler.matchWidthOrHeight = 0.5f;
                    
                    canvasObj.AddComponent<GraphicRaycaster>();
                }
            }
        }
        
        /// <summary>
        /// 初始化资源加载器
        /// UIManager统一使用ResManager的全局资源加载器，确保唯一性
        /// 资源加载器由GameInitializer统一配置，UIManager自动使用ResManager的加载器
        /// </summary>
        private void InitializeResourceLoader()
        {
            // 通过GetResourceLoader()统一获取，避免重复代码
            var loader = GetResourceLoader();
            Debug.Log($"[UIManager] 已使用ResManager的全局资源加载器: {loader.GetType().Name}");
        }
        
        /// <summary>
        /// 获取当前资源加载器（从ResManager获取，确保一致性）
        /// </summary>
        public IResourceLoader GetResourceLoader()
        {
            // 确保ResManager已初始化
            _ = ResManager.Instance;
            
            // 始终从ResManager获取，确保使用最新的加载器
            resourceLoader = ResManager.GetResourceLoader();
            return resourceLoader;
        }
        
        #region UI Loading Methods
        
        /// <summary>
        /// 加载UI（使用资源加载器）
        /// </summary>
        public T LoadUI<T>(string uiPath, bool showImmediately = false) where T : UIBase
        {
            string uiName = typeof(T).Name;
            
            // 性能分析：记录加载开始
            perfProfiler?.RecordLoadStart(uiName);
            
            // 检查配置
            UIConfig config = UIConfigManager.Instance?.GetConfig<T>();
            if (config != null)
            {
                // 使用配置中的路径（如果配置存在）
                if (!string.IsNullOrEmpty(config.uiPath))
                {
                    uiPath = config.uiPath;
                }
            }
            
            // 检查缓存（使用TryGetValue优化性能）
            if (uiCache.TryGetValue(uiName, out UIBase cachedUIBase))
            {
                T cachedUI = cachedUIBase as T;
                if (showImmediately) cachedUI.Show();
                
                // 性能分析：记录加载完成（缓存命中）
                perfProfiler?.RecordLoadEnd(uiName);
                return cachedUI;
            }
            
            // 使用资源加载器加载
            if (resourceLoader == null)
            {
                InitializeResourceLoader();
            }
            
            GameObject prefab = resourceLoader.Load<GameObject>(uiPath);
            if (prefab == null)
            {
                Debug.LogError($"Failed to load UI prefab from path: {uiPath} using {resourceLoader.GetType().Name}");
                
                // 性能分析：记录加载失败
                UIPerformanceProfiler.Instance?.RecordLoadEnd(uiName);
                return null;
            }
            
            T ui = CreateUI<T>(prefab, uiName, showImmediately, config);
            
            // 性能分析：记录加载完成
            perfProfiler?.RecordLoadEnd(uiName);
            
            return ui;
        }
        
        /// <summary>
        /// 加载UI（从预制体）
        /// </summary>
        public T LoadUI<T>(GameObject prefab, bool showImmediately = false) where T : UIBase
        {
            string uiName = typeof(T).Name;
            
            // 检查缓存（使用TryGetValue优化性能）
            if (uiCache.TryGetValue(uiName, out UIBase cachedUIBase))
            {
                T cachedUI = cachedUIBase as T;
                if (showImmediately) cachedUI.Show();
                return cachedUI;
            }
            
            return CreateUI<T>(prefab, uiName, showImmediately);
        }
        
        /// <summary>
        /// 异步加载UI（使用资源加载器）
        /// </summary>
        public IEnumerator LoadUIAsync<T>(string uiPath, Action<T> onComplete = null, bool showImmediately = false) where T : UIBase
        {
            string uiName = typeof(T).Name;
            
            // 检查缓存（使用TryGetValue优化性能）
            if (uiCache.TryGetValue(uiName, out UIBase cachedUIBase))
            {
                T cachedUI = cachedUIBase as T;
                if (showImmediately) cachedUI.Show();
                onComplete?.Invoke(cachedUI);
                yield break;
            }
            
            // 使用资源加载器异步加载
            if (resourceLoader == null)
            {
                InitializeResourceLoader();
            }
            
            GameObject prefab = null;
            yield return resourceLoader.LoadAsync<GameObject>(uiPath, (loadedPrefab) =>
            {
                prefab = loadedPrefab;
            });
            
            if (prefab == null)
            {
                Debug.LogError($"Failed to load UI prefab from path: {uiPath} using {resourceLoader.GetType().Name}");
                onComplete?.Invoke(null);
                yield break;
            }
            
            T ui = CreateUI<T>(prefab, uiName, showImmediately);
            onComplete?.Invoke(ui);
        }
        
        private T CreateUI<T>(GameObject prefab, string uiName, bool showImmediately, UIConfig config = null) where T : UIBase
        {
            GameObject uiObject = Instantiate(prefab, uiRoot);
            T uiComponent = uiObject.GetComponent<T>();
            
            if (uiComponent == null)
            {
                Debug.LogError($"UI component {typeof(T).Name} not found on prefab");
                Destroy(uiObject);
                return null;
            }
            
            // 缓存UI
            uiCache[uiName] = uiComponent;
            uiPrefabCache[uiName] = prefab;
            
            // 初始化
            uiComponent.Initialize();
            
            // 应用配置
            if (config != null)
            {
                // 设置UI层级
                UIHierarchyManager.Instance.SetUILayer(uiComponent, config.layer);
                
                // 应用动画时长（如果UIBase支持）
                // 这里可以通过反射或接口来设置
            }
            else
            {
                // 设置UI层级（默认Normal层）
                UIHierarchyManager.Instance.SetUILayer(uiComponent, UIHierarchyManager.UILayer.Normal);
            }
            
            if (showImmediately)
            {
                // 性能分析：记录显示开始
                perfProfiler?.RecordShowStart(uiName);
                
                uiComponent.Show();
                
                // 性能分析：记录显示完成
                perfProfiler?.RecordShowEnd(uiName);
                
                // HashSet的Add会自动处理重复，无需Contains检查
                activeUIs.Add(uiComponent);
                PushToStack(uiComponent);
            }
            else
            {
                uiComponent.HideImmediate();
            }
            
            OnUILoaded?.Invoke(uiName);
            return uiComponent;
        }
        
        /// <summary>
        /// 加载UI并设置层级
        /// </summary>
        public T LoadUI<T>(string uiPath, UIHierarchyManager.UILayer layer, bool showImmediately = false) where T : UIBase
        {
            T ui = LoadUI<T>(uiPath, false);
            if (ui != null)
            {
                UIHierarchyManager.Instance.SetUILayer(ui, layer);
                if (showImmediately)
                {
                    ui.Show();
                }
            }
            return ui;
        }
        
        /// <summary>
        /// 加载模态UI（带遮罩）
        /// </summary>
        public T LoadModalUI<T>(string uiPath, System.Action onMaskClick = null) where T : UIBase
        {
            T ui = LoadUI<T>(uiPath, UIHierarchyManager.UILayer.Dialog, true);
            if (ui != null)
            {
                // 创建模态遮罩
                UIHierarchyManager.Instance.CreateModalMask(ui.transform, onMaskClick);
            }
            return ui;
        }
        
        #endregion
        
        #region UI Pool Methods
        
        /// <summary>
        /// 从对象池获取UI（使用通用对象池）
        /// </summary>
        public T GetUIFromPool<T>(string uiPath) where T : UIBase
        {
            string uiName = typeof(T).Name;
            string poolName = $"UI_{uiName}";
            
            // 获取UI预制体（从缓存或加载）
            GameObject prefab = null;
            if (uiPrefabCache.ContainsKey(uiPath))
            {
                prefab = uiPrefabCache[uiPath];
            }
            else
            {
                // 加载预制体
                if (resourceLoader == null)
                {
                    InitializeResourceLoader();
                }
                prefab = resourceLoader.Load<GameObject>(uiPath);
                if (prefab != null)
                {
                    uiPrefabCache[uiPath] = prefab;
                }
            }
            
            if (prefab == null)
            {
                Debug.LogError($"[UIManager] 无法获取UI预制体: {uiPath}");
                return null;
            }
            
            // 如果对象池不存在，创建它
            if (ObjectPool.Instance.GetPoolInfo(poolName) == null)
            {
                ObjectPool.Instance.CreatePool(poolName, prefab, maxPoolSize);
            }
            
            // 从通用对象池获取
            GameObject pooledObj = ObjectPool.Instance.Get(poolName, prefab);
            if (pooledObj != null)
            {
                T pooledUI = pooledObj.GetComponent<T>();
                if (pooledUI != null)
                {
                    pooledUI.gameObject.SetActive(true);
                    return pooledUI;
                }
            }
            
            // 如果对象池中没有，创建新的UI
            return LoadUI<T>(uiPath, false);
        }
        
        /// <summary>
        /// 将UI放回对象池（使用通用对象池）
        /// </summary>
        public void ReturnUIToPool<T>(T ui) where T : UIBase
        {
            if (ui == null || ui.gameObject == null)
            {
                return;
            }
            
            string uiName = typeof(T).Name;
            string poolName = $"UI_{uiName}";
            
            // 隐藏UI并回收到对象池
            ui.HideImmediate();
            ObjectPool.Instance.Release(poolName, ui.gameObject);
        }
        
        #endregion
        
        #region UI Show/Hide Methods
        
        /// <summary>
        /// 显示UI
        /// </summary>
        public void ShowUI<T>() where T : UIBase
        {
            string uiName = typeof(T).Name;
            
            // 使用TryGetValue优化性能
            if (uiCache.TryGetValue(uiName, out UIBase ui))
            {
                // 性能分析：记录显示开始
                perfProfiler?.RecordShowStart(uiName);
                
                ui.Show();
                
                // 性能分析：记录显示完成
                perfProfiler?.RecordShowEnd(uiName);
                
                // HashSet的Add会自动处理重复，无需Contains检查
                activeUIs.Add(ui);
                PushToStack(ui);
                OnUIShown?.Invoke(uiName);
            }
            else
            {
                Debug.LogWarning($"UI {uiName} not loaded. Use LoadUI first.");
            }
        }
        
        /// <summary>
        /// 根据配置加载UI
        /// </summary>
        public T LoadUIFromConfig<T>(bool showImmediately = false) where T : UIBase
        {
            UIConfig config = UIConfigManager.Instance?.GetConfig<T>();
            if (config == null)
            {
                Debug.LogWarning($"UI配置未找到: {typeof(T).Name}");
                return null;
            }
            
            return LoadUI<T>(config.uiPath, showImmediately);
        }
        
        /// <summary>
        /// 获取性能报告
        /// </summary>
        public void PrintPerformanceReport()
        {
            UIPerformanceProfiler.Instance?.PrintPerformanceReport();
        }
        
        /// <summary>
        /// 隐藏UI
        /// </summary>
        public void HideUI<T>() where T : UIBase
        {
            string uiName = typeof(T).Name;
            
            // 使用TryGetValue优化性能
            if (uiCache.TryGetValue(uiName, out UIBase ui))
            {
                ui.Hide();
                activeUIs.Remove(ui);
                
                // 从栈中移除（支持任意位置移除）
                RemoveFromStack(ui);
                
                OnUIHidden?.Invoke(uiName);
            }
        }
        
        /// <summary>
        /// 隐藏所有UI
        /// </summary>
        public void HideAllUI()
        {
            // HashSet可以直接遍历，无需ToArray
            foreach (var ui in activeUIs)
            {
                ui.Hide();
            }
            activeUIs.Clear();
            uiStack.Clear();
        }
        
        /// <summary>
        /// 隐藏最顶层的UI
        /// </summary>
        public void HideTopUI()
        {
            if (uiStack.Count > 0)
            {
                UIBase topUI = uiStack[uiStack.Count - 1]; // 获取最后一个元素（栈顶）
                uiStack.RemoveAt(uiStack.Count - 1); // 移除最后一个元素
                topUI.Hide();
                activeUIs.Remove(topUI);
                OnUIHidden?.Invoke(topUI.GetType().Name);
            }
        }
        
        /// <summary>
        /// 将UI推入栈（如果已存在则移到栈顶）
        /// </summary>
        private void PushToStack(UIBase ui)
        {
            // 如果UI已在栈中，先移除
            RemoveFromStack(ui);
            // 添加到栈顶
            uiStack.Add(ui);
        }
        
        /// <summary>
        /// 从栈中移除UI（支持任意位置）
        /// </summary>
        private void RemoveFromStack(UIBase ui)
        {
            uiStack.Remove(ui);
        }
        
        /// <summary>
        /// 获取栈顶UI（不移除）
        /// </summary>
        public UIBase PeekStack()
        {
            if (uiStack.Count > 0)
            {
                return uiStack[uiStack.Count - 1];
            }
            return null;
        }
        
        /// <summary>
        /// 获取UI在栈中的位置（从底部开始，0为最底层）
        /// </summary>
        public int GetUIStackIndex<T>() where T : UIBase
        {
            string uiName = typeof(T).Name;
            // 使用TryGetValue优化性能
            if (uiCache.TryGetValue(uiName, out UIBase ui))
            {
                return uiStack.IndexOf(ui);
            }
            return -1;
        }
        
        #endregion
        
        #region UI Unload Methods
        
        /// <summary>
        /// 卸载UI
        /// </summary>
        public void UnloadUI<T>() where T : UIBase
        {
            string uiName = typeof(T).Name;
            
            // 使用TryGetValue优化性能
            if (uiCache.TryGetValue(uiName, out UIBase ui))
            {
                // 从活动列表中移除
                activeUIs.Remove(ui);
                
                // 从栈中移除（支持任意位置移除）
                RemoveFromStack(ui);
                
                // 销毁UI
                Destroy(ui.gameObject);
                uiCache.Remove(uiName);
                uiPrefabCache.Remove(uiName);
                
                OnUIUnloaded?.Invoke(uiName);
            }
        }
        
        /// <summary>
        /// 卸载所有UI
        /// </summary>
        public void UnloadAllUI()
        {
            foreach (var kvp in uiCache)
            {
                if (kvp.Value != null)
                {
                    Destroy(kvp.Value.gameObject);
                }
            }
            
            uiCache.Clear();
            uiPrefabCache.Clear();
            activeUIs.Clear();
            uiStack.Clear();
        }
        
        #endregion
        
        #region Utility Methods
        
        /// <summary>
        /// 获取UI实例
        /// </summary>
        public T GetUI<T>() where T : UIBase
        {
            string uiName = typeof(T).Name;
            
            // 使用TryGetValue优化性能
            if (uiCache.TryGetValue(uiName, out UIBase ui))
            {
                return ui as T;
            }
            
            return null;
        }
        
        /// <summary>
        /// 检查UI是否已加载
        /// </summary>
        public bool IsUILoaded<T>() where T : UIBase
        {
            string uiName = typeof(T).Name;
            return uiCache.ContainsKey(uiName); // ContainsKey已经很快，无需优化
        }
        
        /// <summary>
        /// 检查UI是否可见
        /// </summary>
        public bool IsUIVisible<T>() where T : UIBase
        {
            T ui = GetUI<T>();
            return ui != null && ui.IsVisible;
        }
        
        /// <summary>
        /// 获取当前活动的UI数量
        /// </summary>
        public int GetActiveUICount()
        {
            return activeUIs.Count;
        }
        
        /// <summary>
        /// 获取UI栈深度
        /// </summary>
        public int GetUIStackDepth()
        {
            return uiStack.Count;
        }
        
        /// <summary>
        /// 获取栈中所有UI（从底层到顶层）
        /// </summary>
        public List<UIBase> GetUIStack()
        {
            return new List<UIBase>(uiStack);
        }
        
        /// <summary>
        /// 清空UI栈
        /// </summary>
        public void ClearUIStack()
        {
            uiStack.Clear();
        }
        
        #endregion
        
        private void OnDestroy()
        {
            if (instance == this)
            {
                UnloadAllUI();
            }
        }
    }
} 