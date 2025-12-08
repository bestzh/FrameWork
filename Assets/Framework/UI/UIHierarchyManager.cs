using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// UI层级管理器，管理Canvas排序和UI分组
    /// </summary>
    public class UIHierarchyManager : MonoBehaviour
    {
        private static UIHierarchyManager instance;
        public static UIHierarchyManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("UIHierarchyManager");
                    instance = go.AddComponent<UIHierarchyManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }
        
        [Header("Layer Settings")]
        [SerializeField] private int baseSortingOrder = 0;
        [SerializeField] private int layerStep = 10; // 每层间隔
        
        // UI层级定义
        public enum UILayer
        {
            Background = 0,   // 背景层
            Normal = 1,        // 普通UI层
            Popup = 2,        // 弹窗层
            Dialog = 3,       // 对话框层
            Top = 4,          // 顶层
            Loading = 5       // 加载层
        }
        
        // 层级对应的排序值
        private Dictionary<UILayer, int> layerSortingOrders = new Dictionary<UILayer, int>();
        
        // 当前使用的排序值
        private Dictionary<UILayer, int> currentSortingOrders = new Dictionary<UILayer, int>();
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void Initialize()
        {
            // 初始化层级排序值
            foreach (UILayer layer in System.Enum.GetValues(typeof(UILayer)))
            {
                int sortingOrder = baseSortingOrder + (int)layer * layerStep;
                layerSortingOrders[layer] = sortingOrder;
                currentSortingOrders[layer] = sortingOrder;
            }
        }
        
        /// <summary>
        /// 设置UI的层级
        /// </summary>
        public void SetUILayer(UIBase ui, UILayer layer)
        {
            // 先检查UI或其父对象是否已有Canvas
            Canvas canvas = ui.GetComponent<Canvas>();
            bool isSceneCanvas = false;
            
            if (canvas == null)
            {
                // 检查父对象是否有Canvas（包括场景中的Canvas）
                canvas = ui.GetComponentInParent<Canvas>();
                if (canvas != null && canvas.transform != ui.transform)
                {
                    isSceneCanvas = true; // Canvas在父对象上，可能是场景Canvas
                }
            }
            
            if (canvas == null)
            {
                // 如果都没有，才添加新的Canvas（UI预制体自带Canvas的情况）
                canvas = ui.gameObject.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                
                // 必须添加GraphicRaycaster才能接收点击事件
                if (ui.GetComponent<GraphicRaycaster>() == null)
                {
                    ui.gameObject.AddComponent<GraphicRaycaster>();
                }
                
                // 设置排序值（新Canvas可以设置）
                canvas.sortingOrder = GetNextSortingOrder(layer);
            }
            else
            {
                // 如果已有Canvas（包括场景Canvas），确保有GraphicRaycaster
                if (canvas.GetComponent<GraphicRaycaster>() == null)
                {
                    canvas.gameObject.AddComponent<GraphicRaycaster>();
                }
                
                // 如果Canvas在父对象上（场景Canvas），不修改sortingOrder
                // 因为修改场景Canvas的sortingOrder会影响所有子UI
                if (isSceneCanvas)
                {
                    // UI在场景Canvas下，通过设置SiblingIndex来管理显示顺序
                    // 或者保持原样，使用场景Canvas的默认排序
                    // 注意：这种方式层级管理有限，如果需要独立层级，UI应该有自己的Canvas
                    int targetIndex = GetLayerSiblingIndex(layer);
                    if (ui.transform.parent == canvas.transform)
                    {
                        // 确保UI在正确的显示顺序
                        ui.transform.SetSiblingIndex(Mathf.Min(targetIndex, canvas.transform.childCount - 1));
                    }
                }
                else
                {
                    // UI自己的Canvas，可以设置排序值
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = GetNextSortingOrder(layer);
                }
            }
            
            // 确保EventSystem存在
            EnsureEventSystem();
        }
        
        /// <summary>
        /// 获取层级的SiblingIndex（用于场景Canvas下的UI排序）
        /// </summary>
        private int GetLayerSiblingIndex(UILayer layer)
        {
            // 返回层级对应的索引，数字越大越靠后显示
            return (int)layer * 100;
        }
        
        /// <summary>
        /// 确保EventSystem存在
        /// </summary>
        private void EnsureEventSystem()
        {
            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }
        
        /// <summary>
        /// 获取下一个排序值
        /// </summary>
        private int GetNextSortingOrder(UILayer layer)
        {
            if (!currentSortingOrders.ContainsKey(layer))
            {
                currentSortingOrders[layer] = layerSortingOrders[layer];
            }
            
            int sortingOrder = currentSortingOrders[layer];
            
            // 防止溢出：如果接近int最大值，重置该层级
            if (sortingOrder >= int.MaxValue - 100)
            {
                Debug.LogWarning($"UI层级 {layer} 的排序值接近溢出，正在重置");
                currentSortingOrders[layer] = layerSortingOrders[layer];
                sortingOrder = currentSortingOrders[layer];
            }
            
            currentSortingOrders[layer] += 1; // 同层级的UI递增
            
            return sortingOrder;
        }
        
        /// <summary>
        /// 重置层级排序值
        /// </summary>
        public void ResetLayer(UILayer layer)
        {
            if (currentSortingOrders.ContainsKey(layer))
            {
                currentSortingOrders[layer] = layerSortingOrders[layer];
            }
        }
        
        /// <summary>
        /// 创建模态遮罩
        /// </summary>
        public GameObject CreateModalMask(Transform parent, System.Action onMaskClick = null)
        {
            GameObject mask = new GameObject("ModalMask");
            mask.transform.SetParent(parent, false);
            
            RectTransform rectTransform = mask.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            
            Image image = mask.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0.5f); // 半透明黑色
            
            Button button = mask.AddComponent<Button>();
            button.onClick.AddListener(() => onMaskClick?.Invoke());
            
            // 设置遮罩在最底层
            Canvas canvas = mask.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                mask.transform.SetAsFirstSibling();
            }
            
            return mask;
        }
    }
}

