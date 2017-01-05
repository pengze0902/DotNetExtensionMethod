using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperForNet
{   
    
    /// <summary>
    /// 用于动态生成类的一个属性
    /// </summary>
    public class DynamicPropertyModel
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType { get; set; }
    }
}
