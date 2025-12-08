using UnityEngine;
using UI;
using UI.Animation;

/// <summary>
/// UI动画使用示例
/// </summary>
public class AnimationExampleUI : UIBase
{
    // 示例1：在Inspector中配置动画
    // 1. 勾选 "Use Advanced Animation"
    // 2. 设置 Show Animation Type 为 Scale
    // 3. 设置 Hide Animation Type 为 Scale
    // 4. 设置 Scale From 为 0.8
    
    public override void Show()
    {
        base.Show(); // 会自动播放配置的动画
    }
    
    public override void Hide()
    {
        base.Hide(); // 会自动播放配置的动画
    }
    
    // 示例2：运行时切换动画类型
    public void ShowWithBounce()
    {
        animationConfig.showAnimationType = UIAnimationType.Bounce;
        animationConfig.showDuration = 0.5f;
        Show();
    }
    
    public void ShowWithSlide()
    {
        animationConfig.showAnimationType = UIAnimationType.SlideFromBottom;
        animationConfig.slideDistance = 800f;
        Show();
    }
    
    // 示例3：自定义动画参数
    protected override void Start()
    {
        // 自定义动画配置
        animationConfig.scaleFrom = 0.5f;      // 从50%缩放
        animationConfig.slideDistance = 1000f;  // 滑动1000像素
        animationConfig.showDuration = 0.4f;    // 0.4秒动画
        animationConfig.hideDuration = 0.3f;    // 0.3秒隐藏动画
    }
}

