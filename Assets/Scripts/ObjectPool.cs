using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 对象池信息
/// </summary>
[Serializable]
public class PoolInfo
{
    public string poolName;
    public int activeCount;      // 当前活跃对象数量
    public int poolCount;        // 池中对象数量
    public int maxSize;          // 最大池大小
    public int totalCreated;     // 总共创建的对象数量
    
    public PoolInfo(string name, int max)
    {
        poolName = name;
        maxSize = max;
        activeCount = 0;
        poolCount = 0;
        totalCreated = 0;
    }
}

/// <summary>
/// 通用对象池管理器 - 支持任意GameObject预制体
/// 替代和扩展现有的UI对象池和音效对象池
/// </summary>
public class ObjectPool : MonoBehaviour
{
    private static ObjectPool m_instance;
    
    /// <summary>
    /// 单例实例
    /// </summary>
    public static ObjectPool Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject obj = new GameObject("ObjectPool");
                DontDestroyOnLoad(obj);
                m_instance = obj.AddComponent<ObjectPool>();
            }
            return m_instance;
        }
    }
    
    /// <summary>
    /// 对象池数据
    /// </summary>
    [Serializable]
    private class PoolData
    {
        public string poolName;
        public GameObject prefab;
        public Queue<GameObject> pool = new Queue<GameObject>();
        public HashSet<GameObject> activeObjects = new HashSet<GameObject>();
        public int maxSize;
        public float defaultAutoReleaseTime;  // 默认自动回收时间（-1表示不自动回收）
        public Transform parent;  // 对象池父节点
        public int totalCreated = 0;  // 总共创建的对象数量
        
        public PoolData(string name, GameObject prefab, int maxSize, float defaultAutoReleaseTime, Transform parent)
        {
            this.poolName = name;
            this.prefab = prefab;
            this.maxSize = maxSize;
            this.defaultAutoReleaseTime = defaultAutoReleaseTime;
            this.parent = parent;
            this.totalCreated = 0;
        }
    }
    
    private Dictionary<string, PoolData> pools = new Dictionary<string, PoolData>();
    private Dictionary<GameObject, string> objectToPool = new Dictionary<GameObject, string>(); // 对象到池名的映射
    private Dictionary<GameObject, Coroutine> autoReleaseCoroutines = new Dictionary<GameObject, Coroutine>(); // 自动回收协程
    
    [Header("对象池设置")]
    [SerializeField] private int defaultMaxSize = 50;  // 默认最大池大小
    [SerializeField] private bool logPoolOperations = false;  // 是否记录日志
    
    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    #region 核心功能
    
    /// <summary>
    /// 创建对象池
    /// </summary>
    /// <param name="poolName">池名称</param>
    /// <param name="prefab">预制体</param>
    /// <param name="maxSize">最大池大小（默认50，-1表示使用默认值）</param>
    /// <param name="defaultAutoReleaseTime">默认自动回收时间（秒，-1表示不自动回收，默认-1）</param>
    public void CreatePool(string poolName, GameObject prefab, int maxSize = -1, float defaultAutoReleaseTime = -1f)
    {
        if (string.IsNullOrEmpty(poolName))
        {
            Debug.LogError("[ObjectPool] 池名称不能为空");
            return;
        }
        
        if (prefab == null)
        {
            Debug.LogError($"[ObjectPool] 创建对象池失败: {poolName}，预制体为空");
            return;
        }
        
        if (pools.ContainsKey(poolName))
        {
            Debug.LogWarning($"[ObjectPool] 对象池已存在: {poolName}，将覆盖现有池");
            DestroyPool(poolName);
        }
        
        if (maxSize < 0)
        {
            maxSize = defaultMaxSize;
        }
        
        // 创建池父节点
        GameObject poolParent = new GameObject($"Pool_{poolName}");
        poolParent.transform.SetParent(transform);
        poolParent.SetActive(false);  // 默认隐藏
        
        PoolData poolData = new PoolData(poolName, prefab, maxSize, defaultAutoReleaseTime, poolParent.transform);
        pools[poolName] = poolData;
        
        if (logPoolOperations)
        {
            string autoReleaseInfo = defaultAutoReleaseTime > 0 
                ? $"默认自动回收: {defaultAutoReleaseTime}秒" 
                : "不自动回收";
            Debug.Log($"[ObjectPool] ✓ 创建对象池: {poolName}，最大大小: {maxSize}，{autoReleaseInfo}");
        }
    }
    
    /// <summary>
    /// 从对象池获取对象
    /// </summary>
    /// <param name="poolName">池名称</param>
    /// <param name="prefab">预制体（如果池不存在，会创建新池）</param>
    /// <param name="autoReleaseTime">自动回收时间（秒，-1表示使用池的默认值，-2表示不自动回收）</param>
    /// <returns>GameObject对象，如果失败返回null</returns>
    public GameObject Get(string poolName, GameObject prefab = null, float autoReleaseTime = -2f)
    {
        if (string.IsNullOrEmpty(poolName))
        {
            Debug.LogError("[ObjectPool] 池名称不能为空");
            return null;
        }
        
        // 如果池不存在，尝试创建
        if (!pools.ContainsKey(poolName))
        {
            if (prefab == null)
            {
                Debug.LogError($"[ObjectPool] 对象池不存在且未提供预制体: {poolName}");
                return null;
            }
            CreatePool(poolName, prefab);
        }
        
        PoolData poolData = pools[poolName];
        GameObject obj = null;
        
        // 尝试从池中获取
        while (poolData.pool.Count > 0)
        {
            obj = poolData.pool.Dequeue();
            if (obj != null)
            {
                break;
            }
        }
        
        // 池为空，创建新对象
        if (obj == null)
        {
            obj = Instantiate(poolData.prefab);
            obj.name = $"{poolData.prefab.name}_{poolData.activeObjects.Count}";
            poolData.totalCreated++;
            
            if (logPoolOperations)
            {
                Debug.Log($"[ObjectPool] 创建新对象: {poolName}，总数: {poolData.totalCreated}");
            }
        }
        
        // 激活对象
        obj.SetActive(true);
        obj.transform.SetParent(null);  // 从池中取出，脱离父节点
        
        // 记录活跃对象
        poolData.activeObjects.Add(obj);
        objectToPool[obj] = poolName;
        
        // 调用OnGet回调
        IPoolable poolable = obj.GetComponent<IPoolable>();
        if (poolable != null)
        {
            poolable.OnGet();
        }
        
        // 自动回收处理
        // autoReleaseTime = -2: 不自动回收（手动指定）
        // autoReleaseTime = -1: 使用池的默认值
        // autoReleaseTime >= 0: 使用指定时间
        float actualReleaseTime = -1f;
        if (autoReleaseTime >= 0f)
        {
            // 使用指定的时间
            actualReleaseTime = autoReleaseTime;
        }
        else if (autoReleaseTime == -1f)
        {
            // 使用池的默认值
            actualReleaseTime = poolData.defaultAutoReleaseTime;
        }
        // else: autoReleaseTime == -2，不自动回收
        
        // 如果设置了自动回收时间，自动应用
        if (actualReleaseTime >= 0f)
        {
            AutoRelease(poolName, obj, actualReleaseTime);
        }
        
        return obj;
    }
    
    /// <summary>
    /// 将对象回收到对象池
    /// </summary>
    /// <param name="poolName">池名称</param>
    /// <param name="obj">要回收的对象</param>
    public void Release(string poolName, GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("[ObjectPool] 尝试回收空对象");
            return;
        }
        
        if (string.IsNullOrEmpty(poolName))
        {
            // 尝试从映射中获取池名
            if (objectToPool.TryGetValue(obj, out string mappedPoolName))
            {
                poolName = mappedPoolName;
            }
            else
            {
                Debug.LogError("[ObjectPool] 无法确定对象所属的池，请指定池名称");
                return;
            }
        }
        
        if (!pools.ContainsKey(poolName))
        {
            Debug.LogWarning($"[ObjectPool] 对象池不存在: {poolName}，直接销毁对象");
            Destroy(obj);
            return;
        }
        
        PoolData poolData = pools[poolName];
        
        // 检查对象是否属于此池
        if (!poolData.activeObjects.Contains(obj))
        {
            Debug.LogWarning($"[ObjectPool] 对象不属于此池: {poolName}");
            return;
        }
        
        // 取消自动回收协程
        if (autoReleaseCoroutines.TryGetValue(obj, out Coroutine coroutine))
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            autoReleaseCoroutines.Remove(obj);
        }
        
        // 调用OnRelease回调
        IPoolable poolable = obj.GetComponent<IPoolable>();
        if (poolable != null)
        {
            poolable.OnRelease();
        }
        
        // 回收到池中
        obj.SetActive(false);
        obj.transform.SetParent(poolData.parent);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        
        // 如果池未满，回收；否则销毁
        if (poolData.pool.Count < poolData.maxSize)
        {
            poolData.pool.Enqueue(obj);
        }
        else
        {
            if (logPoolOperations)
            {
                Debug.Log($"[ObjectPool] 池已满，销毁对象: {poolName}");
            }
            Destroy(obj);
        }
        
        // 从活跃对象中移除
        poolData.activeObjects.Remove(obj);
        objectToPool.Remove(obj);
    }
    
    /// <summary>
    /// 将对象回收到对象池（自动识别池名）
    /// </summary>
    /// <param name="obj">要回收的对象</param>
    public void Release(GameObject obj)
    {
        if (obj == null) return;
        
        if (objectToPool.TryGetValue(obj, out string poolName))
        {
            Release(poolName, obj);
        }
        else
        {
            Debug.LogWarning($"[ObjectPool] 无法确定对象所属的池: {obj.name}，直接销毁");
            Destroy(obj);
        }
    }
    
    #endregion
    
    #region 预加载
    
    /// <summary>
    /// 预加载对象到对象池
    /// </summary>
    /// <param name="poolName">池名称</param>
    /// <param name="prefab">预制体</param>
    /// <param name="count">预加载数量</param>
    /// <param name="onComplete">完成回调</param>
    public void Preload(string poolName, GameObject prefab, int count, Action onComplete = null)
    {
        if (string.IsNullOrEmpty(poolName) || prefab == null || count <= 0)
        {
            Debug.LogError($"[ObjectPool] 预加载参数错误: {poolName}");
            onComplete?.Invoke();
            return;
        }
        
        // 确保池存在
        if (!pools.ContainsKey(poolName))
        {
            CreatePool(poolName, prefab);
        }
        
        StartCoroutine(PreloadCoroutine(poolName, prefab, count, onComplete));
    }
    
    /// <summary>
    /// 预加载协程
    /// </summary>
    private IEnumerator PreloadCoroutine(string poolName, GameObject prefab, int count, Action onComplete)
    {
        PoolData poolData = pools[poolName];
        int preloaded = 0;
        
        while (preloaded < count && poolData.pool.Count < poolData.maxSize)
        {
            GameObject obj = Instantiate(prefab);
            obj.name = $"{prefab.name}_Preload_{preloaded}";
            obj.SetActive(false);
            obj.transform.SetParent(poolData.parent);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            
            poolData.pool.Enqueue(obj);
            poolData.totalCreated++;
            preloaded++;
            
            // 每帧最多创建5个，避免卡顿
            if (preloaded % 5 == 0)
            {
                yield return null;
            }
        }
        
        if (logPoolOperations)
        {
            Debug.Log($"[ObjectPool] ✓ 预加载完成: {poolName}，数量: {preloaded}");
        }
        
        onComplete?.Invoke();
    }
    
    #endregion
    
    #region 自动回收
    
    /// <summary>
    /// 延迟自动回收对象
    /// </summary>
    /// <param name="poolName">池名称</param>
    /// <param name="obj">要回收的对象</param>
    /// <param name="delay">延迟时间（秒）</param>
    public void AutoRelease(string poolName, GameObject obj, float delay)
    {
        if (obj == null || delay < 0)
        {
            return;
        }
        
        if (string.IsNullOrEmpty(poolName))
        {
            if (objectToPool.TryGetValue(obj, out string mappedPoolName))
            {
                poolName = mappedPoolName;
            }
            else
            {
                Debug.LogWarning($"[ObjectPool] 无法确定对象所属的池: {obj.name}");
                return;
            }
        }
        
        // 如果已有自动回收协程，先停止
        if (autoReleaseCoroutines.TryGetValue(obj, out Coroutine existingCoroutine))
        {
            if (existingCoroutine != null)
            {
                StopCoroutine(existingCoroutine);
            }
        }
        
        // 启动新的自动回收协程
        Coroutine coroutine = StartCoroutine(AutoReleaseCoroutine(poolName, obj, delay));
        autoReleaseCoroutines[obj] = coroutine;
    }
    
    /// <summary>
    /// 自动回收协程
    /// </summary>
    private IEnumerator AutoReleaseCoroutine(string poolName, GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (obj != null)
        {
            Release(poolName, obj);
        }
        
        if (autoReleaseCoroutines.ContainsKey(obj))
        {
            autoReleaseCoroutines.Remove(obj);
        }
    }
    
    #endregion
    
    #region 池管理
    
    /// <summary>
    /// 销毁对象池
    /// </summary>
    /// <param name="poolName">池名称</param>
    public void DestroyPool(string poolName)
    {
        if (!pools.ContainsKey(poolName))
        {
            Debug.LogWarning($"[ObjectPool] 对象池不存在: {poolName}");
            return;
        }
        
        PoolData poolData = pools[poolName];
        
        // 销毁所有活跃对象
        foreach (GameObject obj in new List<GameObject>(poolData.activeObjects))
        {
            if (obj != null)
            {
                IPoolable poolable = obj.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnDestroy();
                }
                Destroy(obj);
            }
        }
        
        // 销毁池中对象
        while (poolData.pool.Count > 0)
        {
            GameObject obj = poolData.pool.Dequeue();
            if (obj != null)
            {
                IPoolable poolable = obj.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnDestroy();
                }
                Destroy(obj);
            }
        }
        
        // 销毁父节点
        if (poolData.parent != null)
        {
            Destroy(poolData.parent.gameObject);
        }
        
        // 清理映射
        foreach (var kvp in new Dictionary<GameObject, string>(objectToPool))
        {
            if (kvp.Value == poolName)
            {
                objectToPool.Remove(kvp.Key);
            }
        }
        
        pools.Remove(poolName);
        
        if (logPoolOperations)
        {
            Debug.Log($"[ObjectPool] ✓ 销毁对象池: {poolName}");
        }
    }
    
    /// <summary>
    /// 清空对象池（保留池结构，只清空对象）
    /// </summary>
    /// <param name="poolName">池名称</param>
    public void ClearPool(string poolName)
    {
        if (!pools.ContainsKey(poolName))
        {
            Debug.LogWarning($"[ObjectPool] 对象池不存在: {poolName}");
            return;
        }
        
        PoolData poolData = pools[poolName];
        
        // 销毁池中对象
        while (poolData.pool.Count > 0)
        {
            GameObject obj = poolData.pool.Dequeue();
            if (obj != null)
            {
                IPoolable poolable = obj.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnDestroy();
                }
                Destroy(obj);
            }
        }
        
        if (logPoolOperations)
        {
            Debug.Log($"[ObjectPool] ✓ 清空对象池: {poolName}");
        }
    }
    
    /// <summary>
    /// 获取对象池信息
    /// </summary>
    /// <param name="poolName">池名称</param>
    /// <returns>池信息，如果池不存在返回null</returns>
    public PoolInfo GetPoolInfo(string poolName)
    {
        if (!pools.ContainsKey(poolName))
        {
            return null;
        }
        
        PoolData poolData = pools[poolName];
        return new PoolInfo(poolName, poolData.maxSize)
        {
            activeCount = poolData.activeObjects.Count,
            poolCount = poolData.pool.Count,
            totalCreated = poolData.totalCreated
        };
    }
    
    /// <summary>
    /// 设置对象池最大大小
    /// </summary>
    /// <param name="poolName">池名称</param>
    /// <param name="maxSize">最大大小</param>
    public void SetPoolMaxSize(string poolName, int maxSize)
    {
        if (!pools.ContainsKey(poolName))
        {
            Debug.LogWarning($"[ObjectPool] 对象池不存在: {poolName}");
            return;
        }
        
        if (maxSize < 0)
        {
            Debug.LogError("[ObjectPool] 最大大小不能小于0");
            return;
        }
        
        PoolData poolData = pools[poolName];
        poolData.maxSize = maxSize;
        
        // 如果当前池大小超过新限制，销毁多余对象
        while (poolData.pool.Count > maxSize)
        {
            GameObject obj = poolData.pool.Dequeue();
            if (obj != null)
            {
                IPoolable poolable = obj.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnDestroy();
                }
                Destroy(obj);
            }
        }
        
        if (logPoolOperations)
        {
            Debug.Log($"[ObjectPool] 设置对象池最大大小: {poolName} = {maxSize}");
        }
    }
    
    /// <summary>
    /// 设置对象池默认自动回收时间
    /// </summary>
    /// <param name="poolName">池名称</param>
    /// <param name="autoReleaseTime">自动回收时间（秒，-1表示不自动回收）</param>
    public void SetPoolDefaultAutoReleaseTime(string poolName, float autoReleaseTime)
    {
        if (!pools.ContainsKey(poolName))
        {
            Debug.LogWarning($"[ObjectPool] 对象池不存在: {poolName}");
            return;
        }
        
        PoolData poolData = pools[poolName];
        poolData.defaultAutoReleaseTime = autoReleaseTime;
        
        if (logPoolOperations)
        {
            string info = autoReleaseTime > 0 
                ? $"默认自动回收: {autoReleaseTime}秒" 
                : "不自动回收";
            Debug.Log($"[ObjectPool] 设置对象池默认自动回收时间: {poolName} = {info}");
        }
    }
    
    /// <summary>
    /// 获取所有对象池名称
    /// </summary>
    /// <returns>对象池名称列表</returns>
    public List<string> GetAllPoolNames()
    {
        return new List<string>(pools.Keys);
    }
    
    /// <summary>
    /// 回收所有活跃对象到池中
    /// </summary>
    /// <param name="poolName">池名称（如果为空，回收所有池）</param>
    public void ReleaseAll(string poolName = null)
    {
        if (string.IsNullOrEmpty(poolName))
        {
            // 回收所有池的活跃对象
            foreach (var poolData in pools.Values)
            {
                ReleaseAllInPool(poolData);
            }
        }
        else
        {
            if (pools.TryGetValue(poolName, out PoolData poolData))
            {
                ReleaseAllInPool(poolData);
            }
        }
    }
    
    /// <summary>
    /// 回收指定池的所有活跃对象
    /// </summary>
    private void ReleaseAllInPool(PoolData poolData)
    {
        foreach (GameObject obj in new List<GameObject>(poolData.activeObjects))
        {
            if (obj != null)
            {
                Release(poolData.poolName, obj);
            }
        }
    }
    
    #endregion
    
    #region 清理
    
    void OnDestroy()
    {
        // 清理所有对象池
        foreach (string poolName in new List<string>(pools.Keys))
        {
            DestroyPool(poolName);
        }
    }
    
    #endregion
}

/// <summary>
/// 对象池接口 - 实现此接口的对象可以获得生命周期回调
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// 从对象池中取出时调用
    /// </summary>
    void OnGet();
    
    /// <summary>
    /// 回收到对象池时调用
    /// </summary>
    void OnRelease();
    
    /// <summary>
    /// 对象被销毁时调用
    /// </summary>
    void OnDestroy();
}

