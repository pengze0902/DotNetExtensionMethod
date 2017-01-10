using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace BasicMmethodExtensionWeb.Helper
{
    public class HtmlHelper
    {
        /// <summary>
        /// 过滤html标签
        /// </summary>
        /// <param name="stringToStrip"></param>
        /// <returns></returns>
        public static string StripHtml(string stringToStrip)
        {
            stringToStrip = Regex.Replace(stringToStrip, "</p(?:\\s*)>(?:\\s*)<p(?:\\s*)>", "\n\n", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            stringToStrip = Regex.Replace(stringToStrip, " ", "\n", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            stringToStrip = Regex.Replace(stringToStrip, "\"", "''", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            stringToStrip = StripHtmlXmlTags(stringToStrip);
            return stringToStrip;
        }
        private static string StripHtmlXmlTags(string content)
        {
            return Regex.Replace(content, "<[^>]+>", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

      
        /// <summary>
        /// HTML转行成TEXT
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public static string HtmlToTxt(string strHtml)
        {
            if (string.IsNullOrEmpty(strHtml))
            {
                throw new ArgumentNullException(strHtml);
            }
            string[] aryReg ={
           @"<script[^>]*?>.*?</script>",
           @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>",
           @"([\r\n])[\s]+",
           @"&(quot|#34);",
           @"&(amp|#38);",
           @"&(lt|#60);",
           @"&(gt|#62);", 
           @"&(nbsp|#160);", 
           @"&(iexcl|#161);",
           @"&(cent|#162);",
           @"&(pound|#163);",
           @"&(copy|#169);",
           @"&#(\d+);",
           @"-->",
           @"<!--.*\n"
           };
            var strOutput = strHtml;
            for (var i = 0; i < aryReg.Length; i++)
            {
                var regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, string.Empty);
            }
            strOutput.Replace("<", "");
            strOutput.Replace(">", "");
            strOutput.Replace("\r\n", "");
            return strOutput;
        }


        /// <summary>
        /// 字符串字符处理,TXT代码转换成HTML格式
        /// </summary>
        /// <param name="input">等待处理的字符串</param>
        /// <returns>处理后的字符串</returns>
        /// 把TXT代码转换成HTML格式
        public static string ToHtml(string input)
        {
            var sb = new StringBuilder(input);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\r\n", "<br />");
            sb.Replace("\n", "<br />");
            sb.Replace("\t", " ");
            //sb.Replace(" ", "&nbsp;");
            return sb.ToString();
        }


        ///<summary>   
        ///清除HTML标记   
        ///</summary>   
        ///<param name="htmlstring">包括HTML的源码</param>   
        ///<returns>已经去除后的文字</returns>   
        public static string NoHtml(string htmlstring)
        {
            //删除脚本   
            htmlstring = Regex.Replace(htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Regex regex = new Regex("<.+?>", RegexOptions.IgnoreCase);
            htmlstring = regex.Replace(htmlstring, "");
            htmlstring = Regex.Replace(htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            htmlstring.Replace("<", "");
            htmlstring.Replace(">", "");
            htmlstring.Replace("\r\n", "");
            return htmlstring;
        }

        /// <summary>
        /// 获取页面的链接
        /// </summary>
        /// <param name="htmlCode">html源代码</param>
        /// <returns></returns>
        public string GetHref(string htmlCode)
        {
            if (string.IsNullOrEmpty(htmlCode))
            {
                throw new ArgumentNullException(htmlCode);
            }
            string matchVale = string.Empty;
            string Reg = @"(h|H)(r|R)(e|E)(f|F) *= *('|"")?((\w|\\|\/|\.|:|-|_)+)[\S]*";
            foreach (Match m in Regex.Matches(htmlCode, Reg))
            {
                matchVale += (m.Value).ToLower().Replace("href=", "").Trim() + "|";
            }
            return matchVale;
        }


        /// <summary>
        /// 匹配页面的图片地址
        /// </summary>
        /// <param name="htmlCode"></param>
        /// <param name="imgHttp">要补充的http://路径信息</param>
        /// <returns></returns>
        public string GetImgSrc(string htmlCode, string imgHttp)
        {
            if (string.IsNullOrEmpty(htmlCode))
            {
                throw new ArgumentNullException(htmlCode);
            }
            if (string.IsNullOrEmpty(imgHttp))
            {
                throw new ArgumentNullException(imgHttp);
            }
            string matchVale = string.Empty;
            string Reg = @"<img.+?>";
            foreach (Match m in Regex.Matches(htmlCode.ToLower(), Reg))
            {
                matchVale += GetImg((m.Value).ToLower().Trim(), imgHttp) + "|";
            }

            return matchVale;
        }

        /// <summary>
        /// 匹配<img src="" />中的图片路径实际链接
        /// </summary>
        /// <param name="imgString"><img src="" />字符串</param>
        /// <param name="imgHttp"></param>
        public string GetImg(string imgString, string imgHttp)
        {
            if (string.IsNullOrEmpty(imgHttp))
            {
                throw new ArgumentNullException(imgHttp);
            }
            if (string.IsNullOrEmpty(imgString))
            {
                throw new ArgumentNullException(imgString);
            }
            string matchVale = string.Empty;
            const string Reg = @"src=.+\.(bmp|jpg|gif|png|)";
            foreach (Match m in Regex.Matches(imgString.ToLower(), Reg))
            {
                matchVale += (m.Value).ToLower().Trim().Replace("src=", "");
            }
            if (matchVale.IndexOf(".net", StringComparison.Ordinal) != -1 || 
                matchVale.IndexOf(".com", StringComparison.Ordinal) != -1 || 
                matchVale.IndexOf(".org", StringComparison.Ordinal) != -1 || 
                matchVale.IndexOf(".cn", StringComparison.Ordinal) != -1 || 
                matchVale.IndexOf(".cc", StringComparison.Ordinal) != -1 || 
                matchVale.IndexOf(".info", StringComparison.Ordinal) != -1 || 
                matchVale.IndexOf(".biz", StringComparison.Ordinal) != -1 || 
                matchVale.IndexOf(".tv", StringComparison.Ordinal) != -1)
                return (matchVale);
            return (imgHttp + matchVale);
        }

        /// <summary>
        /// 压缩HTML输出
        /// </summary>
        /// <param name="html">html代码</param>
        /// <returns></returns>
        public static string ZipHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                throw new ArgumentNullException(html);
            }
            //去除HTML中的空白字符
            html = Regex.Replace(html, @">\s+?<", "><");
            html = Regex.Replace(html, @"\r\n\s*", "");
            html = Regex.Replace(html, @"<body([\s|\S]*?)>([\s|\S]*?)</body>", @"<body$1>$2</body>", RegexOptions.IgnoreCase);
            return html;
        }

        /// <summary>
        /// 过滤指定HTML标签
        /// </summary>
        /// <param name="sTextStr">要过滤的字符</param>
        /// <param name="htmlStr">a img p div</param>
        public static string DelHtml(string sTextStr, string htmlStr)
        {
            if (string.IsNullOrEmpty(sTextStr))
            {
                throw new ArgumentNullException(sTextStr);
            }
            if (string.IsNullOrEmpty(htmlStr))
            {
                throw new ArgumentNullException(htmlStr);
            }
            string rStr = string.Empty;
            if (!string.IsNullOrEmpty(sTextStr))
            {
                rStr = Regex.Replace(sTextStr, "<" + htmlStr + "[^>]*>", "", RegexOptions.IgnoreCase);
                rStr = Regex.Replace(rStr, "</" + htmlStr + ">", "", RegexOptions.IgnoreCase);
            }
            return rStr;
        }

        /// <summary>
        /// 加载CSS样式文件
        /// </summary>
        public static string Css(string cssPath, System.Web.UI.Page p)
        {
            return @"<link href=""" + p.ResolveUrl(cssPath) + @""" rel=""stylesheet"" type=""text/css"" />" + "\r\n";
        }

        /// <summary>
        /// 加载javascript脚本文件
        /// </summary>
        public static string Js(string jsPath, System.Web.UI.Page p)
        {
            return @"<script type=""text/javascript"" src=""" + p.ResolveUrl(jsPath) + @"""></script>" + "\r\n";
        }
    }

    public class HttpHeader
    {
        public string ContentType { get; set; }

        public string Accept { get; set; }

        public string UserAgent { get; set; }

        public string Method { get; set; }

        public int MaxTry { get; set; }
    }
}
