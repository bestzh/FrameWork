using UnityEngine;
using XLua;

/// <summary>
/// 连招管理器 - 管理技能连招
/// </summary>
[XLua.LuaCallCSharp]
public class ComboManager : MonoBehaviour
{
    private PlayerAnimationController animationController;
    private SkillSystem skillSystem;
    private bool isComboQueued = false;
    private float comboInputTimer = 0f;

    private void Awake()
    {
        animationController = GetComponent<PlayerAnimationController>();
        skillSystem = GetComponent<SkillSystem>();
    }

    private void Update()
    {
        // 更新连招输入窗口
        if (comboInputTimer > 0)
        {
            comboInputTimer -= Time.deltaTime;
            if (comboInputTimer <= 0)
            {
                isComboQueued = false;
            }
        }
        
        // 检测连招输入
        if (animationController != null && animationController.IsPlayingAnimation())
        {
            if (Input.GetMouseButtonDown(0)) // 左键点击连击
            {
                TryQueueCombo();
            }
        }
    }

    /// <summary>
    /// 尝试排队连招
    /// </summary>
    private void TryQueueCombo()
    {
        var currentAnimName = animationController.CurrentAnimationName();
        if (currentAnimName == null) return;

        var currentAnimData = animationController.GetAnimationData(currentAnimName);
        if (currentAnimData == null || string.IsNullOrEmpty(currentAnimData.NextComboName))
            return;

        isComboQueued = true;
        comboInputTimer = 0.5f; // 默认连招窗口时间
    }

    /// <summary>
    /// 尝试执行连招
    /// </summary>
    public void TryExecuteCombo()
    {
        if (!isComboQueued) return;

        var currentAnimName = animationController.CurrentAnimationName();
        var currentAnimData = animationController.GetAnimationData(currentAnimName);
        if (currentAnimData == null) return;

        if (!string.IsNullOrEmpty(currentAnimData.NextComboName))
        {
            // 播放下一个连招动画
            animationController.PlayAnimation(currentAnimData.NextComboName);
            
            // 如果有对应的技能，也释放技能
            if (skillSystem != null)
            {
                skillSystem.TryCastSkill(currentAnimData.NextComboName);
            }
        }

        isComboQueued = false;
        comboInputTimer = 0f;
    }
}

