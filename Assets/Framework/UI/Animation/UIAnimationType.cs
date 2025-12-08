namespace UI.Animation
{
    /// <summary>
    /// UI动画类型
    /// </summary>
    public enum UIAnimationType
    {
        None,           // 无动画
        Fade,            // 淡入淡出
        Scale,           // 缩放
        SlideFromLeft,   // 从左侧滑入
        SlideFromRight,  // 从右侧滑入
        SlideFromTop,    // 从上方滑入
        SlideFromBottom, // 从下方滑入
        Rotate,          // 旋转
        Bounce,          // 弹跳
        Elastic,         // 弹性
        Back             // 回弹
    }
    
    /// <summary>
    /// UI动画配置
    /// </summary>
    [System.Serializable]
    public class UIAnimationConfig
    {
        [UnityEngine.Header("显示动画")]
        public UIAnimationType showAnimationType = UIAnimationType.Fade;
        public float showDuration = 0.3f;
        public float showDelay = 0f;
        
        [UnityEngine.Header("隐藏动画")]
        public UIAnimationType hideAnimationType = UIAnimationType.Fade;
        public float hideDuration = 0.2f;
        public float hideDelay = 0f;
        
        [UnityEngine.Header("动画参数")]
        public float scaleFrom = 0.8f;      // 缩放起始值
        public float slideDistance = 500f;   // 滑动距离
        public float rotateAngle = 360f;    // 旋转角度
    }
}

