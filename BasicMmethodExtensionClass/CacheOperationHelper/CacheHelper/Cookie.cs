using System;
using System.Collections.Specialized;
using System.Web;

namespace BasicMmethodExtensionClass.CacheOperationHelper.CacheHelper
{
    /// <summary>
    /// Cookie操作类
    /// </summary>
    public static class Cookie
    {
        /// <summary>
        /// 创建COOKIE对象并赋Value值，修改COOKIE的Value值也用此方法，因为对COOKIE修改必须重新设Expires
        /// </summary>
        /// <param name="strCookieName">COOKIE对象名</param>
        /// <param name="strValue">COOKIE对象Value值</param>
        public static void SetObj(string strCookieName, string strValue)
        {
            SetObj(strCookieName, 1, strValue, "", "/");
        }
        /// <summary>
        /// 创建COOKIE对象并赋Value值，修改COOKIE的Value值也用此方法，因为对COOKIE修改必须重新设Expires
        /// </summary>
        /// <param name="strCookieName">COOKIE对象名</param>
        /// <param name="iExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)，</param>
        /// <param name="strValue">COOKIE对象Value值</param>
        public static void SetObj(string strCookieName, int iExpires, string strValue)
        {
            SetObj(strCookieName, iExpires, strValue, "", "/");
        }
        /// <summary>
        /// 创建COOKIE对象并赋Value值，修改COOKIE的Value值也用此方法，因为对COOKIE修改必须重新设Expires
        /// </summary>
        /// <param name="strCookieName">COOKIE对象名</param>
        /// <param name="iExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)，</param>
        /// <param name="strValue">COOKIE对象Value值</param>
        /// <param name="strDomains">作用域,多个域名用;隔开</param>
        public static void SetObj(string strCookieName, int iExpires, string strValue, string strDomains)
        {
            SetObj(strCookieName, iExpires, strValue, strDomains, "/");
        }

        /// <summary>
        /// 创建COOKIE对象并赋Value值，修改COOKIE的Value值也用此方法，因为对COOKIE修改必须重新设Expires
        /// </summary>
        /// <param name="strCookieName">COOKIE对象名</param>
        /// <param name="iExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)，</param>
        /// <param name="strValue">COOKIE对象Value值</param>
        /// <param name="strDomains">作用域,多个域名用;隔开</param>
        /// <param name="strPath">作用路径</param>
        public static void SetObj(string strCookieName, int iExpires, string strValue, string strDomains, string strPath)
        {
            var strDomain = SelectDomain(strDomains);
            var objCookie = new HttpCookie(strCookieName.Trim())
            {
                Value = HttpUtility.UrlEncode(strValue.Trim())
            };
            if (strDomain.Length > 0)
                objCookie.Domain = strDomain;
            if (iExpires > 0)
            {
                if (iExpires == 1)
                    objCookie.Expires = DateTime.MaxValue;
                else
                    objCookie.Expires = DateTime.Now.AddSeconds(iExpires);
            }
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }

        /// <summary>
        /// 创建COOKIE对象并赋多个KEY键值
        /// 设键/值如下：
        /// NameValueCollection myCol = new NameValueCollection();
        /// myCol.Add("red", "rojo");
        /// myCol.Add("green", "verde");
        /// myCol.Add("blue", "azul");
        /// myCol.Add("red", "rouge");   结果“red:rojo,rouge；green:verde；blue:azul”
        /// </summary>
        /// <param name="strCookieName">COOKIE对象名</param>
        /// <param name="iExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)，</param>
        /// <param name="KeyValue">键/值对集合</param>
        public static void SetObj(string strCookieName, int iExpires, NameValueCollection KeyValue)
        {
            SetObj(strCookieName, iExpires, KeyValue, "", "/");
        }
        public static void SetObj(string strCookieName, int iExpires, NameValueCollection KeyValue, string strDomains)
        {
            SetObj(strCookieName, iExpires, KeyValue, strDomains, "/");
        }
        /// <summary>
        /// 创建COOKIE对象并赋多个KEY键值
        /// 设键/值如下：
        /// NameValueCollection myCol = new NameValueCollection();
        /// myCol.Add("red", "rojo");
        /// myCol.Add("green", "verde");
        /// myCol.Add("blue", "azul");
        /// myCol.Add("red", "rouge");   结果“red:rojo,rouge；green:verde；blue:azul”
        /// </summary>
        /// <param name="strCookieName">COOKIE对象名</param>
        /// <param name="iExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)，</param>
        /// <param name="keyValue">键/值对集合</param>
        /// <param name="strDomains">作用域,多个域名用;隔开</param>
        /// <param name="strPath">作用路径</param>
        public static void SetObj(string strCookieName, int iExpires, NameValueCollection keyValue, string strDomains, string strPath)
        {
            var strDomain = SelectDomain(strDomains);
            var objCookie = new HttpCookie(strCookieName.Trim());
            foreach (var key in keyValue.AllKeys)
            {
                objCookie[key] = HttpUtility.UrlEncode(keyValue[key].Trim());
            }
            if (strDomain.Length > 0)
                objCookie.Domain = strDomain;
            objCookie.Path = strPath.Trim();
            if (iExpires > 0)
            {
                if (iExpires == 1)
                    objCookie.Expires = DateTime.MaxValue;
                else
                    objCookie.Expires = DateTime.Now.AddSeconds(iExpires);
            }
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }
        /// <summary>
        /// 读取Cookie某个对象的Value值，返回Value值，如果对象本就不存在，则返回字符串null
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <returns>Value值，如果对象本就不存在，则返回字符串null</returns>
        public static string GetValue(string strCookieName)
        {
            if (HttpContext.Current.Request.Cookies[strCookieName] == null)
            {
                return null;
            }
            var value = HttpContext.Current.Request.Cookies[strCookieName].Value;
            return HttpUtility.UrlDecode(value);
        }
        /// <summary>
        /// 读取Cookie某个对象的某个Key键的键值，返回Key键值，如果对象本就不存在，则返回字符串null，如果Key键不存在，则返回字符串"KeyNonexistence"
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strKeyName">Key键名</param>
        /// <returns>Key键值，如果对象本就不存在，则返回字符串null，如果Key键不存在，则返回字符串"KeyNonexistence"</returns>
        public static string GetValue(string strCookieName, string strKeyName)
        {
            if (HttpContext.Current.Request.Cookies[strCookieName] == null)
            {
                return null;
            }
            var strObjValue = HttpContext.Current.Request.Cookies[strCookieName].Value;
            var strKeyName2 = strKeyName + "=";
            //if (strObjValue.IndexOf(strKeyName2) == -1)
            if (!strObjValue.Contains(strKeyName2))
                return null;
            var value = HttpContext.Current.Request.Cookies[strCookieName][strKeyName];
            return HttpUtility.UrlDecode(value);
        }
        /// <summary>
        /// 修改某个COOKIE对象某个Key键的键值 或 给某个COOKIE对象添加Key键 都调用本方法，操作成功返回字符串"success"，如果对象本就不存在，则返回字符串null。
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strKeyName">Key键名</param>
        /// <param name="KeyValue">Key键值</param>
        /// <param name="iExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)。注意：虽是修改功能，实则重建覆盖，所以时间也要重设，因为没办法获得旧的有效期</param>
        /// <returns>如果对象本就不存在，则返回字符串null，如果操作成功返回字符串"success"。</returns>
        public static string Edit(string strCookieName, string strKeyName, string KeyValue, int iExpires)
        {
            return Edit(strCookieName, strKeyName, KeyValue, iExpires, "", "/");
        }
        /// <summary>
        /// 修改某个COOKIE对象某个Key键的键值 或 给某个COOKIE对象添加Key键 都调用本方法，操作成功返回字符串"success"，如果对象本就不存在，则返回字符串null。
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strKeyName">Key键名</param>
        /// <param name="KeyValue">Key键值</param>
        /// <param name="iExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)。注意：虽是修改功能，实则重建覆盖，所以时间也要重设，因为没办法获得旧的有效期</param>
        /// <param name="strPath">作用路径</param>
        /// <returns>如果对象本就不存在，则返回字符串null，如果操作成功返回字符串"success"。</returns>
        public static string Edit(string strCookieName, string strKeyName, string KeyValue, int iExpires, string strPath)
        {
            return Edit(strCookieName, strKeyName, KeyValue, iExpires, "", strPath);
        }
        /// <summary>
        /// 修改某个COOKIE对象某个Key键的键值 或 给某个COOKIE对象添加Key键 都调用本方法，操作成功返回字符串"success"，如果对象本就不存在，则返回字符串null。
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strKeyName">Key键名</param>
        /// <param name="keyValue">Key键值</param>
        /// <param name="iExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)。注意：虽是修改功能，实则重建覆盖，所以时间也要重设，因为没办法获得旧的有效期</param>
        /// <param name="strDomains">作用域,多个域名用;隔开</param>
        /// <param name="strPath">作用路径</param>
        /// <returns>如果对象本就不存在，则返回字符串null，如果操作成功返回字符串"success"。</returns>
        public static string Edit(string strCookieName, string strKeyName, string keyValue, int iExpires, string strDomains, string strPath)
        {
            if (HttpContext.Current.Request.Cookies[strCookieName] == null)
                return null;
            var objCookie = HttpContext.Current.Request.Cookies[strCookieName];
            objCookie[strKeyName] = HttpUtility.UrlEncode(keyValue.Trim());
            if (iExpires > 0)
            {
                if (iExpires == 1)
                    objCookie.Expires = DateTime.MaxValue;
                else
                    objCookie.Expires = DateTime.Now.AddSeconds(iExpires);
            }
            HttpContext.Current.Response.Cookies.Add(objCookie);
            return "success";
        }
        /// <summary>
        /// 删除COOKIE对象
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        public static void Del(string strCookieName)
        {
            Del(strCookieName, "", "/");
        }
        /// <summary>
        /// 删除COOKIE对象
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strDomains">作用域,多个域名用;隔开</param>
        public static void Del(string strCookieName, string strDomains)
        {
            Del(strCookieName, strDomains, "/");
        }
        /// <summary>
        /// 删除COOKIE对象
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strDomains">作用域,多个域名用;隔开</param>
        /// <param name="strPath">作用路径</param>
        public static void Del(string strCookieName, string strDomains, string strPath)
        {
            var strDomain = SelectDomain(strDomains);
            var objCookie = new HttpCookie(strCookieName.Trim());
            if (strDomain.Length > 0)
                objCookie.Domain = strDomain;
            objCookie.Path = strPath.Trim();
            objCookie.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }
        /// <summary>
        /// 删除某个COOKIE对象某个Key子键，操作成功返回字符串"success"，如果对象本就不存在，则返回字符串null
        /// </summary>
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="strKeyName">Key键名</param>
        /// <param name="iExpires">COOKIE对象有效时间（秒数），1表示永久有效，0和负数都表示不设有效时间，大于等于2表示具体有效秒数，31536000秒=1年=(60*60*24*365)。注意：虽是修改功能，实则重建覆盖，所以时间也要重设，因为没办法获得旧的有效期</param>
        /// <returns>如果对象本就不存在，则返回字符串null，如果操作成功返回字符串"success"。</returns>
        public static string DelKey(string strCookieName, string strKeyName, int iExpires)
        {
            if (HttpContext.Current.Request.Cookies[strCookieName] == null)
            {
                return null;
            }
            var objCookie = HttpContext.Current.Request.Cookies[strCookieName];
            objCookie.Values.Remove(strKeyName);
            if (iExpires > 0)
            {
                if (iExpires == 1)
                    objCookie.Expires = DateTime.MaxValue;
                else
                    objCookie.Expires = DateTime.Now.AddSeconds(iExpires);
            }
            HttpContext.Current.Response.Cookies.Add(objCookie);
            return "success";
        }


        /// <summary>
        /// 定位到正确的域
        /// </summary>
        /// <param name="strDomains">作用域,多个域名用;隔开</param>
        /// <returns></returns>
        private static string SelectDomain(string strDomains)
        {
            var isLocalServer = false;
            if (strDomains.Trim().Length == 0)
                return "";
            var thisDomain = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
            //if (_thisDomain.IndexOf(".") < 0)//说明是计算机名，而不是域名
            if (!thisDomain.Contains("."))
                isLocalServer = true;
            var strDomain = "www.abc.com";//这个域名是瞎扯
            var _strDomains = strDomains.Split(';');
            for (var i = 0; i < _strDomains.Length; i++)
            {
                //if (_thisDomain.IndexOf(_strDomains[i].Trim()) < 0)//判断当前域名是否在作用域内
                if (!thisDomain.Contains(_strDomains[i].Trim()))
                    continue;
                //区分真实域名(或IP)与计算机名
                if (isLocalServer)
                    strDomain = "";//作用域留空，否则Cookie不能写入
                else
                    strDomain = _strDomains[i].Trim();
                break;
            }
            return strDomain;
        }
    }
}
