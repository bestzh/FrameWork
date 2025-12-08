using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace UI.Editor
{
    /// <summary>
    /// UI代码生成模板
    /// </summary>
    public static class UIGeneratorTemplate
    {
        /// <summary>
        /// 获取基础UI类模板
        /// </summary>
        public static string GetBasicUITemplate(string className, string fields, string eventHandlers, string showHideMethods)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using UI;");
            sb.AppendLine();
            sb.AppendLine($"public class {className} : UIBase");
            sb.AppendLine("{");
            sb.AppendLine(fields);
            sb.AppendLine();
            sb.AppendLine("    protected override void Start()");
            sb.AppendLine("    {");
            sb.AppendLine("        base.Start();");
            sb.AppendLine("        // TODO: 添加初始化代码");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine(eventHandlers);
            sb.AppendLine(showHideMethods);
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        /// <summary>
        /// 获取数据绑定模板
        /// </summary>
        public static string GetDataBindingTemplate(string className, string bindings)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using UI;");
            sb.AppendLine("using UI.DataBinding;");
            sb.AppendLine();
            sb.AppendLine($"public class {className} : UIBase");
            sb.AppendLine("{");
            sb.AppendLine(bindings);
            sb.AppendLine();
            sb.AppendLine("    protected override void Start()");
            sb.AppendLine("    {");
            sb.AppendLine("        base.Start();");
            sb.AppendLine("        // 初始化数据绑定");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public override void Show()");
            sb.AppendLine("    {");
            sb.AppendLine("        base.Show();");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public override void Hide()");
            sb.AppendLine("    {");
            sb.AppendLine("        base.Hide();");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
    }
}

