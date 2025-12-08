using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace UI.Performance
{
    /// <summary>
    /// UI性能分析器
    /// </summary>
    public class UIPerformanceProfiler : MonoBehaviour
    {
        private static UIPerformanceProfiler instance;
        public static UIPerformanceProfiler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UIPerformanceProfiler>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("UIPerformanceProfiler");
                        instance = go.AddComponent<UIPerformanceProfiler>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        [Header("Performance Settings")]
        [SerializeField] private bool enableProfiling = true;
        [SerializeField] private bool logToConsole = false;
        [SerializeField] private float updateInterval = 1f; // 更新间隔（秒）
        
        // 性能数据
        private Dictionary<string, UILoadStats> loadStats = new Dictionary<string, UILoadStats>();
        private Dictionary<string, UIShowStats> showStats = new Dictionary<string, UIShowStats>();
        private float lastUpdateTime;
        
        // 当前性能指标
        private int totalUILoads = 0;
        private int totalUIShows = 0;
        private float totalLoadTime = 0f;
        private float totalShowTime = 0f;
        
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
        
        private void Update()
        {
            if (enableProfiling && Time.time - lastUpdateTime >= updateInterval)
            {
                lastUpdateTime = Time.time;
                UpdatePerformanceMetrics();
            }
        }
        
        /// <summary>
        /// 记录UI加载开始
        /// </summary>
        public void RecordLoadStart(string uiName)
        {
            if (!enableProfiling) return;
            
            if (!loadStats.ContainsKey(uiName))
            {
                loadStats[uiName] = new UILoadStats { uiName = uiName };
            }
            
            loadStats[uiName].loadStartTime = Time.realtimeSinceStartup;
        }
        
        /// <summary>
        /// 记录UI加载完成
        /// </summary>
        public void RecordLoadEnd(string uiName)
        {
            if (!enableProfiling) return;
            
            if (loadStats.ContainsKey(uiName))
            {
                float loadTime = Time.realtimeSinceStartup - loadStats[uiName].loadStartTime;
                loadStats[uiName].totalLoadTime += loadTime;
                loadStats[uiName].loadCount++;
                loadStats[uiName].averageLoadTime = loadStats[uiName].totalLoadTime / loadStats[uiName].loadCount;
                
                if (loadTime > loadStats[uiName].maxLoadTime)
                {
                    loadStats[uiName].maxLoadTime = loadTime;
                }
                
                totalUILoads++;
                totalLoadTime += loadTime;
                
                if (logToConsole)
                {
                    Debug.Log($"[UI性能] {uiName} 加载耗时: {loadTime * 1000:F2}ms");
                }
            }
        }
        
        /// <summary>
        /// 记录UI显示开始
        /// </summary>
        public void RecordShowStart(string uiName)
        {
            if (!enableProfiling) return;
            
            if (!showStats.ContainsKey(uiName))
            {
                showStats[uiName] = new UIShowStats { uiName = uiName };
            }
            
            showStats[uiName].showStartTime = Time.realtimeSinceStartup;
        }
        
        /// <summary>
        /// 记录UI显示完成
        /// </summary>
        public void RecordShowEnd(string uiName)
        {
            if (!enableProfiling) return;
            
            if (showStats.ContainsKey(uiName))
            {
                float showTime = Time.realtimeSinceStartup - showStats[uiName].showStartTime;
                showStats[uiName].totalShowTime += showTime;
                showStats[uiName].showCount++;
                showStats[uiName].averageShowTime = showStats[uiName].totalShowTime / showStats[uiName].showCount;
                
                if (showTime > showStats[uiName].maxShowTime)
                {
                    showStats[uiName].maxShowTime = showTime;
                }
                
                totalUIShows++;
                totalShowTime += showTime;
                
                if (logToConsole)
                {
                    Debug.Log($"[UI性能] {uiName} 显示耗时: {showTime * 1000:F2}ms");
                }
            }
        }
        
        /// <summary>
        /// 获取UI加载统计
        /// </summary>
        public UILoadStats GetLoadStats(string uiName)
        {
            return loadStats.ContainsKey(uiName) ? loadStats[uiName] : null;
        }
        
        /// <summary>
        /// 获取UI显示统计
        /// </summary>
        public UIShowStats GetShowStats(string uiName)
        {
            return showStats.ContainsKey(uiName) ? showStats[uiName] : null;
        }
        
        /// <summary>
        /// 获取所有加载统计（按平均加载时间排序）
        /// </summary>
        public List<UILoadStats> GetAllLoadStats()
        {
            return loadStats.Values.OrderByDescending(s => s.averageLoadTime).ToList();
        }
        
        /// <summary>
        /// 获取所有显示统计（按平均显示时间排序）
        /// </summary>
        public List<UIShowStats> GetAllShowStats()
        {
            return showStats.Values.OrderByDescending(s => s.averageShowTime).ToList();
        }
        
        /// <summary>
        /// 获取性能摘要
        /// </summary>
        public PerformanceSummary GetSummary()
        {
            return new PerformanceSummary
            {
                totalUILoads = totalUILoads,
                totalUIShows = totalUIShows,
                averageLoadTime = totalUILoads > 0 ? totalLoadTime / totalUILoads : 0f,
                averageShowTime = totalUIShows > 0 ? totalShowTime / totalUIShows : 0f,
                totalLoadTime = totalLoadTime,
                totalShowTime = totalShowTime,
                uniqueUICount = loadStats.Count
            };
        }
        
        /// <summary>
        /// 更新性能指标
        /// </summary>
        private void UpdatePerformanceMetrics()
        {
            // 可以在这里添加实时性能监控逻辑
            // 例如：检测内存使用、帧率等
        }
        
        /// <summary>
        /// 重置所有统计数据
        /// </summary>
        public void ResetStats()
        {
            loadStats.Clear();
            showStats.Clear();
            totalUILoads = 0;
            totalUIShows = 0;
            totalLoadTime = 0f;
            totalShowTime = 0f;
        }
        
        /// <summary>
        /// 打印性能报告
        /// </summary>
        public void PrintPerformanceReport()
        {
            Debug.Log("=== UI性能报告 ===");
            
            PerformanceSummary summary = GetSummary();
            Debug.Log($"总UI加载次数: {summary.totalUILoads}");
            Debug.Log($"总UI显示次数: {summary.totalUIShows}");
            Debug.Log($"平均加载时间: {summary.averageLoadTime * 1000:F2}ms");
            Debug.Log($"平均显示时间: {summary.averageShowTime * 1000:F2}ms");
            Debug.Log($"唯一UI数量: {summary.uniqueUICount}");
            
            Debug.Log("\n=== 加载时间Top 5 ===");
            var topLoads = GetAllLoadStats().Take(5);
            foreach (var stat in topLoads)
            {
                Debug.Log($"{stat.uiName}: 平均 {stat.averageLoadTime * 1000:F2}ms, 最大 {stat.maxLoadTime * 1000:F2}ms, 次数 {stat.loadCount}");
            }
            
            Debug.Log("\n=== 显示时间Top 5 ===");
            var topShows = GetAllShowStats().Take(5);
            foreach (var stat in topShows)
            {
                Debug.Log($"{stat.uiName}: 平均 {stat.averageShowTime * 1000:F2}ms, 最大 {stat.maxShowTime * 1000:F2}ms, 次数 {stat.showCount}");
            }
        }
        
        private void OnDestroy()
        {
            if (instance == this)
            {
                PrintPerformanceReport();
            }
        }
    }
    
    /// <summary>
    /// UI加载统计
    /// </summary>
    [Serializable]
    public class UILoadStats
    {
        public string uiName;
        public int loadCount;
        public float totalLoadTime;
        public float averageLoadTime;
        public float maxLoadTime;
        public float loadStartTime;
    }
    
    /// <summary>
    /// UI显示统计
    /// </summary>
    [Serializable]
    public class UIShowStats
    {
        public string uiName;
        public int showCount;
        public float totalShowTime;
        public float averageShowTime;
        public float maxShowTime;
        public float showStartTime;
    }
    
    /// <summary>
    /// 性能摘要
    /// </summary>
    [Serializable]
    public class PerformanceSummary
    {
        public int totalUILoads;
        public int totalUIShows;
        public float averageLoadTime;
        public float averageShowTime;
        public float totalLoadTime;
        public float totalShowTime;
        public int uniqueUICount;
    }
}

