namespace BasicMmethodExtensionClass.JavaScriptOperationHelper
{
    /// <summary>
    /// 选项生成器
    /// </summary>
    internal class OptionBuilder
    {
        private readonly JsObjectBuilder _defaultBuilder = new JsObjectBuilder();

        public JsObjectBuilder Listeners { get; set; } = new JsObjectBuilder();

        /// <summary>
        /// 构造函数
        /// </summary>
        public OptionBuilder()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public OptionBuilder(string propertyName, object propertyValue)
        {
            _defaultBuilder.AddProperty(propertyName, propertyValue, false);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <param name="persistOriginal"></param>
        public OptionBuilder(string propertyName, object propertyValue, bool persistOriginal)
        {
            _defaultBuilder.AddProperty(propertyName, propertyValue, persistOriginal);
        }

        /// <summary>
        /// 删除属性
        /// </summary>
        /// <param name="propertyName"></param>
        public void RemoveProperty(string propertyName)
        {
            if (_defaultBuilder.ContainsProperty(propertyName))
            {
                _defaultBuilder.RemoveProperty(propertyName);
            }
        }


        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public void AddProperty(string propertyName, object propertyValue)
        {
            AddProperty(propertyName, propertyValue, false);
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <param name="persistOriginal">是否保持原样</param>
        public void AddProperty(string propertyName, object propertyValue, bool persistOriginal)
        {
            _defaultBuilder.AddProperty(propertyName, propertyValue, persistOriginal);
        }

        /// <summary>
        /// 使用这个方法需要特别注意，因为这里返回的不是设置的属性了
        /// 比如："margin-right:5px;"被添加到OB中就变成："\"margin-right:5px;\""
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        internal string GetProperty(string propertyName)
        {
            return _defaultBuilder.GetProperty(propertyName);
        } 

        /// <summary>
        /// 返回对象的Json字符串表示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Listeners.Count > 0)
            {
                AddProperty("listeners", Listeners.ToString(), true);
            }

            return _defaultBuilder.ToString();
        }
    }
}
