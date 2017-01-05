using DapperForNet.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DapperForNet
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
        public static bool Insert<T>(this DbBase dbs, T t, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var db = dbs.DbConnecttion;
            var sql = SqlQuery<T>.Builder(dbs);
            var flag = db.Execute(sql.InsertSql, t, transaction, commandTimeout);
            return flag == 1;
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
        public static bool Delete<T>(this DbBase dbs, SqlQuery sql = null) where T : class
        {
            var db = dbs.DbConnecttion;
            if (sql == null)
            {
                sql = SqlQuery<T>.Builder(dbs);
            }
            var f = db.Execute(sql.DeleteSql, sql.Param);
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
        public static bool Update<T>(this DbBase dbs, T t, SqlQuery sql = null) where T : class
        {
            var db = dbs.DbConnecttion;
            if (sql == null)
            {
                sql = SqlQuery<T>.Builder(dbs);
            }
            sql = sql.AppendParam<T>(t);
            var f = db.Execute(sql.UpdateSql, sql.Param);
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
        public static bool Update<T>(this DbBase dbs, T t, IList<string> updateProperties, SqlQuery sql = null) where T : class
        {
            var db = dbs.DbConnecttion;
            if (sql == null)
            {
                sql = SqlQuery<T>.Builder(dbs);
            }
            sql = sql.AppendParam<T>(t).SetExcProperties<T>(updateProperties);
            var f = db.Execute(sql.UpdateSql, sql.Param);
            return f > 0;
        }

        /// <summary>
        /// 获取默认一条数据，没有则为NULL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbs"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static T SingleOrDefault<T>(this DbBase dbs, SqlQuery sql) where T : class
        {
            var db = dbs.DbConnecttion;
            if (sql == null)
            {
                sql = SqlQuery<T>.Builder(dbs);
            }
            sql = sql.Top(1);
            var result = db.Query<T>(sql.QuerySql, sql.Param);
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
        public static IList<T> Page<T>(this DbBase dbs, int pageIndex, int pageSize, out long dataCount, SqlQuery sqlQuery = null) where T : class
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
            var cr = db.Query(sqlQuery.CountSql, para).SingleOrDefault();
            dataCount = (long)cr.DataCount;
            result = db.Query<T>(sqlQuery.PageSql, para).ToList();
            return result;
        }


        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbs"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static IList<T> Query<T>(this DbBase dbs, SqlQuery sql = null) where T : class
        {
            var db = dbs.DbConnecttion;
            if (sql == null)
            {
                sql = SqlQuery<T>.Builder(dbs);
            }
            var result = db.Query<T>(sql.QuerySql, sql.Param);
            return result.ToList();
        }

        /// <summary>
        /// 数据数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbs"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static long Count<T>(this DbBase dbs, SqlQuery sql = null) where T : class
        {
            var db = dbs.DbConnecttion;
            if (sql == null)
            {
                sql = SqlQuery<T>.Builder(dbs);
            }
            var cr = db.Query(sql.CountSql, sql.Param).SingleOrDefault();
            return (long)cr.DataCount;
        }
    }
}
