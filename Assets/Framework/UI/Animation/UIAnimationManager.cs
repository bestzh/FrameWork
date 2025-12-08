using UnityEngine;
using System;
using System.Collections;

// DOTween引用（如果项目中没有DOTween，需要注释掉或使用条件编译）
// 如果编译错误，请注释掉下面这行
using DG.Tweening;

namespace UI.Animation
{
    /// <summary>
    /// UI动画管理器
    /// </summary>
    public static class UIAnimationManager
    {
        /// <summary>
        /// 播放显示动画
        /// </summary>
        public static void PlayShowAnimation(RectTransform rectTransform, CanvasGroup canvasGroup, 
            UIAnimationConfig config, Action onComplete = null)
        {
            if (config == null || config.showAnimationType == UIAnimationType.None)
            {
                // 无动画，直接完成
                if (canvasGroup != null) canvasGroup.alpha = 1f;
                onComplete?.Invoke();
                return;
            }

            // 尝试使用DOTween，如果不可用则使用协程
            try
            {
                PlayDOTweenShowAnimation(rectTransform, canvasGroup, config, onComplete);
            }
            catch
            {
                // DOTween不可用，使用协程
                PlayCoroutineShowAnimation(rectTransform, canvasGroup, config, onComplete);
            }
        }

        /// <summary>
        /// 播放隐藏动画
        /// </summary>
        public static void PlayHideAnimation(RectTransform rectTransform, CanvasGroup canvasGroup,
            UIAnimationConfig config, Action onComplete = null)
        {
            if (config == null || config.hideAnimationType == UIAnimationType.None)
            {
                // 无动画，直接完成
                if (canvasGroup != null) canvasGroup.alpha = 0f;
                onComplete?.Invoke();
                return;
            }

            // 尝试使用DOTween，如果不可用则使用协程
            try
            {
                PlayDOTweenHideAnimation(rectTransform, canvasGroup, config, onComplete);
            }
            catch
            {
                // DOTween不可用，使用协程
                PlayCoroutineHideAnimation(rectTransform, canvasGroup, config, onComplete);
            }
        }

        /// <summary>
        /// 使用DOTween播放显示动画
        /// </summary>
        private static void PlayDOTweenShowAnimation(RectTransform rectTransform, CanvasGroup canvasGroup,
            UIAnimationConfig config, Action onComplete)
        {
            // 停止之前的动画
            rectTransform.DOKill();
            if (canvasGroup != null) canvasGroup.DOKill();

            // 保存初始状态
            Vector3 originalPosition = rectTransform.anchoredPosition;
            Vector3 originalScale = rectTransform.localScale;
            Quaternion originalRotation = rectTransform.localRotation;

            // 设置初始状态
            switch (config.showAnimationType)
            {
                case UIAnimationType.Fade:
                    if (canvasGroup != null) canvasGroup.alpha = 0f;
                    if (canvasGroup != null)
                    {
                        canvasGroup.DOFade(1f, config.showDuration)
                            .SetDelay(config.showDelay)
                            .OnComplete(() => onComplete?.Invoke());
                    }
                    else
                    {
                        onComplete?.Invoke();
                    }
                    break;

                case UIAnimationType.Scale:
                    rectTransform.localScale = Vector3.one * config.scaleFrom;
                    if (canvasGroup != null) canvasGroup.alpha = 0f;
                    
                    Sequence seq = DOTween.Sequence();
                    seq.Append(rectTransform.DOScale(Vector3.one, config.showDuration).SetEase(Ease.OutBack));
                    if (canvasGroup != null)
                    {
                        seq.Join(canvasGroup.DOFade(1f, config.showDuration));
                    }
                    seq.SetDelay(config.showDelay);
                    seq.OnComplete(() => onComplete?.Invoke());
                    break;

                case UIAnimationType.SlideFromLeft:
                    rectTransform.anchoredPosition = originalPosition + Vector3.left * config.slideDistance;
                    if (canvasGroup != null) canvasGroup.alpha = 0f;
                    
                    Sequence slideSeq = DOTween.Sequence();
                    slideSeq.Append(rectTransform.DOAnchorPos(originalPosition, config.showDuration).SetEase(Ease.OutCubic));
                    if (canvasGroup != null)
                    {
                        slideSeq.Join(canvasGroup.DOFade(1f, config.showDuration));
                    }
                    slideSeq.SetDelay(config.showDelay);
                    slideSeq.OnComplete(() => onComplete?.Invoke());
                    break;

                case UIAnimationType.SlideFromRight:
                    rectTransform.anchoredPosition = originalPosition + Vector3.right * config.slideDistance;
                    if (canvasGroup != null) canvasGroup.alpha = 0f;
                    
                    Sequence slideSeqR = DOTween.Sequence();
                    slideSeqR.Append(rectTransform.DOAnchorPos(originalPosition, config.showDuration).SetEase(Ease.OutCubic));
                    if (canvasGroup != null)
                    {
                        slideSeqR.Join(canvasGroup.DOFade(1f, config.showDuration));
                    }
                    slideSeqR.SetDelay(config.showDelay);
                    slideSeqR.OnComplete(() => onComplete?.Invoke());
                    break;

                case UIAnimationType.SlideFromTop:
                    rectTransform.anchoredPosition = originalPosition + Vector3.up * config.slideDistance;
                    if (canvasGroup != null) canvasGroup.alpha = 0f;
                    
                    Sequence slideSeqT = DOTween.Sequence();
                    slideSeqT.Append(rectTransform.DOAnchorPos(originalPosition, config.showDuration).SetEase(Ease.OutCubic));
                    if (canvasGroup != null)
                    {
                        slideSeqT.Join(canvasGroup.DOFade(1f, config.showDuration));
                    }
                    slideSeqT.SetDelay(config.showDelay);
                    slideSeqT.OnComplete(() => onComplete?.Invoke());
                    break;

                case UIAnimationType.SlideFromBottom:
                    rectTransform.anchoredPosition = originalPosition + Vector3.down * config.slideDistance;
                    if (canvasGroup != null) canvasGroup.alpha = 0f;
                    
                    Sequence slideSeqB = DOTween.Sequence();
                    slideSeqB.Append(rectTransform.DOAnchorPos(originalPosition, config.showDuration).SetEase(Ease.OutCubic));
                    if (canvasGroup != null)
                    {
                        slideSeqB.Join(canvasGroup.DOFade(1f, config.showDuration));
                    }
                    slideSeqB.SetDelay(config.showDelay);
                    slideSeqB.OnComplete(() => onComplete?.Invoke());
                    break;

                case UIAnimationType.Bounce:
                    rectTransform.localScale = Vector3.zero;
                    if (canvasGroup != null) canvasGroup.alpha = 0f;
                    
                    Sequence bounceSeq = DOTween.Sequence();
                    bounceSeq.Append(rectTransform.DOScale(Vector3.one, config.showDuration).SetEase(Ease.OutBounce));
                    if (canvasGroup != null)
                    {
                        bounceSeq.Join(canvasGroup.DOFade(1f, config.showDuration));
                    }
                    bounceSeq.SetDelay(config.showDelay);
                    bounceSeq.OnComplete(() => onComplete?.Invoke());
                    break;

                case UIAnimationType.Elastic:
                    rectTransform.localScale = Vector3.zero;
                    if (canvasGroup != null) canvasGroup.alpha = 0f;
                    
                    Sequence elasticSeq = DOTween.Sequence();
                    elasticSeq.Append(rectTransform.DOScale(Vector3.one, config.showDuration).SetEase(Ease.OutElastic));
                    if (canvasGroup != null)
                    {
                        elasticSeq.Join(canvasGroup.DOFade(1f, config.showDuration));
                    }
                    elasticSeq.SetDelay(config.showDelay);
                    elasticSeq.OnComplete(() => onComplete?.Invoke());
                    break;

                case UIAnimationType.Back:
                    rectTransform.localScale = Vector3.zero;
                    if (canvasGroup != null) canvasGroup.alpha = 0f;
                    
                    Sequence backSeq = DOTween.Sequence();
                    backSeq.Append(rectTransform.DOScale(Vector3.one, config.showDuration).SetEase(Ease.OutBack));
                    if (canvasGroup != null)
                    {
                        backSeq.Join(canvasGroup.DOFade(1f, config.showDuration));
                    }
                    backSeq.SetDelay(config.showDelay);
                    backSeq.OnComplete(() => onComplete?.Invoke());
                    break;
            }
        }

        /// <summary>
        /// 使用DOTween播放隐藏动画
        /// </summary>
        private static void PlayDOTweenHideAnimation(RectTransform rectTransform, CanvasGroup canvasGroup,
            UIAnimationConfig config, Action onComplete)
        {
            // 停止之前的动画
            rectTransform.DOKill();
            if (canvasGroup != null) canvasGroup.DOKill();

            Vector3 originalPosition = rectTransform.anchoredPosition;
            Vector3 originalScale = rectTransform.localScale;

            switch (config.hideAnimationType)
            {
                case UIAnimationType.Fade:
                    if (canvasGroup != null)
                    {
                        canvasGroup.DOFade(0f, config.hideDuration)
                            .SetDelay(config.hideDelay)
                            .OnComplete(() => onComplete?.Invoke());
                    }
                    else
                    {
                        onComplete?.Invoke();
                    }
                    break;

                case UIAnimationType.Scale:
                    Sequence seq = DOTween.Sequence();
                    seq.Append(rectTransform.DOScale(Vector3.one * config.scaleFrom, config.hideDuration).SetEase(Ease.InBack));
                    if (canvasGroup != null)
                    {
                        seq.Join(canvasGroup.DOFade(0f, config.hideDuration));
                    }
                    seq.SetDelay(config.hideDelay);
                    seq.OnComplete(() => onComplete?.Invoke());
                    break;

                case UIAnimationType.SlideFromLeft:
                case UIAnimationType.SlideFromRight:
                case UIAnimationType.SlideFromTop:
                case UIAnimationType.SlideFromBottom:
                    Vector3 targetPos = originalPosition;
                    if (config.hideAnimationType == UIAnimationType.SlideFromLeft)
                        targetPos += Vector3.left * config.slideDistance;
                    else if (config.hideAnimationType == UIAnimationType.SlideFromRight)
                        targetPos += Vector3.right * config.slideDistance;
                    else if (config.hideAnimationType == UIAnimationType.SlideFromTop)
                        targetPos += Vector3.up * config.slideDistance;
                    else if (config.hideAnimationType == UIAnimationType.SlideFromBottom)
                        targetPos += Vector3.down * config.slideDistance;

                    Sequence slideSeq = DOTween.Sequence();
                    slideSeq.Append(rectTransform.DOAnchorPos(targetPos, config.hideDuration).SetEase(Ease.InCubic));
                    if (canvasGroup != null)
                    {
                        slideSeq.Join(canvasGroup.DOFade(0f, config.hideDuration));
                    }
                    slideSeq.SetDelay(config.hideDelay);
                    slideSeq.OnComplete(() => onComplete?.Invoke());
                    break;

                default:
                    if (canvasGroup != null)
                    {
                        canvasGroup.DOFade(0f, config.hideDuration)
                            .SetDelay(config.hideDelay)
                            .OnComplete(() => onComplete?.Invoke());
                    }
                    else
                    {
                        onComplete?.Invoke();
                    }
                    break;
            }
        }


        /// <summary>
        /// 使用协程播放显示动画（DOTween不可用时）
        /// </summary>
        private static void PlayCoroutineShowAnimation(RectTransform rectTransform, CanvasGroup canvasGroup,
            UIAnimationConfig config, Action onComplete)
        {
            // 如果没有DOTween，使用简单的淡入动画
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                MonoBehaviour mono = rectTransform.GetComponent<MonoBehaviour>();
                if (mono != null)
                {
                    mono.StartCoroutine(FadeInCoroutine(canvasGroup, config.showDuration, config.showDelay, onComplete));
                }
                else
                {
                    onComplete?.Invoke();
                }
            }
            else
            {
                onComplete?.Invoke();
            }
        }

        /// <summary>
        /// 使用协程播放隐藏动画（DOTween不可用时）
        /// </summary>
        private static void PlayCoroutineHideAnimation(RectTransform rectTransform, CanvasGroup canvasGroup,
            UIAnimationConfig config, Action onComplete)
        {
            // 如果没有DOTween，使用简单的淡出动画
            if (canvasGroup != null)
            {
                MonoBehaviour mono = rectTransform.GetComponent<MonoBehaviour>();
                if (mono != null)
                {
                    mono.StartCoroutine(FadeOutCoroutine(canvasGroup, config.hideDuration, config.hideDelay, onComplete));
                }
                else
                {
                    onComplete?.Invoke();
                }
            }
            else
            {
                onComplete?.Invoke();
            }
        }

        private static IEnumerator FadeInCoroutine(CanvasGroup canvasGroup, float duration, float delay, Action onComplete)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                yield return null;
            }
            canvasGroup.alpha = 1f;
            onComplete?.Invoke();
        }

        private static IEnumerator FadeOutCoroutine(CanvasGroup canvasGroup, float duration, float delay, Action onComplete)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                yield return null;
            }
            canvasGroup.alpha = 0f;
            onComplete?.Invoke();
        }
    }
}

