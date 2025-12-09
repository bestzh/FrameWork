# 解决T-Pose和动画问题

## 🔍 问题分析

**T-Pose（T型姿态）**通常是因为：
1. **Animator Controller未设置** - Animator组件没有分配Controller（90%的情况）
2. **Animator Controller没有默认状态** - Controller中没有Idle状态
3. **动画参数未设置** - Controller中缺少Horizontal和Vertical参数

---

## ✅ 快速解决方案

### 方案1：分配Animator Controller（最重要！）

#### 步骤：
```
1. 选择Player对象
2. 查看Inspector中的Animator组件
3. 检查Controller字段：
   - 如果为空 → 这是T-Pose的原因！
   - 如果有值 → 检查Controller配置

4. 分配Controller：
   - 找到 Animator/RPG/PlayerAnim.controller
   - 拖拽到Animator组件的Controller字段
   - 如果没有这个文件，需要创建一个
```

#### 如果Controller不存在，创建基础Controller：

**步骤：**
```
1. 右键点击 Animator/RPG/ 文件夹
2. Create → Animator Controller
3. 命名为 "PlayerAnimController"

4. 双击打开Animator窗口（Window → Animation → Animator）

5. 创建状态：
   - 右键空白处 → Create State → Empty
   - 命名为 "Idle"
   - 右键Idle → Set as Layer Default State（设为默认状态）

6. 创建参数：
   - 点击Parameters标签
   - 点击 + 号 → Float
   - 添加 "Horizontal"
   - 添加 "Vertical"

7. 创建Move状态：
   - 右键空白处 → Create State → Empty
   - 命名为 "Move"

8. 设置过渡：
   - 右键Idle → Make Transition → 指向Move
   - 选择过渡箭头
   - 在Inspector中：
     - Conditions: Speed > 0.1（需要先添加Speed参数）
     - 或者使用：Has Exit Time取消勾选，Conditions添加自定义条件

9. 设置过渡（Move → Idle）：
   - 右键Move → Make Transition → 指向Idle
   - Conditions: Speed < 0.1
```

---

### 方案2：使用现有Controller（如果有）

#### 如果已经有PlayerAnim.controller：

**检查配置：**
```
1. 双击打开 Animator/RPG/PlayerAnim.controller
2. 检查是否有默认状态（橙色状态）
3. 检查是否有Horizontal和Vertical参数
4. 检查是否有Idle和Move状态
```

**如果配置不完整：**
- 按照方案1的步骤补充配置

---

### 方案3：临时解决方案（如果没有动画资源）

#### 如果暂时没有动画Clip：

**选项1：创建空状态（至少不会T-Pose）**
```
1. 创建Animator Controller
2. 创建Idle状态（不分配动画Clip）
3. 设置为默认状态
4. 至少模型不会T-Pose（会保持模型原始姿态）
```

**选项2：使用Unity内置动画**
```
1. 如果有角色模型，检查模型是否有动画
2. 导入动画到Animator Controller
3. 设置Idle和Move动画
```

**选项3：暂时禁用Animator（不推荐）**
```
1. 取消勾选Animator组件的Enabled
2. 模型会保持导入时的姿态
3. 后续添加动画后再启用
```

---

## 🔧 使用动画检查脚本

### 已创建AnimationChecker脚本

**步骤：**
```
1. 选择Player对象
2. Add Component → AnimationChecker
3. 运行场景
4. 查看Console输出
5. 根据提示修复问题
```

**脚本会检查：**
- ✅ Animator组件是否存在
- ✅ Animator Controller是否分配
- ✅ Animator参数是否正确
- ✅ PlayerAnimationController配置

---

## 📋 完整配置检查清单

### Animator组件
- [ ] Player有Animator组件
- [ ] Animator的Controller字段已分配
- [ ] Controller中有Idle状态
- [ ] Idle状态设置为默认状态（橙色）
- [ ] Controller中有Horizontal参数（Float）
- [ ] Controller中有Vertical参数（Float）
- [ ] Controller中有Move状态
- [ ] 设置了Idle ↔ Move的过渡

### 动画资源（如果有）
- [ ] 有Idle动画Clip
- [ ] 有Move动画Clip
- [ ] 动画已分配给对应状态

### PlayerAnimationController（可选）
- [ ] Player有PlayerAnimationController组件
- [ ] Animations列表有数据（或暂时留空也可以）

---

## 🎯 推荐配置（RPG项目）

### 基础Animator Controller配置：

**状态：**
- Idle（默认状态）
- Move

**参数：**
- Horizontal (Float) - 水平移动
- Vertical (Float) - 垂直移动
- Speed (Float) - 移动速度（可选）

**过渡：**
- Idle → Move: Speed > 0.1 或 Horizontal != 0 || Vertical != 0
- Move → Idle: Speed < 0.1 或 Horizontal == 0 && Vertical == 0

---

## 🚀 立即检查步骤

### 现在就做：

1. **选择Player对象**
2. **查看Animator组件**
   - Controller字段是否为空？
   - 如果为空 → 这就是问题！

3. **分配Controller**
   - 如果有 `Animator/RPG/PlayerAnim.controller` → 拖拽过去
   - 如果没有 → 创建一个（按照方案1）

4. **添加AnimationChecker脚本**
   - Add Component → AnimationChecker
   - 运行场景
   - 查看Console提示

5. **测试**
   - 运行场景
   - 移动玩家
   - 检查是否还有T-Pose

---

## 💡 关于Cinemachine

### 你已经在使用Cinemachine，很好！

**确保Cinemachine Virtual Camera设置：**
```
1. Follow: Player（拖拽Player对象）
2. Look At: Player（拖拽Player对象）
3. Body: 根据需要选择（Third Person Follow等）
4. Aim: 根据需要选择
```

**如果相机不跟随：**
- 检查Follow和Look At是否设置
- 检查Cinemachine Brain是否添加到Main Camera

---

## ❓ 常见问题

### Q: Controller已分配，但还是T-Pose？
**A:** 
- 检查Controller中是否有默认状态（橙色状态）
- 检查默认状态是否分配了动画Clip
- 检查模型是否有动画资源

### Q: 没有动画资源怎么办？
**A:**
- 可以先创建空状态（至少不会T-Pose）
- 或者使用模型自带的动画
- 或者暂时禁用Animator

### Q: 动画播放但不流畅？
**A:**
- 检查过渡设置（Has Exit Time、Transition Duration）
- 检查动画Clip的Loop设置
- 检查Animator的Update Mode

---

## 🎯 总结

**T-Pose的主要原因：**
1. ❌ Animator Controller未分配（90%）
2. ❌ Controller没有默认状态
3. ❌ 默认状态没有动画

**快速修复：**
1. ✅ 分配Animator Controller
2. ✅ 设置默认状态
3. ✅ 添加基础参数和状态

**完成这些步骤后，告诉我结果！** 🚀
