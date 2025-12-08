using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

/// <summary>
/// 数据存储管理器 - 统一管理游戏数据保存和加载
/// 支持PlayerPrefs和JSON文件两种存储方式
/// 支持数据加密（可选）
/// </summary>
public class SaveManager
{
    private static SaveManager m_instance;
    
    /// <summary>
    /// 单例实例
    /// </summary>
    public static SaveManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new SaveManager();
            }
            return m_instance;
        }
    }
    
    /// <summary>
    /// 存储根目录（用于JSON文件存储）
    /// </summary>
    private string saveDataPath;
    
    /// <summary>
    /// 是否启用加密（默认false）
    /// </summary>
    private bool enableEncryption = false;
    
    /// <summary>
    /// 加密密钥（简单实现，生产环境建议使用更安全的方案）
    /// </summary>
    private const string EncryptionKey = "SpaceDogView2024";
    
    private SaveManager()
    {
        // 初始化保存路径
        saveDataPath = Path.Combine(Application.persistentDataPath, "SaveData");
        
        // 确保目录存在
        if (!Directory.Exists(saveDataPath))
        {
            Directory.CreateDirectory(saveDataPath);
        }
        
        Debug.Log($"[SaveManager] 数据存储路径: {saveDataPath}");
    }
    
    #region PlayerPrefs方式（简单数据）
    
    /// <summary>
    /// 保存整数
    /// </summary>
    public void SaveInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
        Debug.Log($"[SaveManager] 保存整数: {key} = {value}");
    }
    
    /// <summary>
    /// 加载整数
    /// </summary>
    public int LoadInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }
    
    /// <summary>
    /// 保存浮点数
    /// </summary>
    public void SaveFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
        Debug.Log($"[SaveManager] 保存浮点数: {key} = {value}");
    }
    
    /// <summary>
    /// 加载浮点数
    /// </summary>
    public float LoadFloat(string key, float defaultValue = 0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }
    
    /// <summary>
    /// 保存字符串
    /// </summary>
    public void SaveString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
        Debug.Log($"[SaveManager] 保存字符串: {key} = {value}");
    }
    
    /// <summary>
    /// 加载字符串
    /// </summary>
    public string LoadString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }
    
    /// <summary>
    /// 保存布尔值
    /// </summary>
    public void SaveBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"[SaveManager] 保存布尔值: {key} = {value}");
    }
    
    /// <summary>
    /// 加载布尔值
    /// </summary>
    public bool LoadBool(string key, bool defaultValue = false)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
    }
    
    /// <summary>
    /// 删除数据
    /// </summary>
    public void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
        Debug.Log($"[SaveManager] 删除数据: {key}");
    }
    
    /// <summary>
    /// 检查数据是否存在
    /// </summary>
    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    
    /// <summary>
    /// 删除所有数据
    /// </summary>
    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[SaveManager] 已删除所有PlayerPrefs数据");
    }
    
    #endregion
    
    #region JSON文件方式（复杂数据）
    
    /// <summary>
    /// 保存JSON字符串到文件
    /// </summary>
    /// <param name="fileName">文件名（不需要扩展名）</param>
    /// <param name="jsonString">JSON字符串</param>
    /// <param name="encrypt">是否加密</param>
    public void SaveToJson(string fileName, string jsonString, bool encrypt = false)
    {
        try
        {
            string filePath = Path.Combine(saveDataPath, fileName + ".json");
            
            // 加密处理
            if (encrypt || enableEncryption)
            {
                jsonString = EncryptString(jsonString);
            }
            
            File.WriteAllText(filePath, jsonString, Encoding.UTF8);
            Debug.Log($"[SaveManager] ✓ 保存JSON文件成功: {fileName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] ✗ 保存JSON文件失败: {fileName}\n{e.Message}");
        }
    }
    
    /// <summary>
    /// 保存对象到JSON文件（泛型版本，用于C#代码）
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="fileName">文件名（不需要扩展名）</param>
    /// <param name="data">要保存的数据</param>
    /// <param name="encrypt">是否加密</param>
    public void SaveToJsonObject<T>(string fileName, T data, bool encrypt = false)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            SaveToJson(fileName, json, encrypt);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] ✗ 序列化对象失败: {fileName}\n{e.Message}");
        }
    }
    
    /// <summary>
    /// 从JSON文件加载JSON字符串
    /// </summary>
    /// <param name="fileName">文件名（不需要扩展名）</param>
    /// <param name="encrypted">文件是否加密</param>
    /// <returns>JSON字符串，如果文件不存在返回null</returns>
    public string LoadFromJson(string fileName, bool encrypted = false)
    {
        try
        {
            string filePath = Path.Combine(saveDataPath, fileName + ".json");
            
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"[SaveManager] JSON文件不存在: {fileName}");
                return null;
            }
            
            string json = File.ReadAllText(filePath, Encoding.UTF8);
            
            // 解密处理
            if (encrypted || enableEncryption)
            {
                json = DecryptString(json);
            }
            
            Debug.Log($"[SaveManager] ✓ 加载JSON文件成功: {fileName}");
            return json;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] ✗ 加载JSON文件失败: {fileName}\n{e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 从JSON文件加载对象（泛型版本，用于C#代码）
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="fileName">文件名（不需要扩展名）</param>
    /// <param name="defaultValue">默认值（如果文件不存在）</param>
    /// <param name="encrypted">文件是否加密</param>
    public T LoadFromJsonObject<T>(string fileName, T defaultValue = default(T), bool encrypted = false)
    {
        try
        {
            string json = LoadFromJson(fileName, encrypted);
            if (string.IsNullOrEmpty(json))
            {
                return defaultValue;
            }
            
            T data = JsonUtility.FromJson<T>(json);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] ✗ 反序列化对象失败: {fileName}\n{e.Message}");
            return defaultValue;
        }
    }
    
    /// <summary>
    /// 检查JSON文件是否存在
    /// </summary>
    public bool HasJsonFile(string fileName)
    {
        string filePath = Path.Combine(saveDataPath, fileName + ".json");
        return File.Exists(filePath);
    }
    
    /// <summary>
    /// 删除JSON文件
    /// </summary>
    public void DeleteJsonFile(string fileName)
    {
        try
        {
            string filePath = Path.Combine(saveDataPath, fileName + ".json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"[SaveManager] 删除JSON文件: {fileName}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 删除JSON文件失败: {fileName}\n{e.Message}");
        }
    }
    
    #endregion
    
    #region 通用方法（自动判断类型）
    
    /// <summary>
    /// 保存数据（自动判断类型）
    /// </summary>
    public void SaveData(string key, object data)
    {
        if (data == null)
        {
            Debug.LogWarning($"[SaveManager] 尝试保存null数据: {key}");
            return;
        }
        
        Type dataType = data.GetType();
        
        if (dataType == typeof(int))
        {
            SaveInt(key, (int)data);
        }
        else if (dataType == typeof(float))
        {
            SaveFloat(key, (float)data);
        }
        else if (dataType == typeof(string))
        {
            SaveString(key, (string)data);
        }
        else if (dataType == typeof(bool))
        {
            SaveBool(key, (bool)data);
        }
        else
        {
            // 复杂对象使用JSON文件保存
            string json = JsonUtility.ToJson(data);
            SaveToJson(key, json, enableEncryption);
        }
    }
    
    /// <summary>
    /// 加载数据（需要指定类型）
    /// </summary>
    public T LoadData<T>(string key, T defaultValue = default(T))
    {
        Type dataType = typeof(T);
        
        if (dataType == typeof(int))
        {
            return (T)(object)LoadInt(key, defaultValue != null ? (int)(object)defaultValue : 0);
        }
        else if (dataType == typeof(float))
        {
            return (T)(object)LoadFloat(key, defaultValue != null ? (float)(object)defaultValue : 0f);
        }
        else if (dataType == typeof(string))
        {
            return (T)(object)LoadString(key, defaultValue != null ? (string)(object)defaultValue : "");
        }
        else if (dataType == typeof(bool))
        {
            return (T)(object)LoadBool(key, defaultValue != null ? (bool)(object)defaultValue : false);
        }
        else
        {
            // 复杂对象从JSON文件加载
            return LoadFromJsonObject(key, defaultValue, enableEncryption);
        }
    }
    
    /// <summary>
    /// 检查数据是否存在
    /// </summary>
    public bool HasData(string key)
    {
        // 先检查PlayerPrefs
        if (HasKey(key))
        {
            return true;
        }
        
        // 再检查JSON文件
        return HasJsonFile(key);
    }
    
    /// <summary>
    /// 删除数据
    /// </summary>
    public void DeleteData(string key)
    {
        // 删除PlayerPrefs
        if (HasKey(key))
        {
            DeleteKey(key);
        }
        
        // 删除JSON文件
        if (HasJsonFile(key))
        {
            DeleteJsonFile(key);
        }
    }
    
    #endregion
    
    #region 加密/解密
    
    /// <summary>
    /// 设置是否启用加密
    /// </summary>
    public void SetEncryptionEnabled(bool enabled)
    {
        enableEncryption = enabled;
        Debug.Log($"[SaveManager] 加密功能: {(enabled ? "已启用" : "已禁用")}");
    }
    
    /// <summary>
    /// 简单字符串加密（AES）
    /// </summary>
    private string EncryptString(string plainText)
    {
        try
        {
            byte[] key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32).Substring(0, 32));
            byte[] iv = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(16).Substring(0, 16));
            
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        byte[] encrypted = msEncrypt.ToArray();
                        return Convert.ToBase64String(encrypted);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 加密失败: {e.Message}");
            return plainText; // 加密失败，返回原文
        }
    }
    
    /// <summary>
    /// 简单字符串解密（AES）
    /// </summary>
    private string DecryptString(string cipherText)
    {
        try
        {
            byte[] key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32).Substring(0, 32));
            byte[] iv = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(16).Substring(0, 16));
            
            byte[] buffer = Convert.FromBase64String(cipherText);
            
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                
                using (MemoryStream msDecrypt = new MemoryStream(buffer))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 解密失败: {e.Message}");
            return cipherText; // 解密失败，返回原文
        }
    }
    
    #endregion
    
    #region 工具方法
    
    /// <summary>
    /// 获取保存数据路径
    /// </summary>
    public string GetSaveDataPath()
    {
        return saveDataPath;
    }
    
    /// <summary>
    /// 获取所有保存的文件列表
    /// </summary>
    public string[] GetAllSaveFiles()
    {
        if (!Directory.Exists(saveDataPath))
        {
            return new string[0];
        }
        
        string[] files = Directory.GetFiles(saveDataPath, "*.json");
        for (int i = 0; i < files.Length; i++)
        {
            files[i] = Path.GetFileNameWithoutExtension(files[i]);
        }
        return files;
    }
    
    /// <summary>
    /// 获取保存文件大小（字节）
    /// </summary>
    public long GetSaveFileSize(string fileName)
    {
        string filePath = Path.Combine(saveDataPath, fileName + ".json");
        if (File.Exists(filePath))
        {
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }
        return 0;
    }
    
    #endregion
}

