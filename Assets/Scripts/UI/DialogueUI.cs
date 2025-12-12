using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    /// <summary>
    /// 对话UI - 显示NPC对话内容
    /// </summary>
    public class DialogueUI : UIBase
    {
        [Header("UI组件引用")]
        [SerializeField] private TextMeshProUGUI npcNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Image npcAvatarImage;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject dialoguePanel;
        
        [Header("设置")]
        [SerializeField] private float typingSpeed = 0.05f; // 打字机效果速度
        [SerializeField] private bool useTypingEffect = true; // 是否使用打字机效果
        
        private string currentDialogue;
        private Coroutine typingCoroutine;
        private System.Action onDialogueClosed;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            // 绑定关闭按钮
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(CloseDialogue);
            }
            
            // 默认隐藏
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }
        
        protected override void OnShow()
        {
            base.OnShow();
            
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }
        }
        
        protected override void OnHide()
        {
            base.OnHide();
            
            // 停止打字机效果
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// 显示对话
        /// </summary>
        /// <param name="npcName">NPC名称</param>
        /// <param name="dialogue">对话内容</param>
        /// <param name="avatar">NPC头像（可选）</param>
        /// <param name="onClosed">对话关闭回调</param>
        public void ShowDialogue(string npcName, string dialogue, Sprite avatar = null, System.Action onClosed = null)
        {
            // 设置NPC名称
            if (npcNameText != null)
            {
                npcNameText.text = npcName;
            }
            
            // 保存对话内容和回调
            currentDialogue = dialogue;
            onDialogueClosed = onClosed;
            
            // 设置头像
            if (npcAvatarImage != null)
            {
                if (avatar != null)
                {
                    npcAvatarImage.sprite = avatar;
                    npcAvatarImage.gameObject.SetActive(true);
                }
                else
                {
                    npcAvatarImage.gameObject.SetActive(false);
                }
            }
            
            // 显示对话文本
            if (useTypingEffect && !string.IsNullOrEmpty(dialogue))
            {
                // 使用打字机效果
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }
                typingCoroutine = StartCoroutine(TypeText(dialogue));
            }
            else
            {
                // 直接显示全部文本
                if (dialogueText != null)
                {
                    dialogueText.text = dialogue;
                }
            }
        }
        
        /// <summary>
        /// 打字机效果协程
        /// </summary>
        private System.Collections.IEnumerator TypeText(string text)
        {
            if (dialogueText == null) yield break;
            
            dialogueText.text = "";
            
            foreach (char c in text)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
            
            typingCoroutine = null;
        }
        
        /// <summary>
        /// 关闭对话
        /// </summary>
        public void CloseDialogue()
        {
            // 如果正在打字，直接显示全部文本
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
                if (dialogueText != null && !string.IsNullOrEmpty(currentDialogue))
                {
                    dialogueText.text = currentDialogue;
                }
                return;
            }
            
            // 调用回调
            onDialogueClosed?.Invoke();
            
            // 隐藏UI
            Hide();
        }
        
        /// <summary>
        /// 检查是否可以关闭（用于外部调用，比如按ESC键）
        /// </summary>
        public bool CanClose()
        {
            return typingCoroutine == null;
        }
        
        /// <summary>
        /// 跳过打字机效果，直接显示全部文本
        /// </summary>
        public void SkipTyping()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
                if (dialogueText != null && !string.IsNullOrEmpty(currentDialogue))
                {
                    dialogueText.text = currentDialogue;
                }
            }
        }
    }
}

