using UnityEngine;
using System.Collections.Generic;
using XLua;

/// <summary>
/// 角色管理器 - 管理游戏中的所有角色
/// 使用框架的数据存储和事件系统
/// </summary>
[XLua.LuaCallCSharp]
public class CharacterManager : MonoBehaviour
{
    private static CharacterManager m_instance;
    
    public static CharacterManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject obj = new GameObject("CharacterManager");
                DontDestroyOnLoad(obj);
                m_instance = obj.AddComponent<CharacterManager>();
            }
            return m_instance;
        }
    }
    
    /// <summary>
    /// 当前玩家角色
    /// </summary>
    public CharacterData PlayerCharacter { get; private set; }
    
    /// <summary>
    /// 所有角色字典
    /// </summary>
    private Dictionary<int, CharacterData> characters = new Dictionary<int, CharacterData>();
    
    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 初始化
    /// </summary>
    private void Initialize()
    {
        // 加载玩家角色数据
        LoadPlayerCharacter();
        
        Debug.Log("[CharacterManager] 角色管理器初始化完成");
    }
    
    /// <summary>
    /// 创建角色
    /// </summary>
    public CharacterData CreateCharacter(int characterId, string name)
    {
        // 从配置表读取角色基础数据
        // TODO: 使用 TableManager 读取配置
        
        CharacterData character = new CharacterData
        {
            CharacterId = characterId,
            Name = name,
            Level = 1,
            Exp = 0
        };
        
        characters[characterId] = character;
        
        Debug.Log($"[CharacterManager] 创建角色: {name} (ID: {characterId})");
        
        return character;
    }
    
    /// <summary>
    /// 获取角色
    /// </summary>
    public CharacterData GetCharacter(int characterId)
    {
        characters.TryGetValue(characterId, out CharacterData character);
        return character;
    }
    
    /// <summary>
    /// 设置玩家角色
    /// </summary>
    public void SetPlayerCharacter(CharacterData character)
    {
        PlayerCharacter = character;
        
        // 触发事件
        EventManager.Instance?.TriggerEvent(GlobalEventNames.PLAYER_SPAWN, character);
    }
    
    /// <summary>
    /// 角色升级
    /// </summary>
    public void LevelUp(CharacterData character)
    {
        if (character == null) return;
        
        character.Level++;
        character.Exp = 0;
        
        // 保存数据
        SaveCharacter(character);
        
        // 触发事件
        EventManager.Instance?.TriggerEvent(GlobalEventNames.PLAYER_LEVEL_UP, character.Level);
        
        Debug.Log($"[CharacterManager] 角色升级: {character.Name} -> Level {character.Level}");
    }
    
    /// <summary>
    /// 增加经验
    /// </summary>
    public void AddExp(CharacterData character, int exp)
    {
        if (character == null) return;
        
        character.Exp += exp;
        
        // 检查是否升级
        int expRequired = GetExpRequired(character.Level);
        if (character.Exp >= expRequired)
        {
            LevelUp(character);
        }
        
        // 触发事件
        EventManager.Instance?.TriggerEvent(GlobalEventNames.PLAYER_EXP_CHANGED, character.Exp);
        
        // 保存数据
        SaveCharacter(character);
    }
    
    /// <summary>
    /// 获取升级所需经验
    /// </summary>
    private int GetExpRequired(int level)
    {
        // TODO: 从配置表读取
        return level * 100;
    }
    
    /// <summary>
    /// 保存角色数据
    /// </summary>
    public void SaveCharacter(CharacterData character)
    {
        if (character == null) return;
        
        string json = JsonUtility.ToJson(character);
        SaveManager.Instance.SaveToJson($"Character_{character.CharacterId}", json);
    }
    
    /// <summary>
    /// 加载角色数据
    /// </summary>
    public CharacterData LoadCharacter(int characterId)
    {
        string json = SaveManager.Instance.LoadFromJson($"Character_{characterId}");
        if (!string.IsNullOrEmpty(json))
        {
            CharacterData character = JsonUtility.FromJson<CharacterData>(json);
            characters[characterId] = character;
            return character;
        }
        return null;
    }
    
    /// <summary>
    /// 加载玩家角色
    /// </summary>
    private void LoadPlayerCharacter()
    {
        // 尝试加载保存的玩家角色
        CharacterData player = LoadCharacter(0);
        
        if (player == null)
        {
            // 创建默认玩家角色
            player = CreateCharacter(0, "Player");
        }
        
        SetPlayerCharacter(player);
    }
    
    /// <summary>
    /// 保存所有角色数据
    /// </summary>
    public void SaveAllCharacters()
    {
        foreach (var character in characters.Values)
        {
            SaveCharacter(character);
        }
        
        Debug.Log("[CharacterManager] 已保存所有角色数据");
    }
}

