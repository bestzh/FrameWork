using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;

namespace UI.Editor
{
    /// <summary>
    /// UI代码生成工具
    /// </summary>
    public class UIGenerator
    {
        private const string UI_SCRIPT_PATH = "Assets/Scripts/UI/";
        private const string UI_NAMESPACE = "";
        
        /// <summary>
        /// UI元素信息
        /// </summary>
        public class UIElementInfo
        {
            public string name;
            public string fieldName;
            public Type componentType;
            public bool isGameObject;
            
            public UIElementInfo(string name, Type componentType, bool isGameObject = false)
            {
                this.name = name;
                this.componentType = componentType;
                this.isGameObject = isGameObject;
                this.fieldName = ConvertToFieldName(name);
            }
            
            private string ConvertToFieldName(string name)
            {
                // 移除常见后缀
                name = name.Replace("_Btn", "").Replace("_Button", "");
                name = name.Replace("_Text", "").Replace("_Label", "");
                name = name.Replace("_Img", "").Replace("_Image", "");
                name = name.Replace("_Toggle", "");
                name = name.Replace("_Slider", "");
                
                // 转换为PascalCase
                if (string.IsNullOrEmpty(name)) return name;
                
                // 处理下划线命名
                if (name.Contains("_"))
                {
                    string[] parts = name.Split('_');
                    StringBuilder sb = new StringBuilder();
                    foreach (var part in parts)
                    {
                        if (!string.IsNullOrEmpty(part))
                        {
                            sb.Append(char.ToUpper(part[0]));
                            if (part.Length > 1)
                            {
                                sb.Append(part.Substring(1));
                            }
                        }
                    }
                    return sb.ToString();
                }
                
                // 首字母大写
                return char.ToUpper(name[0]) + (name.Length > 1 ? name.Substring(1) : "");
            }
        }
        
        /// <summary>
        /// 从预制体生成UI代码
        /// </summary>
        [MenuItem("Tools/UI/生成UI代码（选择预制体）")]
        public static void GenerateUICodeFromSelection()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                EditorUtility.DisplayDialog("错误", "请先选择一个UI预制体", "确定");
                return;
            }
            
            GenerateUICode(selected);
        }
        
        /// <summary>
        /// 从预制体生成UI代码
        /// </summary>
        public static void GenerateUICode(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogError("预制体为空");
                return;
            }
            
            string className = prefab.name;
            if (className.EndsWith("UI"))
            {
                className = className;
            }
            else
            {
                className = className + "UI";
            }
            
            // 收集UI元素
            List<UIElementInfo> elements = CollectUIElements(prefab);
            
            // 生成代码
            string code = GenerateCode(className, elements);
            
            // 保存文件
            string filePath = Application.dataPath + "/" + UI_SCRIPT_PATH.Replace("Assets/", "") + className + ".cs";
            Framework.Editor.FileUtilTool.CreateFolderForFile(filePath);
            Framework.Editor.FileUtilTool.WriteFile(filePath, code);
            
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("成功", $"UI代码已生成：{filePath}", "确定");
            Debug.Log($"UI代码已生成：{filePath}");
        }
        
        /// <summary>
        /// 收集UI元素
        /// </summary>
        public static List<UIElementInfo> CollectUIElements(GameObject root)
        {
            List<UIElementInfo> elements = new List<UIElementInfo>();
            HashSet<string> processedNames = new HashSet<string>();
            
            // 遍历所有子对象
            CollectElementsRecursive(root.transform, elements, processedNames);
            
            return elements;
        }
        
        /// <summary>
        /// 递归收集UI元素
        /// </summary>
        private static void CollectElementsRecursive(Transform parent, List<UIElementInfo> elements, HashSet<string> processedNames)
        {
            foreach (Transform child in parent)
            {
                // 跳过某些对象（如遮罩、背景等）
                if (ShouldSkipObject(child.name))
                {
                    CollectElementsRecursive(child, elements, processedNames);
                    continue;
                }
                
                // 检查常见UI组件
                UIElementInfo element = null;
                
                if (child.GetComponent<Button>() != null)
                {
                    element = new UIElementInfo(child.name, typeof(Button));
                }
                else if (child.GetComponent<Text>() != null)
                {
                    element = new UIElementInfo(child.name, typeof(Text));
                }
                else if (child.GetComponent<Image>() != null)
                {
                    element = new UIElementInfo(child.name, typeof(Image));
                }
                else if (child.GetComponent<Toggle>() != null)
                {
                    element = new UIElementInfo(child.name, typeof(Toggle));
                }
                else if (child.GetComponent<Slider>() != null)
                {
                    element = new UIElementInfo(child.name, typeof(Slider));
                }
                else if (child.GetComponent<InputField>() != null)
                {
                    element = new UIElementInfo(child.name, typeof(InputField));
                }
                else if (child.GetComponent<ScrollRect>() != null)
                {
                    element = new UIElementInfo(child.name, typeof(ScrollRect));
                }
                else if (child.GetComponent<Scrollbar>() != null)
                {
                    element = new UIElementInfo(child.name, typeof(Scrollbar));
                }
                else if (child.GetComponent<Dropdown>() != null)
                {
                    element = new UIElementInfo(child.name, typeof(Dropdown));
                }
                else if (child.childCount > 0)
                {
                    // 如果有子对象，可能是容器，作为GameObject处理
                    element = new UIElementInfo(child.name, typeof(GameObject), true);
                }
                
                if (element != null && !processedNames.Contains(element.fieldName))
                {
                    elements.Add(element);
                    processedNames.Add(element.fieldName);
                }
                
                // 递归处理子对象
                CollectElementsRecursive(child, elements, processedNames);
            }
        }
        
        /// <summary>
        /// 判断是否跳过该对象
        /// </summary>
        private static bool ShouldSkipObject(string name)
        {
            string lowerName = name.ToLower();
            return lowerName.Contains("mask") || 
                   lowerName.Contains("background") || 
                   lowerName.Contains("bg") ||
                   lowerName.StartsWith("_");
        }
        
        /// <summary>
        /// 生成代码
        /// </summary>
        private static string GenerateCode(string className, List<UIElementInfo> elements)
        {
            StringBuilder sb = new StringBuilder();
            
            // Using语句
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using UI;");
            sb.AppendLine();
            
            // 类定义
            if (!string.IsNullOrEmpty(UI_NAMESPACE))
            {
                sb.AppendLine($"namespace {UI_NAMESPACE}");
                sb.AppendLine("{");
            }
            
            sb.AppendLine($"public class {className} : UIBase");
            sb.AppendLine("{");
            
            // 字段声明
            sb.AppendLine("    [Header(\"UI Elements\")]");
            foreach (var element in elements)
            {
                string typeName = element.isGameObject ? "GameObject" : element.componentType.Name;
                sb.AppendLine($"    public {typeName} {element.fieldName};");
            }
            sb.AppendLine();
            
            // Start方法
            sb.AppendLine("    protected override void Start()");
            sb.AppendLine("    {");
            sb.AppendLine("        base.Start();");
            sb.AppendLine();
            
            // 按钮绑定
            var buttons = elements.Where(e => e.componentType == typeof(Button));
            foreach (var button in buttons)
            {
                string methodName = $"On{button.fieldName}Click";
                sb.AppendLine($"        {button.fieldName}.onClick.AddListener({methodName});");
            }
            
            if (buttons.Any())
            {
                sb.AppendLine();
            }
            sb.AppendLine("    }");
            sb.AppendLine();
            
            // 事件处理方法
            foreach (var button in buttons)
            {
                string methodName = $"On{button.fieldName}Click";
                sb.AppendLine($"    void {methodName}()");
                sb.AppendLine("    {");
                sb.AppendLine($"        Debug.Log(\"{button.fieldName} Button Clicked\");");
                sb.AppendLine("        // TODO: 添加按钮点击逻辑");
                sb.AppendLine("    }");
                sb.AppendLine();
            }
            
            // Show方法
            sb.AppendLine("    public override void Show()");
            sb.AppendLine("    {");
            sb.AppendLine("        base.Show();");
            sb.AppendLine("        // TODO: 添加显示时的逻辑");
            sb.AppendLine("    }");
            sb.AppendLine();
            
            // Hide方法
            sb.AppendLine("    public override void Hide()");
            sb.AppendLine("    {");
            sb.AppendLine("        base.Hide();");
            sb.AppendLine("        // TODO: 添加隐藏时的逻辑");
            sb.AppendLine("    }");
            
            sb.AppendLine("}");
            
            if (!string.IsNullOrEmpty(UI_NAMESPACE))
            {
                sb.AppendLine("}");
            }
            
            return sb.ToString();
        }
        
        /// <summary>
        /// 批量生成UI代码（从Resources/UI目录）
        /// </summary>
        [MenuItem("Tools/UI/批量生成UI代码")]
        public static void BatchGenerateUICode()
        {
            string uiPath = "Assets/Resources/UI";
            if (!Directory.Exists(uiPath))
            {
                EditorUtility.DisplayDialog("错误", $"UI目录不存在：{uiPath}", "确定");
                return;
            }
            
            string[] prefabPaths = Directory.GetFiles(uiPath, "*.prefab", SearchOption.AllDirectories);
            
            if (prefabPaths.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "未找到UI预制体", "确定");
                return;
            }
            
            int generatedCount = 0;
            float progress = 0f;
            
            foreach (string prefabPath in prefabPaths)
            {
                progress += 1f / prefabPaths.Length;
                EditorUtility.DisplayProgressBar("生成UI代码", $"正在处理：{Path.GetFileName(prefabPath)}", progress);
                
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab != null)
                {
                    try
                    {
                        GenerateUICode(prefab);
                        generatedCount++;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"生成 {prefab.name} 的代码失败：{e.Message}");
                    }
                }
            }
            
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("完成", $"成功生成 {generatedCount}/{prefabPaths.Length} 个UI代码文件", "确定");
        }
    }
}

