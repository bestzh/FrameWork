using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UI.Editor
{
    /// <summary>
    /// UI代码生成器窗口
    /// </summary>
    public class UIGeneratorWindow : EditorWindow
    {
        private GameObject selectedPrefab;
        private string className = "";
        private bool generateDataBinding = false;
        private bool generateEventHandlers = true;
        private bool generateQueryCode = false;
        private Vector2 scrollPosition;
        
        [MenuItem("Tools/UI/UI代码生成器")]
        public static void ShowWindow()
        {
            UIGeneratorWindow window = GetWindow<UIGeneratorWindow>("UI代码生成器");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("UI代码生成器", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // 选择预制体
            EditorGUILayout.LabelField("选择UI预制体", EditorStyles.label);
            GameObject newPrefab = EditorGUILayout.ObjectField("预制体", selectedPrefab, typeof(GameObject), false) as GameObject;
            if (newPrefab != selectedPrefab)
            {
                selectedPrefab = newPrefab;
                if (selectedPrefab != null)
                {
                    className = selectedPrefab.name;
                    if (!className.EndsWith("UI"))
                    {
                        className = className + "UI";
                    }
                }
            }
            
            EditorGUILayout.Space();
            
            // 类名设置
            EditorGUILayout.LabelField("生成设置", EditorStyles.label);
            className = EditorGUILayout.TextField("类名", className);
            
            EditorGUILayout.Space();
            
            // 生成选项
            EditorGUILayout.LabelField("生成选项", EditorStyles.label);
            generateEventHandlers = EditorGUILayout.Toggle("生成事件处理方法", generateEventHandlers);
            generateDataBinding = EditorGUILayout.Toggle("生成数据绑定代码", generateDataBinding);
            generateQueryCode = EditorGUILayout.Toggle("生成查询代码（UIQuery）", generateQueryCode);
            
            EditorGUILayout.Space();
            
            // 预览UI元素
            if (selectedPrefab != null)
            {
                EditorGUILayout.LabelField("UI元素预览", EditorStyles.label);
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
                
                var elements = CollectUIElements(selectedPrefab);
                foreach (var element in elements)
                {
                    string typeName = element.isGameObject ? "GameObject" : element.componentType.Name;
                    EditorGUILayout.LabelField($"  {typeName} {element.fieldName} ({element.name})");
                }
                
                EditorGUILayout.EndScrollView();
            }
            
            EditorGUILayout.Space();
            
            // 生成按钮
            GUI.enabled = selectedPrefab != null && !string.IsNullOrEmpty(className);
            if (GUILayout.Button("生成代码", GUILayout.Height(30)))
            {
                GenerateCode();
            }
            GUI.enabled = true;
            
            EditorGUILayout.Space();
            
            // 批量生成按钮
            if (GUILayout.Button("批量生成（Resources/UI目录）", GUILayout.Height(25)))
            {
                UIGenerator.BatchGenerateUICode();
            }
        }
        
        private void GenerateCode()
        {
            if (selectedPrefab == null || string.IsNullOrEmpty(className))
            {
                EditorUtility.DisplayDialog("错误", "请选择预制体并输入类名", "确定");
                return;
            }
            
            var elements = CollectUIElements(selectedPrefab);
            string code = GenerateAdvancedCode(className, elements);
            
            string filePath = Application.dataPath + "/Scripts/UI/" + className + ".cs";
            Framework.Editor.FileUtilTool.CreateFolderForFile(filePath);
            Framework.Editor.FileUtilTool.WriteFile(filePath, code);
            
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("成功", $"UI代码已生成：{filePath}", "确定");
            Debug.Log($"UI代码已生成：{filePath}");
        }
        
        private List<UIGenerator.UIElementInfo> CollectUIElements(GameObject root)
        {
            return UIGenerator.CollectUIElements(root);
        }
        
        private string GenerateAdvancedCode(string className, List<UIGenerator.UIElementInfo> elements)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            // Using语句
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using UI;");
            // UIQuery已在UI命名空间中
            sb.AppendLine();
            
            // 类定义
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
            
            // 如果使用查询代码，生成查询方法
            if (generateQueryCode)
            {
                sb.AppendLine("    protected override void Awake()");
                sb.AppendLine("    {");
                sb.AppendLine("        base.Awake();");
                sb.AppendLine("        // 使用UIQuery自动查找UI元素");
                foreach (var element in elements)
                {
                    string typeName = element.isGameObject ? "GameObject" : element.componentType.Name;
                    if (typeName != "GameObject")
                    {
                        sb.AppendLine($"        {element.fieldName} = UIQuery.Q<{typeName}>(gameObject, \"{element.name}\") ?? {element.fieldName};");
                    }
                    else
                    {
                        sb.AppendLine($"        {element.fieldName} = UIQuery.Q<Transform>(gameObject, \"{element.name}\")?.gameObject ?? {element.fieldName};");
                    }
                }
                sb.AppendLine("    }");
                sb.AppendLine();
            }
            
            // Start方法
            sb.AppendLine("    protected override void Start()");
            sb.AppendLine("    {");
            sb.AppendLine("        base.Start();");
            sb.AppendLine();
            
            // 按钮绑定
            var buttons = System.Linq.Enumerable.Where(elements, e => e.componentType == typeof(Button));
            foreach (var button in buttons)
            {
                string methodName = $"On{button.fieldName}Click";
                sb.AppendLine($"        if ({button.fieldName} != null)");
                sb.AppendLine($"            {button.fieldName}.onClick.AddListener({methodName});");
            }
            
            if (System.Linq.Enumerable.Any(buttons))
            {
                sb.AppendLine();
            }
            
            // 数据绑定代码
            if (generateDataBinding)
            {
                sb.AppendLine("        // 数据绑定初始化");
                var texts = System.Linq.Enumerable.Where(elements, e => e.componentType == typeof(Text));
                foreach (var text in texts)
                {
                    sb.AppendLine($"        // {text.fieldName} 可以添加 TextBinding 组件");
                }
            }
            
            sb.AppendLine("    }");
            sb.AppendLine();
            
            // 事件处理方法
            if (generateEventHandlers)
            {
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
            
            return sb.ToString();
        }
    }
}

