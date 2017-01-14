using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace BasicMmethodExtensionWeb.Helper.DataTableHelper
{
    /// <summary>
    /// DataTable List相互转化
    /// </summary>
    public class DataTableHelper
    {
        /// <summary>
        /// 将泛型集合类转换成DataTable（表中无数据时使用）
        /// </summary>
        /// <typeparam name="T">集合项类型</typeparam>
        /// <param name="list">集合</param>
        /// <returns>数据集(表)</returns>
        public static DataTable NullListToDataTable(IList list)
        {
            var result = new DataTable();
            if (list.Count <= 0) return result;
            var propertys = list[0].GetType().GetProperties();
            foreach (var pi in propertys)
            {
                if (pi != null)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }
            }
            for (var i = 0; i < list.Count; i++)
            {
                var tempList = new ArrayList();
                foreach (var pi in propertys)
                {
                    var obj = pi.GetValue(list[i], null);
                    tempList.Add(obj);
                }
                var array = tempList.ToArray();
                result.LoadDataRow(array, true);
            }
            return result;
        }

        /// <summary>
        /// 将泛型集合类转换成DataTable（表中有数据时使用）
        /// </summary>
        /// <typeparam name="T">集合项类型</typeparam>
        /// <param name="list">集合</param>
        /// <returns>数据集(表)</returns>
        public static DataTable NoNullListToDataTable<T>(IList<T> list)
        {
            var ds = new DataSet();
            var dt = new DataTable(typeof(T).Name);
            var myPropertyInfo =
                typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var t in list)
            {
                if (t == null) continue;
                var row = dt.NewRow();
                for (int i = 0, j = myPropertyInfo.Length; i < j; i++)
                {
                    var pi = myPropertyInfo[i];
                    var name = pi.Name;
                    if (dt.Columns[name] != null) continue;
                    DataColumn column;
                    if (pi.PropertyType.UnderlyingSystemType.ToString() == "System.Nullable`1[System.Int32]")
                    {
                        column = new DataColumn(name, typeof(int));
                        dt.Columns.Add(column);
                        if (pi.GetValue(t, null) != null)
                            row[name] = pi.GetValue(t, null);
                        else
                            row[name] = DBNull.Value;
                    }
                    else
                    {
                        column = new DataColumn(name, pi.PropertyType);
                        dt.Columns.Add(column);
                        row[name] = pi.GetValue(t, null);
                    }
                }
                dt.Rows.Add(row);
            }
            ds.Tables.Add(dt);
            return ds.Tables[0];
        }


        /// <summary>
        /// 表中有数据或无数据时使用,可排除DATASET不支持System.Nullable错误
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(IList<T> list)
        {
            if (list == null || list.Count <= 0)
            {
                var result = new DataTable();
                object temp;
                if (list == null || list.Count <= 0) return result;
                var propertys = list[0].GetType().GetProperties();
                foreach (var pi in propertys)
                {
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }
                for (var i = 0; i < list.Count; i++)
                {
                    var tempList = new ArrayList();
                    foreach (var pi in propertys)
                    {
                        var obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    var array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
                return result;
            }
            var ds = new DataSet();
            var dt = new DataTable(typeof(T).Name);
            var myPropertyInfo =
                typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var t in list)
            {
                if (t == null) continue;
                var row = dt.NewRow();
                for (int i = 0, j = myPropertyInfo.Length; i < j; i++)
                {
                    var pi = myPropertyInfo[i];
                    var name = pi.Name;
                    if (dt.Columns[name] != null) continue;
                    DataColumn column;
                    if (pi.PropertyType.UnderlyingSystemType.ToString() == "System.Nullable`1[System.Int32]")
                    {
                        column = new DataColumn(name, typeof(int));
                        dt.Columns.Add(column);
                        if (pi.GetValue(t, null) != null)
                            row[name] = pi.GetValue(t, null);
                        else
                            row[name] = DBNull.Value;
                    }
                    else
                    {
                        column = new DataColumn(name, pi.PropertyType);
                        dt.Columns.Add(column);
                        row[name] = pi.GetValue(t, null);
                    }
                }
                dt.Rows.Add(row);
            }
            ds.Tables.Add(dt);
            return ds.Tables[0];
        }



        /// <summary>
        /// 合并相同的DataTable
        /// </summary>
        /// <param name="dataTable1"></param>
        /// <param name="dataTable2"></param>
        /// <returns></returns>
        public static DataTable MergeSameDatatable(DataTable dataTable1, DataTable dataTable2)
        {
            var newDataTable = dataTable1.Clone();
            var obj = new object[newDataTable.Columns.Count];
            for (var i = 0; i < dataTable1.Rows.Count; i++)
            {
                dataTable1.Rows[i].ItemArray.CopyTo(obj, 0);
                newDataTable.Rows.Add(obj);
            }
            for (var i = 0; i < dataTable2.Rows.Count; i++)
            {
                dataTable2.Rows[i].ItemArray.CopyTo(obj, 0);
                newDataTable.Rows.Add(obj);
            }
            return new DataTable();
        }

        /// <summary> 
        /// 将两个列不同的DataTable合并成一个新的DataTable 
        /// </summary> 
        /// <param name="dt1">Table表1</param> 
        /// <param name="dt2">Table表2</param> 
        /// <param name="dtName">合并后新的表名</param> 
        /// <returns></returns>
        public static DataTable UniteDataTable(DataTable dt1, DataTable dt2, string dtName)
        {
            var dt3 = dt1.Clone();
            for (var i = 0; i < dt2.Columns.Count; i++)
            {
                dt3.Columns.Add(dt2.Columns[i].ColumnName);
            }
            var obj = new object[dt3.Columns.Count];

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                dt3.Rows.Add(obj);
            }

            if (dt1.Rows.Count >= dt2.Rows.Count)
            {
                for (var i = 0; i < dt2.Rows.Count; i++)
                {
                    for (var j = 0; j < dt2.Columns.Count; j++)
                    {
                        dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                    }
                }
            }
            else
            {
                for (var i = 0; i < dt2.Rows.Count - dt1.Rows.Count; i++)
                {
                    var dr3 = dt3.NewRow();
                    dt3.Rows.Add(dr3);
                }
                for (var i = 0; i < dt2.Rows.Count; i++)
                {
                    for (var j = 0; j < dt2.Columns.Count; j++)
                    {
                        dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                    }
                }
            }
            dt3.TableName = dtName;
            return dt3;
        }


        /// <summary>
        /// Datatable 转 List<Dictionary<string, object>
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> DataTableToListDictory(DataTable table)
        {
            var ld = new List<Dictionary<string, object>>();
            for (var i = 0; i < table.Rows.Count; i++)
            {
                var dic = new Dictionary<string, object>();
                for (var j = 0; j < table.Columns.Count; j++)
                {
                    dic.Add(table.Columns[j].ColumnName, table.Rows[i][j]);
                }
                ld.Add(dic);
            }
            return ld;
        }

    }
}
