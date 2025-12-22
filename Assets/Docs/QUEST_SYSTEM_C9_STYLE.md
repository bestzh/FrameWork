# C9风格任务系统设计

## 🎯 C9任务系统核心特点

### 1. 等级触发机制
- **玩家达到特定等级后**，与NPC对话时才会出现任务选项
- 任务按等级分层，每个等级段有不同的任务
- 未达到等级要求的任务不会显示

### 2. 任务分层结构

```
1-5级：新手任务系列
  ├── 任务1：击败5只史莱姆（1级）
  ├── 任务2：收集10个草药（2级）
  └── 任务3：完成暗影森林（3级）

5-10级：进阶任务系列
  ├── 任务4：击败10只哥布林（5级）
  ├── 任务5：收集20个矿石（6级）
  └── 任务6：完成火焰洞穴（8级）

10-15级：高级任务系列
  ├── 任务7：击败20只骷髅（10级）
  └── 任务8：达到15级（10级）
```

### 3. NPC对话集成

#### 对话选项动态显示
- 玩家等级 < 任务要求：不显示任务选项
- 玩家等级 >= 任务要求：显示"查看任务"选项
- 点击"查看任务"后，显示该NPC的所有可接取任务

#### 对话流程示例
```
玩家（5级）与NPC（ID=1）对话：
┌─────────────────────────────┐
│  NPC：你好，冒险者！          │
│                             │
│  [查看任务]  [打开商店]  [再见] │
└─────────────────────────────┘

点击"查看任务"后：
┌─────────────────────────────┐
│  可接取的任务：              │
│                             │
│  [1级] 新手任务：击败5只史莱姆 │
│  [2级] 收集任务：收集10个草药  │
│  [3级] 探索任务：完成暗影森林  │
│  [5级] 进阶任务：击败10只哥布林│
│                             │
│  [返回]                     │
└─────────────────────────────┘
```

## 📋 实现方案

### 1. 任务配置表（已完成）

**文件**: `Resources/lua/rpg/table/quest_config.lua.txt`

- ✅ 支持 `levelRequirement`（等级要求）
- ✅ 支持 `npcId`（接取任务的NPC）
- ✅ 支持 `prerequisiteQuestId`（前置任务）

### 2. NPC对话系统集成

#### 在对话配置中添加任务选项

**文件**: `Resources/lua/rpg/table/dialogue_tree_config.lua.txt`

```lua
DialogueTreeConfig.Dialogues = {
    [1] = {  -- 任务NPC
        npcText = "你好，冒险者！需要什么帮助吗？",
        options = {
            {text = "查看任务", action = "open_quest"},  -- 打开任务列表
            {text = "再见", action = "close"}
        }
    }
}
```

#### 动态生成任务选项（可选）

如果NPC有多个任务，可以动态生成选项：

```lua
-- 在npc_controller.lua中
local function GenerateQuestOptions(npcId, playerLevel)
    local QuestConfig = require("rpg.table.quest_config")
    local availableQuests = QuestConfig.GetQuestsByNPC(npcId, playerLevel, completedQuests)
    
    local options = {}
    for _, questId in ipairs(availableQuests) do
        local quest = QuestConfig.GetQuest(questId)
        table.insert(options, {
            text = "[" .. quest.levelRequirement .. "级] " .. quest.questName,
            action = "accept_quest",
            questId = questId
        })
    end
    
    return options
end
```

### 3. 任务管理器（Lua）

**文件**: `Resources/lua/rpg/quest_manager.lua.txt`（待创建）

需要实现：
- 获取玩家等级
- 获取NPC的可接取任务列表
- 检查任务是否可以接取
- 接取任务
- 更新任务进度
- 完成任务并发放奖励

### 4. 任务UI

**文件**: `Resources/lua/ui/QuestUI.lua.txt`（待创建）

UI功能：
- 显示可接取任务列表（按等级排序）
- 显示任务详情（描述、目标、奖励）
- 显示任务进度（进行中的任务）
- 接取/放弃任务按钮

## 🔄 工作流程

### 玩家接取任务流程

```
1. 玩家达到5级
   ↓
2. 玩家与NPC（ID=1）对话
   ↓
3. NPC对话显示"查看任务"选项
   ↓
4. 玩家点击"查看任务"
   ↓
5. 任务UI显示可接取任务：
   - [1级] 新手任务：击败5只史莱姆 ✓（已完成）
   - [2级] 收集任务：收集10个草药 ✓（已完成）
   - [3级] 探索任务：完成暗影森林 ✓（已完成）
   - [5级] 进阶任务：击败10只哥布林 ⭐（可接取）
   ↓
6. 玩家点击任务，查看详情
   ↓
7. 玩家点击"接取任务"
   ↓
8. 任务添加到任务列表，开始追踪进度
```

### 任务进度更新流程

```
1. 玩家在副本中击败1只哥布林
   ↓
2. 战斗系统触发事件："ENEMY_KILLED" (enemyId=2)
   ↓
3. 任务管理器监听事件
   ↓
4. 检查是否有"击败哥布林"的任务
   ↓
5. 更新任务进度：1/10
   ↓
6. 任务UI更新显示进度
   ↓
7. 如果进度达到目标，标记任务完成
```

## 📝 代码示例

### 获取NPC的可接取任务

```lua
local QuestConfig = require("rpg.table.quest_config")
local CharacterHelper = require("rpg.character_helper")

-- 获取玩家等级
local player = CharacterHelper.GetPlayerCharacter()
local playerLevel = player.Level

-- 获取NPC的可接取任务
local npcId = 1
local completedQuests = {}  -- 从QuestManager获取已完成任务
local availableQuests = QuestConfig.GetQuestsByNPC(npcId, playerLevel, completedQuests)

-- 显示任务列表
for _, questId in ipairs(availableQuests) do
    local quest = QuestConfig.GetQuest(questId)
    print("[" .. quest.levelRequirement .. "级] " .. quest.questName)
end
```

### NPC对话中显示任务选项

```lua
-- 在npc_controller.lua中
local function ShowDialogueWithQuests(npc)
    local QuestConfig = require("rpg.table.quest_config")
    local CharacterHelper = require("rpg.character_helper")
    
    local player = CharacterHelper.GetPlayerCharacter()
    local playerLevel = player.Level
    
    -- 获取可接取的任务
    local availableQuests = QuestConfig.GetQuestsByNPC(npc.configID, playerLevel, completedQuests)
    
    -- 构建对话选项
    local options = {}
    
    if #availableQuests > 0 then
        table.insert(options, {text = "查看任务", action = "open_quest"})
    end
    
    table.insert(options, {text = "再见", action = "close"})
    
    -- 显示对话
    DialogueManager.ShowSimpleDialogue(
        npc.configID,
        npc.npcName,
        nil,
        options,
        optionCallback
    )
end
```

## ✅ 实现检查清单

- [x] 任务配置表支持等级要求
- [x] 任务配置表支持NPC关联
- [x] 辅助函数支持按等级筛选任务
- [ ] 任务管理器（Lua）
- [ ] 任务UI界面
- [ ] NPC对话集成（显示"查看任务"选项）
- [ ] 任务进度追踪
- [ ] 任务奖励发放

## 🚀 下一步

1. **创建任务管理器（Lua）** - 核心逻辑
2. **创建任务UI** - 显示任务列表和详情
3. **集成到NPC对话** - 添加"查看任务"选项
4. **实现任务进度追踪** - 监听游戏事件更新进度

