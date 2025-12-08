using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI.Config
{
    /// <summary>
    /// UI配置管理器
    /// </summary>
    public class UIConfigManager : MonoBehaviour
    {
        private static UIConfigManager instance;
        public static UIConfigManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UIConfigManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("UIConfigManager");
                        instance = go.AddComponent<UIConfigManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        [Header("Config Settings")]
        [SerializeField] private string configPath = "Config/UIConfig";
        [SerializeField] private bool loadOnStart = true;
        [SerializeField] private bool showWarningIfNotFound = false; // 文件不存在时是否显示警告
        
        // UI配置字典
        private Dictionary<string, UIConfig> uiConfigs = new Dictionary<string, UIConfig>();
        
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
        
        private void Start()
        {
            if (loadOnStart)
            {
                LoadConfig();
            }
        }
        
        /// <summary>
        /// 加载配置
        /// </summary>
        public void LoadConfig()
        {
            // 从Resources加载JSON配置
            TextAsset configFile = Resources.Load<TextAsset>(configPath);
            if (configFile != null)
            {
                try
                {
                    UIConfigData configData = JsonUtility.FromJson<UIConfigData>(configFile.text);
                    foreach (var config in configData.configs)
                    {
                        uiConfigs[config.uiName] = config;
                    }
                    Debug.Log($"UI配置加载成功，共 {uiConfigs.Count} 个配置");
                }
                catch (Exception e)
                {
                    Debug.LogError($"加载UI配置失败: {e.Message}");
                }
            }
            else
            {
                if (showWarningIfNotFound)
                {
                    Debug.LogWarning($"UI配置文件未找到: {configPath}。配置文件是可选的，如果不需要配置可以忽略此警告。");
                }
                // 配置文件不存在时，使用默认配置（静默处理）
            }
        }
        
        /// <summary>
        /// 获取UI配置
        /// </summary>
        public UIConfig GetConfig(string uiName)
        {
            // 使用TryGetValue优化性能
            return uiConfigs.TryGetValue(uiName, out UIConfig config) ? config : null;
        }
        
        /// <summary>
        /// 获取UI配置（泛型）
        /// </summary>
        public UIConfig GetConfig<T>() where T : UIBase
        {
            return GetConfig(typeof(T).Name);
        }
        
        /// <summary>
        /// 添加或更新配置
        /// </summary>
        public void SetConfig(UIConfig config)
        {
            uiConfigs[config.uiName] = config;
        }
        
        /// <summary>
        /// 检查是否有配置
        /// </summary>
        public bool HasConfig(string uiName)
        {
            // ContainsKey已经很快，但为了代码一致性，可以保持原样
            return uiConfigs.ContainsKey(uiName);
        }
        
        /// <summary>
        /// 获取所有配置
        /// </summary>
        public Dictionary<string, UIConfig> GetAllConfigs()
        {
            return new Dictionary<string, UIConfig>(uiConfigs);
        }
        
        /// <summary>
        /// 保存配置到文件（仅Editor）
        /// </summary>
#if UNITY_EDITOR
        public void SaveConfig()
        {
            UIConfigData configData = new UIConfigData
            {
                configs = new List<UIConfig>(uiConfigs.Values)
            };
            
            string json = JsonUtility.ToJson(configData, true);
            string path = $"Assets/Resources/{configPath}.json";
            
            // 确保目录存在
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            File.WriteAllText(path, json);
            AssetDatabase.Refresh();
            Debug.Log($"UI配置已保存到: {path}");
        }
#endif
    }
    
    /// <summary>
    /// UI配置数据
    /// </summary>
    [Serializable]
    public class UIConfigData
    {
        public List<UIConfig> configs = new List<UIConfig>();
    }
    
    /// <summary>
    /// UI配置
    /// </summary>
    [Serializable]
    public class UIConfig
    {
        public string uiName;
        public string uiPath;
        public UIHierarchyManager.UILayer layer = UIHierarchyManager.UILayer.Normal;
        public bool isModal = false;
        public bool cacheOnLoad = true;
        public bool preload = false;
        public int poolSize = 0; // 0表示不使用对象池
        public float showAnimationDuration = 0.3f;
        public float hideAnimationDuration = 0.2f;
        public List<CustomProperty> customProperties = new List<CustomProperty>();
    }
    
    /// <summary>
    /// 自定义属性（用于序列化）
    /// </summary>
    [Serializable]
    public class CustomProperty
    {
        public string key;
        public string value;
    }
}

