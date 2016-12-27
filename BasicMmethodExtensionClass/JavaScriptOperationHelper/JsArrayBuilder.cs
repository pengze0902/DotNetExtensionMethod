using System;
using System.Collections.Generic;
using System.Text;

namespace BasicMmethodExtensionClass.JavaScriptOperationHelper
{
    /// <summary>
    /// 创建Javascript数组参数的帮助类
    /// </summary>
    public class JsArrayBuilder
    {
        /// <summary>
        /// 内部保存的数据
        /// </summary>
        public List<string> Properties { get; set; } = new List<string>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public JsArrayBuilder()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="propertyValue">初始属性值</param>
        public JsArrayBuilder(object propertyValue)
        {
            AddProperty(propertyValue, false);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="propertyValue">初始属性值</param>
        /// <param name="persistOriginal">是否保持原样</param>
        public JsArrayBuilder(object propertyValue, bool persistOriginal)
        {
            AddProperty(propertyValue, persistOriginal);
        }


        /// <summary>
        /// 删除属性
        /// </summary>
        /// <param name="propertyValue">属性值</param>
        public void RemoveProperty(string propertyValue)
        {
            if (Properties.Contains(propertyValue))
            {
                Properties.Remove(propertyValue);
            }
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="propertyValue">属性值</param>
        public void AddProperty(object propertyValue)
        {
            AddProperty(propertyValue, false);
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="propertyValue">属性值</param>
        /// <param name="persistOriginal">是否保持原样</param>
        public void AddProperty(object propertyValue, bool persistOriginal)
        {
            if (propertyValue == null)
            {
                throw new ArgumentNullException(nameof(propertyValue));
            }
            if (persistOriginal)
            {
                Properties.Add(propertyValue.ToString());
            }
            else
            {
                if (propertyValue is string)
                {
                    Properties.Add(JsHelper.Enquote(propertyValue.ToString()));
                }
                else if (propertyValue is bool)
                {
                    Properties.Add(propertyValue.ToString().ToLower());
                }
                else if (propertyValue is float || propertyValue is double)
                {
                    Properties.Add(JsHelper.NumberToString(propertyValue));
                }
                else
                {
                    Properties.Add(propertyValue.ToString());
                }
            }
        }

        /// <summary>
        /// 将整个数组中元素顺序反转
        /// </summary>
        public void Reverse()
        {
            Properties.Reverse();
        }

        /// <summary>
        /// 已经添加属性的个数
        /// </summary>
        public int Count
        {
            get
            {
                return Properties.Count;
            }
        }

        /// <summary>
        /// 返回对象的JSON字符串形式
        /// </summary>
        /// <returns>对象的JSON形式</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (string item in Properties)
            {
                sb.AppendFormat("{0},", item);
            }

            return "[" + sb.ToString().TrimEnd(',') + "]";
        }
    }
}
