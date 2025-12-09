using UnityEngine;
using System.Collections.Generic;
using XLua;

/// <summary>
/// 背包管理器 - 管理玩家的物品和装备
/// 使用框架的数据存储系统
/// </summary>
[XLua.LuaCallCSharp]
public class InventoryManager : MonoBehaviour
{
    private static InventoryManager m_instance;
    
    public static InventoryManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject obj = new GameObject("InventoryManager");
                DontDestroyOnLoad(obj);
                m_instance = obj.AddComponent<InventoryManager>();
            }
            return m_instance;
        }
    }
    
    /// <summary>
    /// 背包最大容量
    /// </summary>
    private const int MaxInventorySize = 50;
    
    /// <summary>
    /// 物品列表
    /// </summary>
    private List<ItemData> items = new List<ItemData>();
    
    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
            LoadInventory();
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 添加物品
    /// </summary>
    public bool AddItem(int itemId, int count = 1)
    {
        // TODO: 从配置表读取物品数据
        ItemData item = new ItemData
        {
            ItemId = itemId,
            Count = count
        };
        
        // 检查是否已有相同物品
        ItemData existingItem = items.Find(i => i.ItemId == itemId);
        if (existingItem != null)
        {
            existingItem.Count += count;
        }
        else
        {
            if (items.Count >= MaxInventorySize)
            {
                Debug.LogWarning("[InventoryManager] 背包已满");
                return false;
            }
            items.Add(item);
        }
        
        SaveInventory();
        
        // 触发事件
        EventManager.Instance?.TriggerEvent("INVENTORY_CHANGED", null);
        
        Debug.Log($"[InventoryManager] 添加物品: ItemId={itemId}, Count={count}");
        
        return true;
    }
    
    /// <summary>
    /// 移除物品
    /// </summary>
    public bool RemoveItem(int itemId, int count = 1)
    {
        ItemData item = items.Find(i => i.ItemId == itemId);
        if (item == null || item.Count < count)
        {
            Debug.LogWarning($"[InventoryManager] 物品不足: ItemId={itemId}");
            return false;
        }
        
        item.Count -= count;
        if (item.Count <= 0)
        {
            items.Remove(item);
        }
        
        SaveInventory();
        
        // 触发事件
        EventManager.Instance?.TriggerEvent("INVENTORY_CHANGED", null);
        
        Debug.Log($"[InventoryManager] 移除物品: ItemId={itemId}, Count={count}");
        
        return true;
    }
    
    /// <summary>
    /// 获取物品数量
    /// </summary>
    public int GetItemCount(int itemId)
    {
        ItemData item = items.Find(i => i.ItemId == itemId);
        return item != null ? item.Count : 0;
    }
    
    /// <summary>
    /// 获取所有物品
    /// </summary>
    public List<ItemData> GetAllItems()
    {
        return new List<ItemData>(items);
    }
    
    /// <summary>
    /// 保存背包数据
    /// </summary>
    public void SaveInventory()
    {
        InventorySaveData saveData = new InventorySaveData
        {
            Items = items
        };
        
        string json = JsonUtility.ToJson(saveData);
        SaveManager.Instance.SaveToJson("Inventory", json);
    }
    
    /// <summary>
    /// 加载背包数据
    /// </summary>
    private void LoadInventory()
    {
        string json = SaveManager.Instance.LoadFromJson("Inventory");
        if (!string.IsNullOrEmpty(json))
        {
            InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);
            items = saveData.Items ?? new List<ItemData>();
        }
        else
        {
            items = new List<ItemData>();
        }
        
        Debug.Log($"[InventoryManager] 加载背包数据，物品数量: {items.Count}");
    }
}

/// <summary>
/// 物品数据
/// </summary>
[System.Serializable]
public class ItemData
{
    public int ItemId;
    public int Count = 1;
}

/// <summary>
/// 背包保存数据
/// </summary>
[System.Serializable]
public class InventorySaveData
{
    public List<ItemData> Items = new List<ItemData>();
}

