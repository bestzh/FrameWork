using UnityEngine;
using System.Collections.Generic;
using XLua;

/// <summary>
/// 任务管理器 - 管理游戏中的任务
/// 使用框架的事件系统和数据存储
/// </summary>
[XLua.LuaCallCSharp]
public class QuestManager : MonoBehaviour
{
    private static QuestManager m_instance;
    
    public static QuestManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject obj = new GameObject("QuestManager");
                DontDestroyOnLoad(obj);
                m_instance = obj.AddComponent<QuestManager>();
            }
            return m_instance;
        }
    }
    
    /// <summary>
    /// 任务列表
    /// </summary>
    private Dictionary<int, QuestData> quests = new Dictionary<int, QuestData>();
    
    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
            LoadQuests();
            
            // 注册事件监听
            EventManager.Instance?.RegisterEvent(GlobalEventNames.PLAYER_LEVEL_UP, OnPlayerLevelUp);
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 接取任务
    /// </summary>
    public void AcceptQuest(int questId)
    {
        // TODO: 从配置表读取任务数据
        QuestData quest = new QuestData
        {
            QuestId = questId,
            Status = QuestStatus.InProgress,
            Progress = 0
        };
        
        quests[questId] = quest;
        
        SaveQuests();
        
        // 触发事件
        EventManager.Instance?.TriggerEvent("QUEST_ACCEPTED", questId);
        
        Debug.Log($"[QuestManager] 接取任务: QuestId={questId}");
    }
    
    /// <summary>
    /// 更新任务进度
    /// </summary>
    public void UpdateQuestProgress(int questId, int progress)
    {
        if (!quests.TryGetValue(questId, out QuestData quest))
        {
            return;
        }
        
        quest.Progress = progress;
        
        // 检查任务是否完成
        // TODO: 从配置表读取任务目标
        int targetProgress = 10; // 示例
        if (quest.Progress >= targetProgress)
        {
            CompleteQuest(questId);
        }
        else
        {
            SaveQuests();
            EventManager.Instance?.TriggerEvent("QUEST_PROGRESS_UPDATED", questId);
        }
    }
    
    /// <summary>
    /// 完成任务
    /// </summary>
    public void CompleteQuest(int questId)
    {
        if (!quests.TryGetValue(questId, out QuestData quest))
        {
            return;
        }
        
        quest.Status = QuestStatus.Completed;
        
        // 发放奖励
        // TODO: 从配置表读取任务奖励
        int expReward = 100;
        int goldReward = 50;
        
        CharacterData player = CharacterManager.Instance?.PlayerCharacter;
        if (player != null)
        {
            CharacterManager.Instance.AddExp(player, expReward);
            player.Gold += goldReward;
        }
        
        SaveQuests();
        
        // 触发事件
        EventManager.Instance?.TriggerEvent("QUEST_COMPLETED", questId);
        
        Debug.Log($"[QuestManager] 完成任务: QuestId={questId}, 奖励: Exp={expReward}, Gold={goldReward}");
    }
    
    /// <summary>
    /// 获取任务
    /// </summary>
    public QuestData GetQuest(int questId)
    {
        quests.TryGetValue(questId, out QuestData quest);
        return quest;
    }
    
    /// <summary>
    /// 获取所有任务
    /// </summary>
    public List<QuestData> GetAllQuests()
    {
        return new List<QuestData>(quests.Values);
    }
    
    /// <summary>
    /// 玩家升级事件处理
    /// </summary>
    private void OnPlayerLevelUp(object level)
    {
        // 检查是否有需要特定等级的任务
        // TODO: 实现任务触发逻辑
    }
    
    /// <summary>
    /// 保存任务数据
    /// </summary>
    public void SaveQuests()
    {
        QuestSaveData saveData = new QuestSaveData
        {
            Quests = new List<QuestData>(quests.Values)
        };
        
        string json = JsonUtility.ToJson(saveData);
        SaveManager.Instance.SaveToJson("Quests", json);
    }
    
    /// <summary>
    /// 加载任务数据
    /// </summary>
    private void LoadQuests()
    {
        string json = SaveManager.Instance.LoadFromJson("Quests");
        if (!string.IsNullOrEmpty(json))
        {
            QuestSaveData saveData = JsonUtility.FromJson<QuestSaveData>(json);
            quests.Clear();
            foreach (var quest in saveData.Quests)
            {
                quests[quest.QuestId] = quest;
            }
        }
        
        Debug.Log($"[QuestManager] 加载任务数据，任务数量: {quests.Count}");
    }
    
    void OnDestroy()
    {
        if (m_instance == this)
        {
            EventManager.Instance?.UnregisterEvent(GlobalEventNames.PLAYER_LEVEL_UP, OnPlayerLevelUp);
        }
    }
}

/// <summary>
/// 任务状态
/// </summary>
public enum QuestStatus
{
    NotStarted,
    InProgress,
    Completed,
    Failed
}

/// <summary>
/// 任务数据
/// </summary>
[System.Serializable]
public class QuestData
{
    public int QuestId;
    public QuestStatus Status = QuestStatus.NotStarted;
    public int Progress = 0;
}

/// <summary>
/// 任务保存数据
/// </summary>
[System.Serializable]
public class QuestSaveData
{
    public List<QuestData> Quests = new List<QuestData>();
}

