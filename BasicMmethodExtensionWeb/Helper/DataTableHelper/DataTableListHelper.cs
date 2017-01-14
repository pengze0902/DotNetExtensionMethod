using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace BasicMmethodExtensionWeb.Helper.DataTableHelper
{
    /// <summary>
    /// DataTable和 List集合相互转换
    /// </summary>
    public static class DataTableListHelper
    {
        /// <summary>
        /// DataTable 转换成泛型List
        /// </summary>
        /// <typeparam name="T">实体对象类</typeparam>
        /// <param name="table">数据DatatTable</param>
        /// <returns> List<实体对象></returns>
        public static List<T> ToList<T>(this DataTable table)
        {
            if (table == null)
            {
                return null;
            }
            var list = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                var t = Activator.CreateInstance<T>();
                var propertypes = t.GetType().GetProperties();
                foreach (var pro in propertypes)
                {
                    if (!pro.CanWrite)
                    {
                        continue;
                    }
                    var tempName = pro.Name;
                    if (!table.Columns.Contains(tempName.ToUpper())) continue;
                    var value = row[tempName];
                    if (value is DBNull)
                    {
                        value = null;
                        if (pro.PropertyType.FullName == "System.String")
                        {
                            value = string.Empty;
                        }
                    }
                    if (pro.PropertyType.IsGenericType && pro.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && value != null)
                    {
                        pro.SetValue(t, Convert.ChangeType(value, Nullable.GetUnderlyingType(pro.PropertyType)), null);
                    }
                    else if (pro.PropertyType.IsEnum)
                    {
                        pro.SetValue(t, Convert.ChangeType(value, System.Enum.GetUnderlyingType(pro.PropertyType)), null);
                    }
                    else
                    {
                        pro.SetValue(t, value, null);
                    }
                }
                list.Add(t);
            }
            return list;
        }

        /// <summary>
        /// 将List转换成DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            var properties = TypeDescriptor.GetProperties(typeof(T));
            var dt = new DataTable();
            for (var i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                dt.Columns.Add(property.Name, property.PropertyType);
            }
            var values = new object[properties.Count];
            foreach (var item in data)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = properties[i].GetValue(item);
                }
                dt.Rows.Add(values);
            }
            return dt;
        }
    }
}
