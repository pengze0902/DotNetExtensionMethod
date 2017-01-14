using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BasicMmethodExtensionWeb.Helper.JsonSerializer;

namespace BasicMmethodExtensionWeb.Helper.DataTableHelper
{
    /// <summary>
    /// DataTable数据转换类
    /// </summary>
    public static class DtHelp
    {
        /// <summary>
        /// 将dt转化成Json数据 格式如 table[{id:1,title:'体育'},id:2,title:'娱乐'}]
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DT2JSON(DataTable dt)
        {
            return DT2JSON(dt, 0, "Total", "Rows");
        }

        public static string DT2JSON(DataTable dt, int fromCount)
        {
            return DT2JSON(dt, fromCount, "Total", "Rows");
        }

        /// <summary>
        /// 将dt转化成Json数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fromCount"></param>
        /// <param name="totalCountStr"></param>
        /// <param name="tbname"></param>
        /// <returns></returns>
        public static string DT2JSON(DataTable dt, int fromCount, string totalCountStr, string tbname)
        {
            return DT2JSON(dt, fromCount, "Total", "Rows", true);
        }

        /// <summary>
        /// 将dt转化成Json数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fromCount"></param>
        /// <param name="totalCountStr"></param>
        /// <param name="tbname"></param>
        /// <param name="formatData"></param>
        /// <returns></returns>
        public static string DT2JSON(DataTable dt, int fromCount, string totalCountStr, string tbname, bool formatData)
        {
            var jsonBuilder = new StringBuilder();
            jsonBuilder.Append("\"" + totalCountStr + "\"" + ":\"" + fromCount+ "\",\"" + tbname + "\": [");
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                if (i > 0)
                    jsonBuilder.Append(",");
                jsonBuilder.Append("{");

                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    if (j > 0)
                        jsonBuilder.Append(",");
                    if (dt.Columns[j].DataType == typeof(DateTime) && dt.Rows[i][j].ToString() != "")
                        jsonBuilder.Append("\"" + dt.Columns[j].ColumnName.ToLower() + "\": \"" + Convert.ToDateTime(dt.Rows[i][j].ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "\"");
                    else if (dt.Columns[j].DataType == typeof(string))
                        jsonBuilder.Append("\"" + dt.Columns[j].ColumnName.ToLower() + "\":"+JSON.WriteString(dt.Rows[i][j].ToString()));
                    else
                        jsonBuilder.Append("\"" + dt.Columns[j].ColumnName.ToLower() + "\": \"" + dt.Rows[i][j] + "\"");
                }
                jsonBuilder.Append("}");
            }
            jsonBuilder.Append("]");
            return "{"+jsonBuilder+"}";

        }

        /// <summary>
        /// 将DataTable转换为list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> DT2List<T>(DataTable dt)//is2imgs 表示在转换list时是否附加img
        {
            if (dt == null)
                return null;
            var result = new List<T>();
            for (var j = 0; j < dt.Rows.Count; j++)
            {
                T t = (T)Activator.CreateInstance(typeof(T));
                System.Reflection.PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (System.Reflection.PropertyInfo pi in propertys)
                {
                    for (var i = 0; i < dt.Columns.Count; i++)
                    {
                        // 属性与字段名称一致的进行赋值 
                        if (!pi.Name.ToLower().Equals(dt.Columns[i].ColumnName.ToLower())) continue;
                        if (dt.Rows[j][i] != DBNull.Value)
                        {
                            switch (pi.PropertyType.ToString())
                            {
                                case "System.Int32":
                                    pi.SetValue(t, int.Parse(dt.Rows[j][i].ToString()), null);
                                    break;
                                case "System.DateTime":
                                    pi.SetValue(t, Convert.ToDateTime(dt.Rows[j][i].ToString()), null);
                                    break;
                                case "System.Boolean":
                                    pi.SetValue(t, Convert.ToBoolean(dt.Rows[j][i].ToString()), null);
                                    break;
                                case "System.Single":
                                    pi.SetValue(t, Convert.ToSingle(dt.Rows[j][i].ToString()), null);
                                    break;
                                case "System.Double":
                                    pi.SetValue(t, Convert.ToDouble(dt.Rows[j][i].ToString()),null);
                                    break;
                                default:
                                    pi.SetValue(t, dt.Rows[j][i].ToString(), null);
                                    break;
                            }
                        }
                        else
                            pi.SetValue(t, "", null);//为空，但不为Null
                        break;
                    }// end for
                } //end foreache
                result.Add(t);
            }

            return result;
        }
    }
}
