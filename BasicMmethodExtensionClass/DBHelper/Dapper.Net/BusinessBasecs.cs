using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperEx;

namespace BasicMmethodExtensionClass.DBHelper.Dapper.Net
{
    public class BusinessBase
    {
        public DbBase OpenConnection(string name = null)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                name = "strSqlCe";
            }
            return new DbBase(name);
        }
    }

    public class BusinessBase<T> : BusinessBase where T : class
    {
        public int Insert(T model)
        {
            using (var db = OpenConnection())
            {
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                return dal.Insert(model);
            }
        }

        public bool InsertBatch(List<T> models)
        {
            using (var db = OpenConnection())
            {
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                return dal.InsertBatch(models);
            }
        }

        public T SingleOrDefault(SqlQuery sqlWhere = null)
        {
            using (var db = OpenConnection())
            {
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                var result = dal.SingleOrDefault(sqlWhere);
                return result;
            }
        }

        public T GetByID(object ID)
        {
            using (var db = OpenConnection())
            {
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                var result = dal.GetByID(ID);
                return result;
            }
        }

        public IList<T> GetAll()
        {
            using (var db = OpenConnection())
            {
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                return dal.GetAll();
            }
        }

        public IList<T> GetAll(SqlQuery<T> sqlWhere)
        {
            using (var db = OpenConnection())
            {
                if (sqlWhere == null)
                {
                    sqlWhere = SqlQuery<T>.Builder(db);
                }
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                return dal.GetAll(sqlWhere);
            }
        }

        public IList<T> Page(int PageIndex, int PageSize, out long row, SqlQuery sql = null)
        {
            using (var db = OpenConnection())
            {
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                var result = dal.Page(PageIndex, PageSize, out row, sql);
                return result;
            }

        }

        public bool Delete(SqlQuery sqlWhere)
        {
            using (var db = OpenConnection())
            {
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                var result = dal.Delete(sqlWhere);
                return result;
            }
        }

        public bool Delete(T model)
        {
            using (var db = OpenConnection())
            {
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                var result = dal.Delete(model);
                return result;
            }
        }

        public bool DeleteByID(object ID)
        {
            using (var db = OpenConnection())
            {
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                var result = dal.DeleteByID(ID);
                return result;
            }
        }

        public bool DeleteByIds(IEnumerable idValues)
        {
            using (var db = OpenConnection())
            {
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                var result = dal.DeleteByIds(idValues);
                return result;
            }
        }

        public bool DeleteBatch(List<T> model)
        {
            using (var db = OpenConnection())
            {
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                var result = dal.DeleteBatch(model);
                return result;
            }
        }

        public bool Update(T Model, SqlQuery sqlWhere = null)
        {
            using (var db = OpenConnection())
            {
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                var result = dal.Update(Model, sqlWhere);
                return result;
            }
        }

        public bool UpdateAll(List<T> model)
        {
            using (var db = OpenConnection())
            {
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                var result = dal.UpdateBatch(model);
                return result;
            }
        }

        public long GetCount(SqlQuery sqlWhere = null)
        {
            using (var db = OpenConnection())
            {
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                var result = dal.GetCount(sqlWhere);
                return result;
            }
        }

        public bool Exists(object ID)
        {
            using (var db = OpenConnection())
            {
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                var result = dal.Exists(ID);
                return result;
            }
        }

        public IEnumerable<T> Query<T>(string sql, object param = null, CommandType cmdType = CommandType.Text)
            where T : class
        {
            using (var db = OpenConnection())
            {
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                return dal.Query<T>(sql, param, cmdType);
            }
        }

        public int Execute(string sql, object param = null, CommandType cmdType = CommandType.Text)
        {
            using (var db = OpenConnection())
            {
                DataAccessBase<T> dal = new DataAccessBase<T>(db);
                return dal.Execute(sql, param, cmdType);
            }
        }
    }
}
