using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DapperForNet.SQL
{

    public abstract class SqlQuery
    {
        private static object objLock = new object();

        /// <summary>
        /// 查询TOP
        /// </summary>
        protected int _TopNum;

        /// <summary>
        /// 排序
        /// </summary>
        protected QueryOrder _Order;

        /// <summary>
        /// 组装的SQL WHERE部分
        /// </summary>
        protected StringBuilder _Sql;

        /// <summary>
        /// 参数动态类
        /// </summary>
        protected IList<DynamicPropertyModel> _Param;

        /// <summary>
        /// 参数前缀
        /// </summary>
        protected string ParamPrefix = "@";

        /// <summary>
        /// 处理的实体对象描述
        /// </summary>
        internal ModelDes _ModelDes;
        protected int _PageIndex;
        protected int _PageSize;
        protected DBType DbType;

        /// <summary>
        /// 不加入SQL执行的列
        /// </summary>
        protected IList<string> ExcColums;

        /// <summary>
        /// 动态参数类缓存
        /// </summary>
        protected static Dictionary<string, Type> DynamicParamModelCache = new Dictionary<string, Type>();

        /// <summary>
        /// 记录参数名和值
        /// </summary>
        protected Dictionary<string, object> ParamValues = new Dictionary<string, object>();

        /// <summary>
        /// TOP
        /// </summary>
        public virtual int TopNum { get { return _TopNum; } }

        /// <summary>
        /// 排序
        /// </summary>
        public virtual QueryOrder Order { get { return _Order; } }

        /// <summary>
        /// SQL字符串,只表示包括Where部分
        /// </summary>
        internal virtual string WhereSql
        {
            get
            {
                var sb = new StringBuilder();
                var arr = _Sql.ToString().Split(' ').Where(m => !string.IsNullOrEmpty(m)).ToList();
                if (arr.Count > 0)
                {
                    sb.Append("WHERE");
                }
                for (int i = 0; i < arr.Count; i++)
                {
                    if (i == 0 && (arr[i] == "AND" || arr[i] == "OR"))
                    {
                        continue;
                    }
                    if (i > 0 && arr[i - 1] == "(" && (arr[i] == "AND" || arr[i] == "OR"))
                    {
                        continue;
                    }
                    sb.Append(" ");
                    sb.Append(arr[i]);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        internal virtual string OrderSql
        {
            get
            {
                var sb = new StringBuilder();
                if (Order != null)
                {
                    sb.Append(" ");
                    sb.Append("ORDER BY");
                    sb.Append(" ");
                    sb.Append(Order.Field);
                    sb.Append(" ");
                    var desc = Order.IsDesc ? "DESC" : "ASC";
                    sb.Append(desc);
                }
                return sb.ToString();
            }
        }
        /// <summary>
        /// 查询参数对象
        /// </summary>
        internal object Param
        {
            get
            {
                if (this._Param != null && this._Param.Count > 0)
                {
                   //处理参数对象是否存在缓存中
                    var paramKeys = this.ParamValues.Keys.ToList();//当前使用的参数集合
                    var listCacheKeys = new List<string>();
                    listCacheKeys.Add(this._ModelDes.TableName);
                    listCacheKeys.AddRange(paramKeys);

                    var cacheKey = string.Empty;
                    foreach (var key in DynamicParamModelCache.Keys.Where(m => m.StartsWith(this._ModelDes.TableName)))
                    {
                        if (listCacheKeys.All(m => key.Split('_').Contains(m)))//当前参数是否是一样已经缓存的子集
                        {
                            cacheKey = key;
                            break;
                        }
                    }
                    if (string.IsNullOrEmpty(cacheKey))//为空则说明缓存不存在相应数据类型
                    {
                        cacheKey = string.Join("_", listCacheKeys);
                    }
                    Type modelType;
                    lock (objLock)//防止多线程同时操作DynamicParamModelCache
                    {
                        DynamicParamModelCache.TryGetValue(cacheKey, out modelType);
                        if (modelType == null)
                        {
                            var tyName = "CustomDynamicParamClass";
                            modelType = CustomDynamicBuilder.DynamicCreateType(tyName, this._Param);
                            DynamicParamModelCache.Add(cacheKey, modelType);
                        }
                    }
                    var model = Activator.CreateInstance(modelType);
                    foreach (var item in this.ParamValues)
                    {
                        modelType.GetProperty(item.Key).SetValue(model, item.Value, null);
                    }

                    return model;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 插入语句SQL
        /// </summary>
        internal virtual string InsertSql
        {
            get
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(string.Format("INSERT INTO {0}(", this._ModelDes.TableName));
                var colums = Common.GetExecColumns(this._ModelDes);
                for (int i = 0; i < colums.Count; i++)
                {
                    if (i == 0) sql.Append(colums[i].ColumnName);
                    else sql.Append(string.Format(",{0}", colums[i].ColumnName));
                }
                sql.Append(")");
                sql.Append(" VALUES(");
                for (int i = 0; i < colums.Count; i++)
                {
                    if (i == 0) sql.Append(string.Format("{0}{1}", ParamPrefix, colums[i].FieldName));
                    else sql.Append(string.Format(",{0}{1}", ParamPrefix, colums[i].FieldName));
                }
                sql.Append(") ");
                return sql.ToString();
            }
        }

        /// <summary>
        /// 删除SQL
        /// </summary>
        internal virtual string DeleteSql
        {
            get
            {
                return string.Format("DELETE FROM {0} {1}", this._ModelDes.TableName, this.WhereSql);
            }
        }

        /// <summary>
        /// 修改SQL
        /// </summary>
        internal virtual string UpdateSql
        {
            get
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(string.Format("UPDATE {0} SET", this._ModelDes.TableName));
                var colums = Common.GetExecColumns(this._ModelDes, false);
                if (this.ExcColums != null && this.ExcColums.Count > 0)
                {
                    colums = colums.Where(m => this.ExcColums.Contains(m.ColumnName)).ToList();
                }
                for (int i = 0; i < colums.Count; i++)
                {
                    if (i != 0) sql.Append(",");
                    sql.Append(" ");
                    sql.Append(colums[i].ColumnName);
                    sql.Append(" ");
                    sql.Append("=");
                    sql.Append(" ");
                    sql.Append(ParamPrefix + colums[i].FieldName);
                }
                if (string.IsNullOrEmpty(WhereSql))//没有where条件的情况
                {
                    var p = Common.GetPrimary(this._ModelDes);
                    sql.Append(string.Format(" WHERE {0}={1}", p.Column, ParamPrefix + p.Field));
                }
                else
                {
                    sql.Append(string.Format(" {0}", WhereSql));
                }

                return sql.ToString();
            }
        }

        /// <summary>
        /// 查询SQL
        /// </summary>
        internal virtual string QuerySql
        {
            get
            {
                var sqlStr = "";
                if (_TopNum > 0)
                {
                    if (this.DbType == DBType.SqlServer || this.DbType == DBType.SqlServerCE)
                        sqlStr = string.Format("SELECT TOP {0} {1} FROM {2} {3} {4}", this._TopNum, "*", this._ModelDes.TableName, this.WhereSql, this.OrderSql);
                    else if (this.DbType == DBType.Oracle)
                    {
                        var strWhere = "";
                        if (string.IsNullOrEmpty(this.WhereSql))
                        {
                            strWhere = string.Format(" WHERE  ROWNUM <= {0} ", this._TopNum);
                        }
                        else
                        {
                            strWhere = string.Format(" {0} AND ROWNUM <= {1} ", this.WhereSql, this._TopNum);
                        }
                        sqlStr = string.Format("SELECT * FROM {2} {3} {4}", this._ModelDes.TableName, strWhere, this.OrderSql);
                    }
                    else
                    {
                        sqlStr = string.Format("SELECT {0} FROM {1} {2} {3} LIMIT {4}", "*", this._ModelDes.TableName, this.WhereSql, this.OrderSql, this._TopNum);
                    }
                }
                else
                {
                    sqlStr = string.Format("SELECT {0} FROM {1} {2} {3}", "*", this._ModelDes.TableName, this.WhereSql, this.OrderSql);
                }
                return sqlStr;
            }
        }

        /// <summary>
        /// 分页SQL
        /// </summary>
        internal virtual string PageSql
        {
            get
            {
                var sqlPage = "";
                var orderStr = string.IsNullOrEmpty(this.OrderSql) ? "ORDER BY " + this._ModelDes.Properties.FirstOrDefault().Column : this.OrderSql;

                if (this.DbType == DBType.SqlServer || this.DbType == DBType.Oracle)
                {
                    var tP = this.DbType == DBType.Oracle ? this._ModelDes.TableName + ".*" : "*";
                    sqlPage = string.Format("SELECT * FROM (SELECT ROW_NUMBER() OVER ({0}) rid, {1} FROM {5} {2} ) p_paged WHERE rid>{3} AND rid<={4}",
                                             orderStr, tP, this.WhereSql, (this._PageIndex - 1) * this._PageSize, (this._PageIndex - 1) * this._PageSize + this._PageSize, this._ModelDes.TableName);
                }
                else if (this.DbType == DBType.SqlServerCE)
                {
                    sqlPage = string.Format("SELECT * FROM {0} {1} {2} OFFSET {3} ROWS FETCH NEXT {4} ROWS ONLY",
                        this._ModelDes.TableName, this.WhereSql, orderStr, (this._PageIndex - 1) * this._PageSize, (this._PageIndex - 1) * this._PageSize + this._PageSize);
                }
                else
                {
                    sqlPage = string.Format("SELECT * FROM {0} {1} {2} LIMIT {1} OFFSET {2}",
                        this._ModelDes.TableName, this.WhereSql, orderStr, (this._PageIndex - 1) * this._PageSize, (this._PageIndex - 1) * this._PageSize + this._PageSize);
                }
                return sqlPage;
            }
        }

        /// <summary>
        /// 数据总是SQL
        /// </summary>
        internal virtual string CountSql
        {
            get
            {
                return string.Format("SELECT COUNT(*) DataCount FROM {0} {1}", this._ModelDes.TableName, this.WhereSql);
            }
        }


        protected SqlQuery()
        {
            this._Sql = new StringBuilder();
        }

        /// <summary>
        /// TOP
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public SqlQuery Top(int top)
        {
            this._TopNum = top;
            return this;
        }

        /// <summary>
        /// 奖其它参数添加到参数对象中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        internal SqlQuery AppendParam<T>(T t) where T : class
        {
            if (this._Param == null)
            {
                this._Param = new List<DynamicPropertyModel>();
            }
            var model = Common.GetModelDes<T>();
            foreach (var item in model.Properties)
            {
                var value = model.ClassType.GetProperty(item.Field).GetValue(t, null);
                this.ParamValues.Add(item.Field, value);
                var pmodel = new DynamicPropertyModel();
                pmodel.Name = item.Field;
                if (value != null)
                    pmodel.PropertyType = value.GetType();
                else
                    pmodel.PropertyType = typeof(System.String);
                this._Param.Add(pmodel);
            }
            return this;
        }

        /// <summary>
        /// 分页条件
        /// </summary>
        /// <param name="pindex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public SqlQuery Page(int pindex, int pageSize)
        {
            this._PageIndex = pindex;
            this._PageSize = pageSize;
            return this;
        }

        /// <summary>
        /// 设置执行的属性列
        /// </summary>
        /// <param name="properties">属性集合</param>
        /// <returns></returns>
        public SqlQuery SetExcProperties<T>(IList<string> properties)
        {
            if (ExcColums == null)
                this.ExcColums = new List<string>();
            foreach (var item in properties)
            {
                var col = this._ModelDes.Properties.Where(m => m.Field == item).FirstOrDefault();
                if (col != null && (!this.ExcColums.Contains(col.Column)))
                    this.ExcColums.Add(col.Column);
            }
            return this;
        }
    }

    /// <summary>
    ///  组装查询
    /// </summary>
    public class SqlQuery<T> : SqlQuery where T : class
    {
        private SqlQuery()
            : base()
        {
            this._ModelDes = Common.GetModelDes<T>();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <returns></returns>
        public static SqlQuery<T> Builder(DbBase db)
        {
            var result = new SqlQuery<T>();
            result.ParamPrefix = db.ParamPrefix;
            result.DbType = db.DbType;
            return result;
        }

        /// <summary>
        /// 设置不执行的属性列
        /// </summary>
        /// <param name="properties">属性集合</param>
        /// <returns></returns>
        public SqlQuery<T> SetExcProperties(IList<string> properties)
        {
            if (ExcColums == null)
                this.ExcColums = new List<string>();
            foreach (var item in properties)
            {
                var col = this._ModelDes.Properties.Where(m => m.Field == item).FirstOrDefault();
                if (col != null && (!this.ExcColums.Contains(col.Column)))
                    this.ExcColums.Add(col.Column);
            }
            return this;
        }

        /// <summary>
        /// 创建排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public SqlQuery<T> OrderBy(Expression<Func<T, object>> expr, bool desc)
        {
            var field = Common.GetPropertyByExpress<T>(this._ModelDes, expr).Column;
            this._Order = new QueryOrder() { Field = field, IsDesc = desc };
            return this;
        }

        /// <summary>
        /// TOP
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public new SqlQuery<T> Top(int top)
        {
            this._TopNum = top;
            return this;
        }

        /// <summary>
        /// 左括号(
        /// </summary>
        /// <param name="isAnd">true为AND false为OR</param>
        /// <returns></returns>
        public SqlQuery<T> LeftInclude(bool isAnd = true)
        {
            var cn = isAnd ? "AND" : "OR";
            this._Sql.Append(" ");
            this._Sql.Append(cn);
            this._Sql.Append(" ");
            this._Sql.Append("(");
            return this;
        }

        /// <summary>
        /// 右括号)
        /// </summary>
        /// <returns></returns>
        public SqlQuery<T> RightInclude()
        {
            this._Sql.Append(" ");
            this._Sql.Append(")");
            return this;
        }

        /// <summary>
        /// AND方式连接一条查询条件
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="operation"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlQuery<T> AndWhere(Expression<Func<T, object>> expr, OperationMethod operation, object value)
        {
            return Where(expr, operation, value, true);
        }

        /// <summary>
        ///  Or方式连接一条查询条件
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="operation"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlQuery<T> OrWhere(Expression<Func<T, object>> expr, OperationMethod operation, object value)
        {
            return Where(expr, operation, value, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="operation"></param>
        /// <param name="value"></param>
        /// <param name="isAnd">true为AND false为OR</param>
        /// <returns></returns>
        private SqlQuery<T> Where(Expression<Func<T, object>> expr, OperationMethod operation, object value, bool isAnd)
        {
            var cn = isAnd ? "AND" : "OR";
            var field = Common.GetPropertyByExpress<T>(this._ModelDes, expr).Column;
            var op = GetOpStr(operation);
            StringBuilder sb = new StringBuilder();
            this._Sql.Append(" ");
            this._Sql.Append(cn);
            this._Sql.Append(" ");
            this._Sql.Append(field);
            this._Sql.Append(" ");
            this._Sql.Append(op);
            this._Sql.Append(" ");
            var model = AddParam(operation, field, value);
            this._Sql.Append(this.ParamPrefix + model.Name);

            return this;
        }

        /// <summary>
        /// 比较符
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private string GetOpStr(OperationMethod method)
        {
            switch (method)
            {
                case OperationMethod.Contains:
                    return "LIKE";
                case OperationMethod.EndsWith:
                    return "LIKE";
                case OperationMethod.Equal:
                    return "=";
                case OperationMethod.Greater:
                    return ">";
                case OperationMethod.GreaterOrEqual:
                    return ">=";
                case OperationMethod.In:
                    return "IN";
                case OperationMethod.Less:
                    return "<";
                case OperationMethod.LessOrEqual:
                    return "<=";
                case OperationMethod.NotEqual:
                    return "<>";
                case OperationMethod.StartsWith:
                    return "LIKE";
            }
            return "=";
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private object CreateParam(OperationMethod method, object value)
        {
            switch (method)
            {
                case OperationMethod.Contains:
                    return string.Format("%{0}%", value);
                case OperationMethod.EndsWith:
                    return string.Format("%{0}", value);
                case OperationMethod.Equal:
                    return value;
                case OperationMethod.Greater:
                    return value;
                case OperationMethod.GreaterOrEqual:
                    return value;
                case OperationMethod.In:
                    return value;
                case OperationMethod.Less:
                    return value;
                case OperationMethod.LessOrEqual:
                    return value;
                case OperationMethod.NotEqual:
                    return value;
                case OperationMethod.StartsWith:
                    return string.Format("{0}%", value);
            }
            return value;
        }

        /// <summary>
        /// 通过方法和值创建一个参数对象并记录
        /// </summary>
        /// <param name="method"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private DynamicPropertyModel AddParam(OperationMethod method, string field, object value)
        {
            if (this._Param == null)
            {
                this._Param = new List<DynamicPropertyModel>();
            }

            var model = new DynamicPropertyModel();
            model.Name = field + GetParamIndex(field);
            model.PropertyType = value.GetType();
            this._Param.Add(model);

            switch (method)
            {
                case OperationMethod.Contains:
                    this.ParamValues.Add(model.Name, string.Format("%{0}%", value));
                    break;
                case OperationMethod.EndsWith:
                    this.ParamValues.Add(model.Name, string.Format("%{0}", value));
                    break;
                case OperationMethod.Equal:
                    this.ParamValues.Add(model.Name, value);
                    break;
                case OperationMethod.Greater:
                    this.ParamValues.Add(model.Name, value);
                    break;
                case OperationMethod.GreaterOrEqual:
                    this.ParamValues.Add(model.Name, value);
                    break;
                case OperationMethod.In:
                    this.ParamValues.Add(model.Name, value);
                    break;
                case OperationMethod.Less:
                    this.ParamValues.Add(model.Name, value);
                    break;
                case OperationMethod.LessOrEqual:
                    this.ParamValues.Add(model.Name, value);
                    break;
                case OperationMethod.NotEqual:
                    this.ParamValues.Add(model.Name, value);
                    break;
                case OperationMethod.StartsWith:
                    this.ParamValues.Add(model.Name, string.Format("{0}%", value));
                    break;
            }
            return model;
        }

        private string GetParamIndex(string field)
        {
            var key = this.ParamValues.Keys.Where(m => m.StartsWith(field)).FirstOrDefault();
            if (key == null)
            {
                return "1";
            }
            else
            {
                return (int.Parse(key.Remove(0, field.Length)) + 1).ToString();
            }
        }
    }
}
