using System;
using System.Globalization;
using System.Text;

namespace BasicMmethodExtensionClass.JavaScriptOperationHelper
{
    /// <summary>
    /// JavaScript帮助类
    /// </summary>
    public class JsHelper
    {
        /// <summary>
        /// 获取一段完整的JavaScript函数
        /// </summary>
        /// <param name="jsContent">函数主体</param>
        /// <param name="funParameters">函数参数</param>
        /// <returns>字符串表示的JavaScript函数</returns>
        public static string GetFunction(string jsContent, params string[] funParameters)
        {
            if (string.IsNullOrEmpty(jsContent))
            {
                throw new ArgumentNullException(jsContent);
            }
            if (funParameters == null)
            {
                throw new ArgumentNullException(nameof(funParameters));
            }
            var sb = new StringBuilder();
            sb.Append("function(");
            if (funParameters.Length > 0)
            {
                for (int i = 0, count = funParameters.Length; i < count; i++)
                {
                    sb.Append(funParameters[i]);
                    if (i != count - 1)
                    {
                        sb.Append(",");
                    }
                }
            }
            sb.Append("){");
            sb.Append(jsContent);
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// 获取延迟执行JavaScript脚本的字符串
        /// </summary>
        /// <param name="jsContent">需要执行的脚本内容</param>
        /// <param name="milliseconds">延迟毫秒数</param>
        /// <returns>延迟执行的客户端脚本</returns>
        public static string GetDeferScript(string jsContent, int milliseconds)
        {
            if (string.IsNullOrEmpty(jsContent))
            {
                throw new ArgumentNullException(jsContent);
            }
            return string.Format("Ext.defer({0},{1});", GetFunction(jsContent), milliseconds);
        }

        /// <summary>
        /// 获取延迟执行JavaScript脚本的字符串
        /// </summary>
        /// <param name="jsContent">需要执行的脚本内容</param>
        /// <param name="milliseconds">延迟毫秒数</param>
        /// <param name="scope">执行脚本时的函数上下文</param>
        /// <returns>延迟执行的客户端脚本</returns>
        public static string GetDeferScript(string jsContent, int milliseconds, string scope)
        {
            if (string.IsNullOrEmpty(jsContent))
            {
                throw new ArgumentNullException(jsContent);
            }
            if (string.IsNullOrEmpty(scope))
            {
                throw new ArgumentNullException(scope);
            }
            return string.Format("Ext.defer({0},{1},{2});", GetFunction(jsContent), milliseconds, scope);
        }
        
        /// <summary>
        /// 返回的是单引号括起来的字符串，用来作为JSON属性比较合适
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <returns>单引号括起来的字符串</returns>
        public static string Enquote(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
              throw new ArgumentNullException(s);
            }
            var sb = new StringBuilder();
            sb.Append('\'');
            for (int i = 0, len = s.Length; i < len; i++)
            {
                var c = s[i];
                switch (c)
                {
                    case '\\':
                    case '\'':
                    case '>':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            sb.Append('\'');
            return sb.ToString().Replace("</script>", @"<\/script>");
        }

        /// <summary>
        /// 将包含JavaScript代码块的字符串转换为可以使用的客户端脚本
        /// </summary>
        /// <param name="text">包含JavaScript代码块的字符串</param>
        /// <returns>转换后的客户端脚本</returns>
        public static string EnquoteWithScriptTag(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(text);
            }
            if (text.Contains("&lt;script&gt;"))
            {
                text = text.Replace("&lt;script&gt;", "<script>");
                text = text.Replace("&lt;/script&gt;", "</script>");
            }
            var splits = text.Split(new[] { "<script>" }, StringSplitOptions.None);
            var sb = new StringBuilder();
            foreach (var split in splits)
            {
                var subSplits = split.Split(new[] { "</script>" }, StringSplitOptions.None);
                if (subSplits.Length == 2)
                {
                    sb.AppendFormat("+{0}+", subSplits[0]);
                    sb.Append(Enquote(subSplits[1]));
                }
                else
                {
                    sb.Append(Enquote(subSplits[0]));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取字符串数组的脚本字符串形式
        /// </summary>
        /// <param name="values">字符串数组</param>
        /// <returns>字符串数组的脚本字符串</returns>
        public static string EnquoteStringArray(string[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            var sb = new StringBuilder();
            foreach (string value in values)
            {
                sb.AppendFormat("{0},", Enquote(value));
            }
            return string.Format("[{0}]", sb.ToString().TrimEnd(','));
        }


        /// <summary>
        /// 获取整形数组的脚本字符串形式
        /// </summary>
        /// <param name="values">整数数组</param>
        /// <returns>整形数组的脚本字符串</returns>
        public static string EnquoteIntArray(int[] values)
        {
            var sb = new StringBuilder();
            foreach (int value in values)
            {
                sb.AppendFormat("{0},", value);
            }
            return string.Format("[{0}]", sb.ToString().TrimEnd(','));
        }

        /// <summary>
        /// 将数字对象转化为字符串
        /// </summary>
        /// <param name="number">数字对象</param>
        /// <returns>字符串</returns>
        public static string NumberToString(object number)
        {
            if (number is float && float.IsNaN(((float)number)))
            {
                throw new ArgumentException("object must be a valid number", "number");
            }
            if (number is double && double.IsNaN(((double)number)))
            {
                throw new ArgumentException("object must be a valid number", "number");
            }
            var s = ((double)number).ToString(NumberFormatInfo.InvariantInfo).ToLower();
            if (s.IndexOf('e') >= 0 || s.IndexOf('.') <= 0) return s;
            while (s.EndsWith("0"))
            {
                s.Substring(0, s.Length - 1);
            }
            if (s.EndsWith("."))
            {
                s.Substring(0, s.Length - 1);
            }
            return s;
        }
    }
}
