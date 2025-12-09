# 开发起步指南

## 🎯 第一步：确定开发顺序

### 推荐开发顺序（从简单到复杂）

```
1. 城镇场景（静态场景，无交互）
   ↓
2. 玩家在城镇中移动
   ↓
3. NPC系统（基础交互）
   ↓
4. 传送门系统（场景切换）
   ↓
5. 第一个副本（简单副本）
   ↓
6. 战斗系统（已有，需要整合）
   ↓
7. 返回城镇系统
   ↓
8. 完善和优化
```

---

## 📋 第一周开发计划（MVP版本）

### Day 1-2: 创建城镇场景

#### 目标
创建一个简单的城镇场景，玩家可以在其中移动。

#### 任务清单
- [ ] 创建新场景 `Scenes/Town.unity`
- [ ] 添加地面（Plane）
- [ ] 添加玩家（使用已有的PlayerController）
- [ ] 添加基础光照
- [ ] 测试玩家移动

#### 具体步骤

**Step 1: 创建场景**
```
1. Unity中：File → New Scene
2. 保存为：Assets/Scenes/Town.unity
3. 设置场景：删除默认的Main Camera和Directional Light（如果有）
```

**Step 2: 添加玩家**
```
1. 创建一个GameObject，命名为"Player"
2. 添加PlayerController组件（已有）
3. 添加CharacterController或Rigidbody（根据你的PlayerController需求）
4. 添加Animator组件
5. 设置Tag为"Player"
```

**Step 3: 添加相机**
```
1. 创建Main Camera
2. 设置相机跟随玩家（或固定视角）
3. 调整相机位置和角度
```

**Step 4: 测试**
```
1. 运行场景
2. 测试WASD移动
3. 测试鼠标旋转（如果有）
```

---

### Day 3-4: 添加NPC系统（基础版）

#### 目标
添加一个NPC，玩家可以靠近并交互。

#### 任务清单
- [ ] 创建NPC GameObject
- [ ] 创建NPCController脚本
- [ ] 实现交互提示（按E键）
- [ ] 创建对话UI（简单版）
- [ ] 测试交互

#### 具体步骤

**Step 1: 创建NPC**
```
1. 创建一个Cube或Sphere，命名为"NPC_Test"
2. 添加NPCController组件
3. 设置Tag为"NPC"（可选）
```

**Step 2: 创建NPCController脚本**
```csharp
// Scripts/RPG/Town/NPCController.cs
using UnityEngine;
using XLua;

[XLua.LuaCallCSharp]
public class NPCController : MonoBehaviour
{
    [Header("NPC信息")]
    public string npcName = "测试NPC";
    public string dialogueText = "你好，冒险者！";
    
    [Header("交互设置")]
    public float interactionDistance = 2f;
    public GameObject interactionHint; // 交互提示UI
    
    private GameObject player;
    private bool isPlayerNearby = false;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        // 创建交互提示（如果还没有）
        if (interactionHint == null)
        {
            // 可以创建一个简单的Text UI作为提示
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        float distance = Vector3.Distance(transform.position, player.transform.position);
        bool wasNearby = isPlayerNearby;
        isPlayerNearby = distance <= interactionDistance;
        
        // 显示/隐藏交互提示
        if (interactionHint != null)
        {
            interactionHint.SetActive(isPlayerNearby);
        }
        
        // 检测交互输入
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }
    
    void Interact()
    {
        Debug.Log($"[NPC] {npcName}: {dialogueText}");
        
        // TODO: 打开对话UI
        // UIManager.Instance.ShowDialogue(npcName, dialogueText);
    }
    
    void OnDrawGizmosSelected()
    {
        // 在Scene视图中显示交互范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
```

**Step 3: 测试**
```
1. 运行场景
2. 靠近NPC
3. 看到交互提示
4. 按E键交互
5. 查看Console输出
```

---

### Day 5-7: 添加传送门系统

#### 目标
添加传送门，玩家可以进入副本。

#### 任务清单
- [ ] 创建传送门GameObject
- [ ] 创建PortalController脚本
- [ ] 创建副本选择UI（简单版）
- [ ] 实现场景切换
- [ ] 测试传送功能

#### 具体步骤

**Step 1: 创建传送门**
```
1. 创建一个GameObject，命名为"Portal"
2. 添加一个视觉效果（可以是简单的粒子特效或模型）
3. 添加PortalController组件
```

**Step 2: 创建PortalController脚本**
```csharp
// Scripts/RPG/Town/PortalController.cs
using UnityEngine;
using XLua;

[XLua.LuaCallCSharp]
public class PortalController : MonoBehaviour
{
    [Header("传送门设置")]
    public string portalName = "副本传送门";
    public float interactionDistance = 3f;
    public GameObject interactionHint;
    
    [Header("副本列表")]
    public string[] availableDungeons = { "Dungeon1" }; // 场景名称
    
    private GameObject player;
    private bool isPlayerNearby = false;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    void Update()
    {
        if (player == null) return;
        
        float distance = Vector3.Distance(transform.position, player.transform.position);
        isPlayerNearby = distance <= interactionDistance;
        
        if (interactionHint != null)
        {
            interactionHint.SetActive(isPlayerNearby);
        }
        
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }
    
    void Interact()
    {
        Debug.Log($"[Portal] 进入{portalName}");
        
        // 简单版：直接进入第一个副本
        if (availableDungeons.Length > 0)
        {
            EnterDungeon(availableDungeons[0]);
        }
    }
    
    void EnterDungeon(string dungeonSceneName)
    {
        Debug.Log($"[Portal] 正在进入副本: {dungeonSceneName}");
        
        // 使用框架的SceneManager加载场景
        if (SceneManager.Instance != null)
        {
            SceneManager.Instance.LoadScene(dungeonSceneName);
        }
        else
        {
            // Fallback: 使用Unity的SceneManager
            UnityEngine.SceneManagement.SceneManager.LoadScene(dungeonSceneName);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
```

**Step 3: 创建第一个副本场景**
```
1. 创建新场景：Scenes/Dungeon1.unity
2. 添加地面
3. 添加玩家（或使用DontDestroyOnLoad）
4. 添加一些敌人（使用已有的EnemyController）
5. 添加Boss（可选）
```

**Step 4: 测试**
```
1. 运行Town场景
2. 靠近传送门
3. 按E键
4. 应该切换到Dungeon1场景
```

---

## 🎯 第一周完成后的成果

### 应该能够：
1. ✅ 在城镇中移动
2. ✅ 与NPC交互（看到对话）
3. ✅ 通过传送门进入副本
4. ✅ 在副本中战斗（使用已有系统）

### 下一步可以：
1. 完善对话UI
2. 添加更多NPC
3. 添加副本选择UI
4. 完善副本系统

---

## 📝 快速开始代码模板

### 1. 最简单的NPC交互

```csharp
// Scripts/RPG/Town/SimpleNPC.cs
using UnityEngine;

public class SimpleNPC : MonoBehaviour
{
    public string npcName = "NPC";
    public string message = "你好！";
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"{npcName}: {message}");
        }
    }
}
```

### 2. 最简单的传送门

```csharp
// Scripts/RPG/Town/SimplePortal.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimplePortal : MonoBehaviour
{
    public string targetScene = "Dungeon1";
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(targetScene);
        }
    }
}
```

---

## 🚀 立即开始

### 第一步：创建城镇场景

**现在就做：**
1. 打开Unity
2. File → New Scene
3. 保存为 `Scenes/Town.unity`
4. 添加一个Plane作为地面
5. 添加一个Cube作为玩家（临时）
6. 运行，测试移动

**完成这一步后，告诉我，我们继续下一步！**

---

## 💡 开发建议

### 1. 先做最简单的版本
- 不要一开始就做复杂的UI
- 先用Debug.Log输出信息
- 先让功能跑起来，再优化

### 2. 每完成一步就测试
- 不要等到全部做完再测试
- 每完成一个小功能就测试
- 及时发现问题

### 3. 使用已有的系统
- 你的框架已经有战斗系统
- 你的框架已经有场景管理
- 先整合现有系统，再扩展

### 4. 保持简单
- MVP版本不需要完美
- 先实现核心功能
- 后续再优化和扩展

---

## ❓ 遇到问题？

### 常见问题

**Q: 玩家移动不工作？**
- A: 检查PlayerController是否正确添加
- A: 检查Input设置是否正确

**Q: NPC交互不工作？**
- A: 检查距离计算是否正确
- A: 检查Input.GetKeyDown是否触发

**Q: 场景切换不工作？**
- A: 检查场景名称是否正确
- A: 检查场景是否添加到Build Settings

**Q: 不知道下一步做什么？**
- A: 参考这个指南，一步一步来
- A: 完成当前步骤后，继续下一步

---

## 🎯 总结

**第一步应该做什么：**
1. ✅ 创建城镇场景（最简单）
2. ✅ 添加玩家移动（使用已有系统）
3. ✅ 添加一个NPC（基础交互）
4. ✅ 添加传送门（场景切换）

**不要一开始就做：**
- ❌ 复杂的UI系统
- ❌ 完整的对话系统
- ❌ 复杂的副本生成
- ❌ 完美的视觉效果

**记住：先让功能跑起来，再优化！**

---

现在就开始第一步：创建城镇场景！

完成后告诉我，我们继续下一步！🚀

