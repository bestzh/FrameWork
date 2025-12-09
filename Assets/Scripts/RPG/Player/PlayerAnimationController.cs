using UnityEngine;
using System.Collections.Generic;
using XLua;

/// <summary>
/// 玩家动画控制器 - 管理玩家动画播放
/// </summary>
[XLua.LuaCallCSharp]
public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    
    [Header("动作数据列表")]
    public List<AnimationData> animations;

    private Dictionary<string, AnimationData> animationDict = new Dictionary<string, AnimationData>();
    private AnimationData currentAnimation = null;
    private float currentTimer = 0f;
    private bool isPlaying = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }

        // 初始化动画字典
        if (animations != null)
        {
            foreach (var anim in animations)
            {
                if (anim != null && !animationDict.ContainsKey(anim.AnimationName))
                {
                    animationDict.Add(anim.AnimationName, anim);
                }
            }
        }
    }

    private void Update()
    {
        if (isPlaying && currentAnimation != null)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= currentAnimation.Duration)
            {
                EndCurrentAnimation();
            }
        }
    }
    
    public AnimationData GetCurrentAnimation()
    {
        return currentAnimation;
    }
    
    public AnimationData GetAnimationData(string name)
    {
        if (animationDict.TryGetValue(name, out var anim))
        {
            return anim;
        }
        else
        {
            Debug.LogWarning($"[PlayerAnimationController] 动画 {name} 未找到！");
            return null;
        }
    }

    public bool PlayAnimation(string name)
    {
        if (!animationDict.TryGetValue(name, out var anim))
        {
            Debug.LogWarning($"[PlayerAnimationController] 动画 {name} 未找到！");
            return false;
        }

        if (isPlaying && currentAnimation != null && !currentAnimation.CanInterrupt)
        {
            return false;
        }

        if (animator != null)
        {
            animator.Play(anim.AnimatorStateName);
        }
        
        currentAnimation = anim;
        currentTimer = 0f;
        isPlaying = true;
        return true;
    }

    private void EndCurrentAnimation()
    {
        isPlaying = false;
        currentAnimation = null;
        currentTimer = 0f;
        ReturnToMoveTree();
    }
    
    public void ReturnToMoveTree()
    {
        if (animator != null)
        {
            animator.Play("MoveTree");
        }
    }

    public bool IsPlayingAnimation() => isPlaying;
    
    public string CurrentAnimationName() => currentAnimation != null ? currentAnimation.AnimationName : null;
}

