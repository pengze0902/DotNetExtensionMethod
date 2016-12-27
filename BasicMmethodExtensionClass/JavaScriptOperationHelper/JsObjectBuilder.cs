using System;
using System.Collections.Generic;
using System.Text;

namespace BasicMmethodExtensionClass.JavaScriptOperationHelper
{
    /// <summary>
    /// 创建Javascript对象参数的帮助类
    /// </summary>
    public class JsObjectBuilder
    {
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();

        /// <summary>
        /// 已经添加属性的个数
        /// </summary>
        public int Count => _properties.Count;

        /// <summary>
        /// 构造函数
        /// </summary>
        public JsObjectBuilder()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="propertyValue">属性值</param>
        public JsObjectBuilder(string propertyName, object propertyValue)
        {
            AddProperty(propertyName, propertyValue, false);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="propertyValue">属性值</param>
        /// <param name="persistOriginal">是否保持原样</param>
        public JsObjectBuilder(string propertyName, object propertyValue, bool persistOriginal)
        {
            AddProperty(propertyName, propertyValue, persistOriginal);
        }

        /// <summary>
        /// 删除属性
        /// </summary>
        /// <param name="propertyName">属性名</param>
        public void RemoveProperty(string propertyName)
        {
            if (_properties.ContainsKey(propertyName))
            {
                _properties.Remove(propertyName);
            }
        }

        /// <summary>
        /// 是否包含属性
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public bool ContainsProperty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(propertyName);
            }
            return _properties.ContainsKey(propertyName);
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="propertyValue">属性值</param>
        public void AddProperty(string propertyName, object propertyValue)
        {
            AddProperty(propertyName, propertyValue, false);
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="propertyValue">属性值</param>
        /// <param name="persistOriginal">是否保持原样</param>
        public void AddProperty(string propertyName, object propertyValue, bool persistOriginal)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(propertyName);
            }
            if (propertyValue == null)
            {
                throw new ArgumentNullException(nameof(propertyValue));
            }
            RemoveProperty(propertyName);
            if (persistOriginal)
            {
                _properties.Add(propertyName, propertyValue.ToString());
            }
            else
            {
                if (propertyValue is string)
                {
                    _properties.Add(propertyName, JsHelper.Enquote(propertyValue.ToString()));
                }
                else if (propertyValue is bool)
                {
                    _properties.Add(propertyName, propertyValue.ToString().ToLower());
                }
                else if (propertyValue is float || propertyValue is double)
                {
                    _properties.Add(propertyName, JsHelper.NumberToString(propertyValue));
                }
                else
                {
                    _properties.Add(propertyName, propertyValue.ToString());
                }
            }
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns>属性值</returns>
        public string GetProperty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(propertyName);
            }
            return _properties.ContainsKey(propertyName) ? _properties[propertyName] : string.Empty;
        }

        /// <summary>
        /// 返回对象的JSON字符串形式
        /// </summary>
        /// <returns>对象的JSON形式</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (string key in _properties.Keys)
            {
                sb.AppendFormat("{0}:{1},", key, _properties[key]);
            }

            return "{" + sb.ToString().TrimEnd(',') + "}";
        }
    }
}
