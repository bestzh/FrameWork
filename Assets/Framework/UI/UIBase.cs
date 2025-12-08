using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.Animation;
using DG.Tweening;

namespace UI
{
    public abstract class UIBase : MonoBehaviour
    {
        [Header("UI Base Settings")]
        [SerializeField] protected Canvas canvas;
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected RectTransform rectTransform;
        
        [Header("Animation Settings")]
        [SerializeField] protected bool useAdvancedAnimation = false; // 是否使用高级动画
        [SerializeField] protected UIAnimationConfig animationConfig = new UIAnimationConfig();
        
        [Header("Legacy Animation Settings (向后兼容)")]
        [SerializeField] protected float showAnimationDuration = 0.3f;
        [SerializeField] protected float hideAnimationDuration = 0.2f;
        [SerializeField] protected AnimationCurve showCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] protected AnimationCurve hideCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        
        protected bool isInitialized = false;
        protected bool isVisible = false;
        protected Coroutine animationCoroutine;
        
        // 保存原始位置和缩放（用于动画）
        private Vector3 originalAnchoredPosition;
        private Vector3 originalLocalScale;
        private Quaternion originalLocalRotation;
        
        public bool IsVisible => isVisible;
        public bool IsInitialized => isInitialized;
        
        protected virtual void Awake()
        {
            if (canvas == null) canvas = GetComponent<Canvas>();
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            
            // 保存原始状态
            if (rectTransform != null)
            {
                originalAnchoredPosition = rectTransform.anchoredPosition;
                originalLocalScale = rectTransform.localScale;
                originalLocalRotation = rectTransform.localRotation;
            }
        }
        
        protected virtual void Start()
        {
            Initialize();
        }
        
        public virtual void Initialize()
        {
            if (isInitialized) return;
            
            OnInitialize();
            isInitialized = true;
        }
        
        public virtual void Show()
        {
            if (!isInitialized) Initialize();
            
            gameObject.SetActive(true);
            OnShow();
            
            // 恢复原始状态
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = originalAnchoredPosition;
                rectTransform.localScale = originalLocalScale;
                rectTransform.localRotation = originalLocalRotation;
            }
            
            if (useAdvancedAnimation && animationConfig != null)
            {
                // 使用高级动画系统
                isVisible = true;
                UIAnimationManager.PlayShowAnimation(rectTransform, canvasGroup, animationConfig, () =>
                {
                    animationCoroutine = null;
                });
            }
            else
            {
                // 使用传统动画（向后兼容）
                if (animationCoroutine != null)
                    StopCoroutine(animationCoroutine);
                    
                animationCoroutine = StartCoroutine(ShowAnimation());
            }
        }
        
        public virtual void Hide()
        {
            if (useAdvancedAnimation && animationConfig != null)
            {
                // 使用高级动画系统
                UIAnimationManager.PlayHideAnimation(rectTransform, canvasGroup, animationConfig, () =>
                {
                    isVisible = false;
                    gameObject.SetActive(false);
                    OnHide();
                    animationCoroutine = null;
                });
            }
            else
            {
                // 使用传统动画（向后兼容）
                if (animationCoroutine != null)
                    StopCoroutine(animationCoroutine);
                    
                animationCoroutine = StartCoroutine(HideAnimation());
            }
        }
        
        public virtual void ShowImmediate()
        {
            if (!isInitialized) Initialize();
            
            gameObject.SetActive(true);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
            isVisible = true;
            OnShow();
        }
        
        public virtual void HideImmediate()
        {
            gameObject.SetActive(false);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
            isVisible = false;
            OnHide();
        }
        
        protected virtual IEnumerator ShowAnimation()
        {
            if (canvasGroup == null)
            {
                // 如果没有CanvasGroup，直接显示
                isVisible = true;
                yield break;
            }
            
            canvasGroup.alpha = 0f;
            isVisible = true;
            
            float elapsedTime = 0f;
            while (elapsedTime < showAnimationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / showAnimationDuration;
                canvasGroup.alpha = showCurve.Evaluate(progress);
                yield return null;
            }
            
            canvasGroup.alpha = 1f;
            animationCoroutine = null;
        }
        
        protected virtual IEnumerator HideAnimation()
        {
            if (canvasGroup == null)
            {
                // 如果没有CanvasGroup，直接隐藏
                isVisible = false;
                gameObject.SetActive(false);
                OnHide();
                yield break;
            }
            
            float elapsedTime = 0f;
            while (elapsedTime < hideAnimationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / hideAnimationDuration;
                canvasGroup.alpha = hideCurve.Evaluate(progress);
                yield return null;
            }
            
            canvasGroup.alpha = 0f;
            isVisible = false;
            gameObject.SetActive(false);
            OnHide();
            animationCoroutine = null;
        }
        
        // 子类需要实现的方法
        protected virtual void OnInitialize() { }
        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
        
        // 销毁时清理
        protected virtual void OnDestroy()
        {
            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);
            
            // 清理DOTween动画（如果可用）
            try
            {
                if (rectTransform != null)
                    rectTransform.DOKill();
                if (canvasGroup != null)
                    canvasGroup.DOKill();
            }
            catch { }
        }
    }
} 