using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace UI.Localization
{
    /// <summary>
    /// UI本地化组件
    /// </summary>
    public class UILocalization : MonoBehaviour
    {
        [SerializeField] private string localizationKey;
        [SerializeField] private Text targetText;
        [SerializeField] private bool updateOnLanguageChange = true;
        [SerializeField] private string[] formatParams; // 格式化参数
        
        private void Start()
        {
            if (targetText == null)
            {
                targetText = GetComponent<Text>();
            }
            
            UpdateText();
            
            if (updateOnLanguageChange)
            {
                LanguageManager.OnLanguageChanged += OnLanguageChanged;
            }
        }
        
        private void OnLanguageChanged(SystemLanguage language)
        {
            UpdateText();
        }
        
        private void UpdateText()
        {
            if (targetText != null && !string.IsNullOrEmpty(localizationKey))
            {
                string text = GetLocalizedText(localizationKey);
                
                // 格式化文本
                if (formatParams != null && formatParams.Length > 0)
                {
                    try
                    {
                        text = string.Format(text, formatParams);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"格式化本地化文本失败: {e.Message}");
                    }
                }
                
                targetText.text = text;
            }
        }
        
        private string GetLocalizedText(string key)
        {
            // 从本地化表获取文本
            // 这里需要根据你的本地化系统实现
            // 示例实现：
            return UILocalizationManager.Instance?.GetText(key) ?? key;
        }
        
        public void SetKey(string key)
        {
            localizationKey = key;
            UpdateText();
        }
        
        public void SetFormatParams(params string[] formatParamsArray)
        {
            formatParams = formatParamsArray;
            UpdateText();
        }
        
        private void OnDestroy()
        {
            if (updateOnLanguageChange)
            {
                LanguageManager.OnLanguageChanged -= OnLanguageChanged;
            }
        }
    }
    
    /// <summary>
    /// UI本地化管理器
    /// </summary>
    public class UILocalizationManager : MonoBehaviour
    {
        private static UILocalizationManager instance;
        public static UILocalizationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UILocalizationManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("UILocalizationManager");
                        instance = go.AddComponent<UILocalizationManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        // 本地化文本字典
        private Dictionary<string, Dictionary<SystemLanguage, string>> localizationTable = 
            new Dictionary<string, Dictionary<SystemLanguage, string>>();
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                LoadLocalizationTable();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            // 监听语言变化
            LanguageManager.OnLanguageChanged += OnLanguageChanged;
        }
        
        private void OnLanguageChanged(SystemLanguage language)
        {
            // 通知所有UI本地化组件更新
            UILocalization[] localizations = FindObjectsOfType<UILocalization>();
            foreach (var loc in localizations)
            {
                // 通过反射或公共方法更新
                // 这里简化处理，实际应该通过事件通知
            }
        }
        
        /// <summary>
        /// 加载本地化表（从Resources或配置文件）
        /// </summary>
        private void LoadLocalizationTable()
        {
            // TODO: 从Resources或配置文件加载本地化表
            // 示例：
            // TextAsset localizationFile = Resources.Load<TextAsset>("Localization/localization");
            // ParseLocalizationFile(localizationFile);
        }
        
        /// <summary>
        /// 添加本地化文本
        /// </summary>
        public void AddLocalization(string key, SystemLanguage language, string text)
        {
            if (!localizationTable.ContainsKey(key))
            {
                localizationTable[key] = new Dictionary<SystemLanguage, string>();
            }
            localizationTable[key][language] = text;
        }
        
        /// <summary>
        /// 获取本地化文本
        /// </summary>
        public string GetText(string key)
        {
            SystemLanguage currentLanguage = (SystemLanguage)LanguageManager.LanguageID;
            
            if (localizationTable.ContainsKey(key))
            {
                var langDict = localizationTable[key];
                
                // 优先使用当前语言
                if (langDict.ContainsKey(currentLanguage))
                {
                    return langDict[currentLanguage];
                }
                
                // 回退到英语
                if (langDict.ContainsKey(SystemLanguage.English))
                {
                    return langDict[SystemLanguage.English];
                }
                
                // 返回第一个可用的
                foreach (var kvp in langDict)
                {
                    return kvp.Value;
                }
            }
            
            // 未找到，返回key
            Debug.LogWarning($"本地化文本未找到: {key}");
            return key;
        }
        
        /// <summary>
        /// 检查是否有本地化文本
        /// </summary>
        public bool HasText(string key)
        {
            return localizationTable.ContainsKey(key);
        }
        
        private void OnDestroy()
        {
            if (instance == this)
            {
                LanguageManager.OnLanguageChanged -= OnLanguageChanged;
            }
        }
    }
}

