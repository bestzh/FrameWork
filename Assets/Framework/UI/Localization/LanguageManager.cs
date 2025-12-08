using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LanguageManager 
{
    // 使用 SysEnv 中的全局变量
    
    // 语言变化事件
    public static event Action<SystemLanguage> OnLanguageChanged;
    
    public static int LanguageID 
    { 
        get 
        {
            if (SysEnv._languageID == -1)
            {
                LoadLanguageFromLocal();
            }
            return SysEnv._languageID;
        }
        set 
        {
            SystemLanguage oldLanguage = (SystemLanguage)SysEnv._languageID;
            SysEnv._languageID = value;
            SaveLanguageToLocal();
            
            // 触发语言变化事件
            SystemLanguage newLanguage = (SystemLanguage)value;
            if (oldLanguage != newLanguage)
            {
                OnLanguageChanged?.Invoke(newLanguage);
            }
        }
    }
    
    /// <summary>
    /// 保存语言设置到本地
    /// </summary>
    private static void SaveLanguageToLocal()
    {
        try
        {
            PlayerPrefs.SetInt("LanguageID", SysEnv._languageID);
            PlayerPrefs.Save();
            Debug.Log($"语言设置已保存：{SysEnv._languageID}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存语言设置失败：{e.Message}");
        }
    }
    
    /// <summary>
    /// 从本地加载语言设置
    /// </summary>
    private static void LoadLanguageFromLocal()
    {
        try
        {
            if (PlayerPrefs.HasKey("LanguageID"))
            {
                SysEnv._languageID = PlayerPrefs.GetInt("LanguageID");
                Debug.Log($"从本地加载语言设置：{SysEnv._languageID}");
            }
            else
            {
                SysEnv._languageID = (int)Application.systemLanguage;
                Debug.Log($"未找到保存的语言设置，使用系统语言：{SysEnv._languageID}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载语言设置失败：{e.Message}");
            SysEnv._languageID = (int)Application.systemLanguage;
        }
    }
    
    /// <summary>
    /// 检查是否已经设置过语言
    /// </summary>
    public static bool HasLanguageBeenSet()
    {
        return PlayerPrefs.HasKey("LanguageID");
    }
    
    /// <summary>
    /// 清除语言设置
    /// </summary>
    public static void ClearLanguageSetting()
    {
        PlayerPrefs.DeleteKey("LanguageID");
        SysEnv._languageID = -1;
        Debug.Log("语言设置已清除");
    }
}

