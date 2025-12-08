using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UI
{
    /// <summary>
    /// UI加载器，提供高级UI加载功能
    /// </summary>
    public class UILoader : MonoBehaviour
    {
        [Header("Loader Settings")]
        [SerializeField] private int maxPreloadCount = 5;
        [SerializeField] private float preloadDelay = 0.1f;
        
        private static UILoader instance;
        public static UILoader Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UILoader>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("UILoader");
                        instance = go.AddComponent<UILoader>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        // 预加载队列
        private Queue<PreloadRequest> preloadQueue = new Queue<PreloadRequest>();
        private Dictionary<string, bool> preloadedUIs = new Dictionary<string, bool>();
        private Coroutine preloadCoroutine;
        
        // 加载进度回调
        public event Action<string, float> OnLoadProgress;
        public event Action<string> OnLoadComplete;
        public event Action<string> OnLoadFailed;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        #region Preload Methods
        
        /// <summary>
        /// 预加载UI
        /// </summary>
        public void PreloadUI<T>(string uiPath, Action<bool> onComplete = null)
        {
            string uiName = typeof(T).Name;
            
            if (preloadedUIs.ContainsKey(uiName))
            {
                onComplete?.Invoke(true);
                return;
            }
            
            PreloadRequest request = new PreloadRequest
            {
                uiPath = uiPath,
                uiName = uiName,
                onComplete = onComplete
            };
            
            preloadQueue.Enqueue(request);
            
            if (preloadCoroutine == null)
            {
                preloadCoroutine = StartCoroutine(ProcessPreloadQueue());
            }
        }
        
        /// <summary>
        /// 批量预加载UI
        /// </summary>
        public void PreloadUIs(List<PreloadRequest> requests, Action onAllComplete = null)
        {
            foreach (var request in requests)
            {
                preloadQueue.Enqueue(request);
            }
            
            if (preloadCoroutine == null)
            {
                preloadCoroutine = StartCoroutine(ProcessPreloadQueue(onAllComplete));
            }
        }
        
        private IEnumerator ProcessPreloadQueue(Action onAllComplete = null)
        {
            while (preloadQueue.Count > 0)
            {
                PreloadRequest request = preloadQueue.Dequeue();
                
                yield return StartCoroutine(UIManager.Instance.LoadUIAsync<UIBase>(request.uiPath, (ui) => {
                    if (ui != null)
                    {
                        preloadedUIs[request.uiName] = true;
                        request.onComplete?.Invoke(true);
                    }
                    else
                    {
                        request.onComplete?.Invoke(false);
                    }
                }, false));
                
                yield return new WaitForSeconds(preloadDelay);
            }
            
            preloadCoroutine = null;
            onAllComplete?.Invoke();
        }
        
        #endregion
        
        #region Advanced Loading Methods
        
        /// <summary>
        /// 带进度回调的异步加载
        /// </summary>
        public IEnumerator LoadUIWithProgress<T>(string uiPath, Action<T> onComplete = null, Action<float> onProgress = null) where T : UIBase
        {
            string uiName = typeof(T).Name;
            
            // 检查缓存
            if (UIManager.Instance.IsUILoaded<T>())
            {
                T cachedUI = UIManager.Instance.GetUI<T>();
                onProgress?.Invoke(1f);
                onComplete?.Invoke(cachedUI);
                yield break;
            }
            
            // 模拟加载进度
            float progress = 0f;
            while (progress < 0.9f)
            {
                progress += UnityEngine.Random.Range(0.1f, 0.3f);
                progress = Mathf.Clamp01(progress);
                onProgress?.Invoke(progress);
                OnLoadProgress?.Invoke(uiName, progress);
                yield return new WaitForSeconds(0.1f);
            }
            
            // 实际加载
            yield return StartCoroutine(UIManager.Instance.LoadUIAsync<T>(uiPath, (ui) => {
                if (ui != null)
                {
                    onProgress?.Invoke(1f);
                    OnLoadProgress?.Invoke(uiName, 1f);
                    onComplete?.Invoke(ui);
                    OnLoadComplete?.Invoke(uiName);
                }
                else
                {
                    OnLoadFailed?.Invoke(uiName);
                }
            }, false));
        }
        
        /// <summary>
        /// 条件加载UI（根据条件决定是否加载）
        /// </summary>
        public T LoadUIWithCondition<T>(string uiPath, Func<bool> condition, bool showImmediately = false) where T : UIBase
        {
            if (condition())
            {
                return UIManager.Instance.LoadUI<T>(uiPath, showImmediately);
            }
            
            Debug.LogWarning($"Condition not met for loading UI: {typeof(T).Name}");
            return null;
        }
        
        /// <summary>
        /// 延迟加载UI
        /// </summary>
        public IEnumerator LoadUIWithDelay<T>(string uiPath, float delay, Action<T> onComplete = null, bool showImmediately = false) where T : UIBase
        {
            yield return new WaitForSeconds(delay);
            
            yield return StartCoroutine(UIManager.Instance.LoadUIAsync<T>(uiPath, onComplete, showImmediately));
        }
        
        #endregion
        
        #region Batch Loading Methods
        
        /// <summary>
        /// 批量加载UI
        /// </summary>
        public IEnumerator LoadUIsBatch<T>(List<string> uiPaths, Action<List<T>> onComplete = null, bool showImmediately = false) where T : UIBase
        {
            List<T> loadedUIs = new List<T>();
            
            foreach (string path in uiPaths)
            {
                yield return StartCoroutine(UIManager.Instance.LoadUIAsync<T>(path, (ui) => {
                    if (ui != null)
                    {
                        loadedUIs.Add(ui);
                        if (showImmediately)
                        {
                            ui.Show();
                        }
                    }
                }, false));
            }
            
            onComplete?.Invoke(loadedUIs);
        }
        
        /// <summary>
        /// 并行加载UI（限制并发数）
        /// </summary>
        public IEnumerator LoadUIsParallel<T>(List<string> uiPaths, int maxConcurrent, Action<List<T>> onComplete = null) where T : UIBase
        {
            List<T> loadedUIs = new List<T>();
            List<Coroutine> loadingCoroutines = new List<Coroutine>();
            
            for (int i = 0; i < uiPaths.Count; i += maxConcurrent)
            {
                int batchSize = Mathf.Min(maxConcurrent, uiPaths.Count - i);
                
                for (int j = 0; j < batchSize; j++)
                {
                    int index = i + j;
                    Coroutine coroutine = StartCoroutine(UIManager.Instance.LoadUIAsync<T>(uiPaths[index], (ui) => {
                        if (ui != null)
                        {
                            loadedUIs.Add(ui);
                        }
                    }, false));
                    loadingCoroutines.Add(coroutine);
                }
                
                // 等待当前批次完成
                foreach (var coroutine in loadingCoroutines)
                {
                    yield return coroutine;
                }
                loadingCoroutines.Clear();
            }
            
            onComplete?.Invoke(loadedUIs);
        }
        
        #endregion
        
        #region Utility Methods
        
        /// <summary>
        /// 检查UI是否已预加载
        /// </summary>
        public bool IsUIPreloaded<T>()
        {
            string uiName = typeof(T).Name;
            return preloadedUIs.ContainsKey(uiName);
        }
        
        /// <summary>
        /// 清除预加载缓存
        /// </summary>
        public void ClearPreloadCache()
        {
            preloadedUIs.Clear();
        }
        
        /// <summary>
        /// 获取预加载队列长度
        /// </summary>
        public int GetPreloadQueueLength()
        {
            return preloadQueue.Count;
        }
        
        /// <summary>
        /// 停止预加载
        /// </summary>
        public void StopPreloading()
        {
            if (preloadCoroutine != null)
            {
                StopCoroutine(preloadCoroutine);
                preloadCoroutine = null;
            }
            preloadQueue.Clear();
        }
        
        #endregion
        
        private void OnDestroy()
        {
            if (instance == this)
            {
                StopPreloading();
            }
        }
    }
    
    /// <summary>
    /// 预加载请求结构
    /// </summary>
    [System.Serializable]
    public class PreloadRequest
    {
        public string uiPath;
        public string uiName;
        public Action<bool> onComplete;
    }
} 