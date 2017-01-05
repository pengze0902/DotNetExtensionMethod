using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperEx;

namespace BasicMmethodExtensionClass.DBHelper.Dapper.Net
{
    public class DataAccessBase
   {
       public DbBase db { get; private set; }
 
       public DataAccessBase(DbBase Db)
       {
           this.db = Db;
       }
       #region 自定义其他方法
 
       #endregion
   }
   public class DataAccessBase<T> : DataAccessBase where T : class
   {
       public DataAccessBase(DbBase db) : base(db) { }
 
       #region  INSERT
       /// <summary>
       /// //插入一条数据
       /// </summary>
       /// <param name="user"></param>
       /// <returns></returns>
       public int Insert(T model, IDbTransaction tran = null)
       {
           var result = db.Insert<T>(model, tran);
           return result;
       }
 
       /// <summary>
       /// 插入批量数据
       /// </summary>
       /// <param name="models"></param>
       public bool InsertBatch(List<T> models, IDbTransaction tran = null)
       {
           var result = db.InsertBatch<T>(models, tran);
           return result;
 
       }
       #endregion
 
       #region SELECT
       /// <summary>
       /// 获取默认一条数据，没有则为NULL
       /// </summary>
       /// <param name="sqlWhere"></param>
       /// <returns></returns>
       public T SingleOrDefault(SqlQuery sqlWhere = null, IDbTransaction tran = null)
       {
           var result = db.SingleOrDefault<T>(sqlWhere, tran);
           return result;
       }
 
       /// <summary>
       /// 根据主键查询
       /// </summary>
       /// <param name="ID"></param>
       /// <returns></returns>
       public T GetByID(object ID, IDbTransaction tran = null)
       {
           var result = db.QueryByID<T>(ID, tran);
           return result;
       }
 
       /// <summary>
       /// 获取全部数据
       /// </summary>
       /// <returns></returns>
       public IList<T> GetAll(IDbTransaction tran = null)
       {
           var result = db.Query<T>(null, tran);
           return result;
 
       }
 
       /// <summary>
       /// 带条件查询
       /// </summary>
       /// <param name="d"></param>
       /// <returns></returns>
       public IList<T> GetAll(SqlQuery sqlWhere, IDbTransaction tran = null)
       {
           var result = db.Query<T>(sqlWhere, tran);
           return result;
       }
 
       /// <summary>
       /// 分页查询
       /// </summary>
       /// <param name="PageIndex"></param>
       /// <param name="PageSize"></param>
       /// <param name="row"></param>
       /// <param name="sql"></param>
       /// <returns></returns>
       public IList<T> Page(int PageIndex, int PageSize, out long row, SqlQuery sql = null, IDbTransaction tran = null)
       {
           var result = db.Page<T>(PageIndex, PageSize, out row, sql, tran);
           return result;
 
       }
    #endregion

    #region DELETE
    /// <summary>
    /// 自定义条件删除
    /// </summary>
    /// <param name="sqlWhere"></param>
    /// <returns></returns>
    public bool Delete(SqlQuery sqlWhere, IDbTransaction tran = null)
    {
        var result = db.Delete<T>(sqlWhere, tran);
        return result;
    }

    /// <summary>
    /// 按模型删除
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public bool Delete(T model, IDbTransaction tran = null)
    {
        var result = db.Delete<T>(model, tran);
        return result;
    }

    /// <summary>
    /// 根据主键ID删除
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public bool DeleteByID(object ID, IDbTransaction tran = null)
    {
        var result = db.DeleteByID<T>(ID, tran);
        return result;
    }

    /// <summary>
    /// 按主键批量删除
    /// </summary>
    /// <param name="idValues"></param>
    /// <returns></returns>
    public bool DeleteByIds(IEnumerable idValues, IDbTransaction tran = null)
    {
        bool result = false;
        //开启事务
        if (tran == null)
        {
            tran = db.DbTransaction;
        }
        foreach (var item in idValues)
        {
            result = db.DeleteByID<T>(item, tran);
            if (!result)
            {
                break;
            }
        }
        if (result)
        {
            tran.Commit();
        }
        else
        {
            tran.Rollback();
        }
        return result;
    }

    /// <summary>
    /// 批量删除
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public bool DeleteBatch(List<T> model, IDbTransaction tran = null)
    {
        bool result = false;
        //开启事务
        if (tran == null)
        {
            tran = db.DbTransaction;
        }
        foreach (var item in model)
        {
            result = db.Delete<T>(item, tran);
            if (!result)
            {
                break;
            }
        }
        if (result)
        {
            tran.Commit();
        }
        else
        {
            tran.Rollback();
        }
        return result;
    }
    #endregion

    #region UPDATE
    /// <summary>
    /// 修改--（带T和sqlWhere时可实现统一修改）
    /// </summary>
    /// <param name="model">如果sql为null，则根据model的主键进行修改</param>
    /// <param name="sqlWhere">按条件修改</param>
    public bool Update(T model, SqlQuery sqlWhere = null, IDbTransaction tran = null)
    {
        var result = db.Update<T>(model, sqlWhere, tran);
        return result;
    }

    /// <summary>
    /// 修改--可指定属性修改
    /// </summary>
    /// <param name="model">如果sql为null，则根据t的主键进行修改</param>
    /// <param name="updateProperties">要修改的属性集合</param>
    /// <param name="sqlWhere">按条件修改</param>
    /// <returns></returns>
    public bool Update(T model, IList<string> updateProperties, SqlQuery sqlWhere = null, IDbTransaction tran = null)
    {
        var result = db.Update<T>(model, updateProperties, sqlWhere, tran);
        return result;

    }

    /// <summary>
    /// 批量插入
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public bool UpdateBatch(List<T> model, IDbTransaction tran = null)
    {
        bool result = false;
        //开启事务
        if (tran == null)
        {
            tran = db.DbTransaction;
        }
        foreach (var item in model)
        {
            result = db.Update<T>(item, null, tran);
            if (!result)
            {
                break;
            }
        }
        if (result)
        {
            tran.Commit();
        }
        else
        {
            tran.Rollback();
        }
        return result;
    }
    #endregion

    #region ORTHER
    /// <summary>
    /// 获取数量
    /// </summary>
    /// <param name="sqlWhere"></param>
    /// <returns></returns>
    public long GetCount(SqlQuery sqlWhere = null, IDbTransaction tran = null)
    {
        return db.Count<T>(sqlWhere, tran);
    }

    /// <summary>
    /// 判断对象是否存在
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public bool Exists(object ID, IDbTransaction tran = null)
    {
        return db.Exists<T>(ID, tran);
    }

    /// <summary>
    /// 自定义语句和存储过程查询--返回集合
    /// </summary>
    /// <param name="sql">自定的语句或存储过程名字</param>
    /// <param name="param">参数</param>
    /// <param name="cmdType">类型</param>
    /// <returns></returns>
    public IEnumerable<T> Query<T>(string sql, object param = null, CommandType cmdType = CommandType.Text, IDbTransaction tran = null)
    {
        return db.Query<T>(sql, param, cmdType, tran);
    }

    /// <summary>
    /// 自定义语句和存储过程的增删改--返回影响的行数
    /// </summary>
    /// <param name="sql">自定的语句或存储过程名字</param>
    /// <param name="param">参数</param>
    /// <param name="cmdType">类型</param>
    /// <returns></returns>
    public int Execute(string sql, object param = null, CommandType cmdType = CommandType.Text, IDbTransaction tran = null)
    {
        return db.Execute(sql, param, cmdType, tran);
    }

    /// <summary>
    /// 使用DynamicParameters方式
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="cmdType"></param>
    /// <returns></returns>
    public int Execute(string sql, DynamicParameters param = null, CommandType cmdType = CommandType.Text, IDbTransaction tran = null)
    {
        //param.Add("@ID", 123);
        return db.Execute(sql, param, cmdType, tran);
    }
    #endregion

}
}     
