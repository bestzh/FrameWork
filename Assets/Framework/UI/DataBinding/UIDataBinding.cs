using System;
using UnityEngine;
using UnityEngine.UI;
using UI;

namespace UI.DataBinding
{
    /// <summary>
    /// UI数据绑定基类
    /// </summary>
    public abstract class UIDataBinding : MonoBehaviour
    {
        protected bool isBound = false;
        
        /// <summary>
        /// 绑定数据
        /// </summary>
        public abstract void Bind(object data);
        
        /// <summary>
        /// 解绑数据
        /// </summary>
        public virtual void Unbind()
        {
            isBound = false;
        }
        
        /// <summary>
        /// 加载Sprite（支持Addressables和Resources）
        /// </summary>
        protected Sprite LoadSprite(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            
            // 检查UIManager使用的加载器类型
            var loader = UIManager.Instance?.GetResourceLoader();
            if (loader != null)
            {
                // 使用UIManager的加载器加载
                return loader.Load<Sprite>(path);
            }
            
            // 降级到Resources加载
            return Resources.Load<Sprite>(path);
        }
    }
    
    /// <summary>
    /// 文本绑定
    /// </summary>
    public class TextBinding : UIDataBinding
    {
        [SerializeField] private Text targetText;
        [SerializeField] private string format = "{0}";
        
        private Func<object, string> valueConverter;
        
        public override void Bind(object data)
        {
            if (targetText == null)
            {
                targetText = GetComponent<Text>();
            }
            
            if (targetText != null && data != null)
            {
                string text = valueConverter != null ? valueConverter(data) : data.ToString();
                targetText.text = string.Format(format, text);
                isBound = true;
            }
        }
        
        public void SetConverter(Func<object, string> converter)
        {
            valueConverter = converter;
        }
    }
    
    /// <summary>
    /// 图片绑定
    /// </summary>
    public class ImageBinding : UIDataBinding
    {
        [SerializeField] private Image targetImage;
        [SerializeField] private string spritePathPrefix = "";
        
        private Func<object, Sprite> valueConverter;
        
        public override void Bind(object data)
        {
            if (targetImage == null)
            {
                targetImage = GetComponent<Image>();
            }
            
            if (targetImage != null && data != null)
            {
                if (valueConverter != null)
                {
                    targetImage.sprite = valueConverter(data);
                }
                else if (data is string path)
                {
                    // 使用统一的资源加载方法（支持Addressables和Resources）
                    Sprite sprite = LoadSprite(spritePathPrefix + path);
                    if (sprite != null)
                    {
                        targetImage.sprite = sprite;
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to load sprite: {spritePathPrefix + path}");
                    }
                }
                else if (data is Sprite sprite)
                {
                    targetImage.sprite = sprite;
                }
                
                isBound = true;
            }
        }
        
        public void SetConverter(Func<object, Sprite> converter)
        {
            valueConverter = converter;
        }
    }
    
    /// <summary>
    /// 按钮绑定
    /// </summary>
    public class ButtonBinding : UIDataBinding
    {
        [SerializeField] private Button targetButton;
        
        private Action<object> onClickAction;
        
        public override void Bind(object data)
        {
            if (targetButton == null)
            {
                targetButton = GetComponent<Button>();
            }
            
            if (targetButton != null)
            {
                targetButton.onClick.RemoveAllListeners();
                if (onClickAction != null)
                {
                    targetButton.onClick.AddListener(() => onClickAction(data));
                }
                
                isBound = true;
            }
        }
        
        public void SetOnClick(Action<object> action)
        {
            onClickAction = action;
        }
        
        public override void Unbind()
        {
            if (targetButton != null)
            {
                targetButton.onClick.RemoveAllListeners();
            }
            base.Unbind();
        }
    }
    
    /// <summary>
    /// Toggle绑定
    /// </summary>
    public class ToggleBinding : UIDataBinding
    {
        [SerializeField] private Toggle targetToggle;
        
        private Func<object, bool> valueGetter;
        private Action<object, bool> valueSetter;
        
        public override void Bind(object data)
        {
            if (targetToggle == null)
            {
                targetToggle = GetComponent<Toggle>();
            }
            
            if (targetToggle != null && data != null)
            {
                // 设置初始值
                if (valueGetter != null)
                {
                    targetToggle.isOn = valueGetter(data);
                }
                else if (data is bool boolValue)
                {
                    targetToggle.isOn = boolValue;
                }
                
                // 监听值变化
                targetToggle.onValueChanged.RemoveAllListeners();
                targetToggle.onValueChanged.AddListener((isOn) => {
                    if (valueSetter != null)
                    {
                        valueSetter(data, isOn);
                    }
                });
                
                isBound = true;
            }
        }
        
        public void SetValueGetter(Func<object, bool> getter)
        {
            valueGetter = getter;
        }
        
        public void SetValueSetter(Action<object, bool> setter)
        {
            valueSetter = setter;
        }
    }
    
    /// <summary>
    /// Slider绑定
    /// </summary>
    public class SliderBinding : UIDataBinding
    {
        [SerializeField] private Slider targetSlider;
        
        private Func<object, float> valueGetter;
        private Action<object, float> valueSetter;
        
        public override void Bind(object data)
        {
            if (targetSlider == null)
            {
                targetSlider = GetComponent<Slider>();
            }
            
            if (targetSlider != null && data != null)
            {
                // 设置初始值
                if (valueGetter != null)
                {
                    targetSlider.value = valueGetter(data);
                }
                else if (data is float floatValue)
                {
                    targetSlider.value = floatValue;
                }
                
                // 监听值变化
                targetSlider.onValueChanged.RemoveAllListeners();
                targetSlider.onValueChanged.AddListener((value) => {
                    if (valueSetter != null)
                    {
                        valueSetter(data, value);
                    }
                });
                
                isBound = true;
            }
        }
        
        public void SetValueGetter(Func<object, float> getter)
        {
            valueGetter = getter;
        }
        
        public void SetValueSetter(Action<object, float> setter)
        {
            valueSetter = setter;
        }
    }
    
    /// <summary>
    /// 集合绑定（用于列表）
    /// </summary>
    public class ListBinding : UIDataBinding
    {
        [SerializeField] private Transform itemContainer;
        [SerializeField] private GameObject itemPrefab;
        
        private System.Collections.IList dataList;
        private System.Action<GameObject, object> itemBinder;
        
        public override void Bind(object data)
        {
            if (data is System.Collections.IList list)
            {
                dataList = list;
                RefreshList();
                isBound = true;
            }
        }
        
        private void RefreshList()
        {
            if (itemContainer == null || itemPrefab == null || dataList == null)
                return;
            
            // 清除现有项
            foreach (Transform child in itemContainer)
            {
                Destroy(child.gameObject);
            }
            
            // 创建新项
            foreach (var itemData in dataList)
            {
                GameObject itemObj = Instantiate(itemPrefab, itemContainer);
                itemBinder?.Invoke(itemObj, itemData);
            }
        }
        
        public void SetItemBinder(System.Action<GameObject, object> binder)
        {
            itemBinder = binder;
        }
        
        public override void Unbind()
        {
            if (itemContainer != null)
            {
                foreach (Transform child in itemContainer)
                {
                    Destroy(child.gameObject);
                }
            }
            base.Unbind();
        }
    }
}

