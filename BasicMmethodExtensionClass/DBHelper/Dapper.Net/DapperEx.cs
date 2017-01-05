using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperEx;

namespace BasicMmethodExtensionClass.DBHelper.Dapper.Net
{
    public static class DapperEx
    {
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbs"></param>
        /// <param name="t"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static int Insert<T>(this DbBase dbs, T t, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var db = dbs.DbConnecttion;
            var sql = SqlQuery<T>.Builder(dbs);
            var flag = db.Execute(sql.InsertSql, t, transaction, commandTimeout);
            int KeyID = 0;
            SetIdentity(db, (id) => { KeyID = id; }, transaction);
            return KeyID;
            //return flag == 1;
        }

        /// <summary>
        ///  批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbs"></param>
        /// <param name="lt"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static bool InsertBatch<T>(this DbBase dbs, IList<T> lt, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var db = dbs.DbConnecttion;
            var sql = SqlQuery<T>.Builder(dbs);
            var flag = db.Execute(sql.InsertSql, lt, transaction, commandTimeout);
            return flag == lt.Count;
        }

        /// <summary>
        /// 按条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbs"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static bool Delete<T>(this DbBase dbs, SqlQuery sql = null, IDbTransaction transaction = null) where T : class
        {
            var db = dbs.DbConnecttion;
            if (sql == null)
            {
                sql = SqlQuery<T>.Builder(dbs);
            }
            var f = db.Execute(sql.DeleteSql, sql.Param, transaction);
            return f > 0;
        }

        /// <summary>
        /// 按指定某型删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbs"></param>
        /// <param name="sql">如果sql为null，则根据t的主键进行修改</param>
        /// <returns></returns>
        public static bool Delete<T>(this DbBase dbs, T t, IDbTransaction transaction = null) where T : class
        {
            var db = dbs.DbConnecttion;
            SqlQuery sql = SqlQuery<T>.Builder(dbs);
            sql = sql.AppendParam<T>(t);
            var f = db.Execute(sql.DeleteSql, sql.Param, transaction);
            return f > 0;
        }

        /// <summary>
        /// 指定主键ID删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbs"></param>
        /// <param name="ID"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static bool DeleteByID<T>(this DbBase dbs, object ID, IDbTransaction transaction = null) where T : class
        {
            var db = dbs.DbConnecttion;
            SqlQuery sql = SqlQuery<T>.Builder(dbs);
            sql.KeyValue = ID;
            var f = db.Execute(sql.DeleteKeySql, sql.Param, transaction);
            return f > 0;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbs"></param>
        /// <param name="t">如果sql为null，则根据t的主键进行修改</param>
        /// <param name="sql">按条件修改</param>
        /// <returns></returns>
        public static bool Update<T>(this DbBase dbs, T t, SqlQuery sql = null, IDbTransaction transaction = null) where T : class
        {
            var db = dbs.DbConnecttion;
            if (sql == null)
            {
                sql = SqlQuery<T>.Builder(dbs);
            }
            sql = sql.AppendParam<T>(t);
            var f = db.Execute(sql.UpdateSql, sql.Param, transaction);
            return f > 0;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbs"></param>
        /// <param name="t">如果sql为null，则根据t的主键进行修改</param>
        /// <param name="updateProperties">要修改的属性集合</param>
        /// <param name="sql">按条件修改</param>
        /// <returns></returns>
        public static bool Update<T>(this DbBase dbs, T t, IList<string> updateProperties, SqlQuery sql = null, IDbTransaction transaction = null) where T : class
        {
            var db = dbs.DbConnecttion;
            if (sql == null)
            {
                sql = SqlQuery<T>.Builder(dbs);
            }
            sql = sql.AppendParam<T>(t)cProperties<T>(updateProperties);
            var f = db.Execute(sql.UpdateSql, sql.Param, transaction);
            return f > 0;
        }

        /// <summary>
        /// 获取默认一条数据，没有则为NULL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbs"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static T SingleOrDefault<T>(this DbBase dbs, SqlQuery sql, IDbTransaction transaction = null) where T : class
        {
            var db = dbs.DbConnecttion;
            if (sql == null)
            {
                sql = SqlQuery<T>.Builder(dbs);
            }
            sql = sql.Top(1);
            var result = db.Query<T>(sql.QuerySql, sql.Param, transaction);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbs"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="dataCount"></param>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        public static IList<T> Page<T>(this DbBase dbs, int pageIndex, int pageSize, out long dataCount, SqlQuery sqlQuery = null, IDbTransaction transaction = null) where T : class
        {
            var db = dbs.DbConnecttion;
            var result = new List<T>();
            dataCount = 0;
            if (sqlQuery == null)
            {
                sqlQuery = SqlQuery<T>.Builder(dbs);
            }
            sqlQuery = sqlQuery.Page(pageIndex, pageSize);
            var para = sqlQuery.Param;
            var cr = db.Query(sqlQuery.CountSql, para, transaction).SingleOrDefault();
            dataCount = (long)cr.DataCount;
            result = db.Query<T>(sqlQuery.PageSql, para, transaction).ToList();
            return result;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbs"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static IList<T> Query<T>(this DbBase dbs, SqlQuery sql = null, IDbTransaction transaction = null) where T : class
        {
            var db = dbs.DbConnecttion;
            if (sql == null)
            {
                sql = SqlQuery<T>.Builder(dbs);
            }
            var result = db.Query<T>(sql.QuerySql, sql.Param, transaction);
            return result.ToList();
        }

        /// <summary>
        /// 通过主键查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbs"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static T QueryByID<T>(this DbBase dbs, object ID, IDbTransaction transaction = null) where T : class
        {
            var db = dbs.DbConnecttion;
            SqlQuery sql = SqlQuery<T>.Builder(dbs);
            sql.KeyValue = ID;
            var result = db.Query<T>(sql.QueryKeySql, sql.Param, transaction).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 数据数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbs"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static long Count<T>(this DbBase dbs, SqlQuery sql = null, IDbTransaction transaction = null) where T : class
        {
            var db = dbs.DbConnecttion;
            if (sql == null)
            {
                sql = SqlQuery<T>.Builder(dbs);
            }
            var cr = db.Query(sql.CountSql, sql.Param, transaction).SingleOrDefault();
            return (long)cr.DataCount;
        }

        public static void SetIdentity(IDbConnection conn, Action<int> setId, IDbTransaction transaction = null)
        {
            dynamic identity = conn.Query("SELECT @@IDENTITY AS Id", null, transaction).Single();
            int newId = (int)identity.Id;
            setId(newId);
        }

        /// <summary>
        /// 判断对象是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbs"></param>
        /// <param name="ID"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static bool Exists<T>(this DbBase dbs, object ID, IDbTransaction transaction = null) where T : class
        {
            var db = dbs.DbConnecttion;
            SqlQuery sql = SqlQuery<T>.Builder(dbs);
            sql.KeyValue = ID;
            var f = db.Query(sql.ExistsSql, sql.Param, transaction).SingleOrDefault();
            return f.DataCount > 0; ;// f > 0;
        }

        /// <summary>
        ///自定义语句和存储过程查询--返回集合
        /// </summary>
        /// <typeparam name="T">返回集合</typeparam>
        /// <param name="sql">sql语句或存储过程名字</param>
        /// <param name="p">参数</param>
        /// <param name="cmdType">执行的命令类型</param>
        /// <param name="transaction">事物控制</param>
        /// DynamicParameters
        /// <returns></returns>
        public static IEnumerable<T> Query<T>(this DbBase dbs, string query, object p = null, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null)
        {
            var db = dbs.DbConnecttion;
            return db.Query<T>(query, p, transaction, true, null, cmdType);
        }

        /// <summary>
        /// 自定义语句和存储过程的增删改--返回影响的行数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbs"></param>
        /// <param name="query">执行的语句</param>
        /// <param name="parans">参数</param>
        /// <param name="transaction">事物控制</param>
        /// <returns>影响的行数</returns>
        public static int Execute(this DbBase dbs, string query, object parans, CommandType cmdType = CommandType.Text, IDbTransaction transaction = null)
        {
            var db = dbs.DbConnecttion;
            int row = db.Execute(query, parans, transaction, null, cmdType);
            return row;
        }

    }

}
