using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace DapperForNet
{
    public class DbBase : IDisposable
    {
        private string paramPrefix = "@";

        private string providerName = "System.Data.SqlClient";

        private IDbConnection dbConnecttion;

        private DbProviderFactory dbFactory;

        private DBType _dbType = DBType.SqlServer;

        public IDbConnection DbConnecttion
        {
            get
            {
                return dbConnecttion;
            }
        }

        public IDbTransaction DbTransaction
        {
            get
            {
                return dbConnecttion.BeginTransaction();
            }
        }

        public string ParamPrefix
        {
            get
            {
                return paramPrefix;
            }
        }

        public string ProviderName
        {
            get
            {
                return providerName;
            }
        }

        public DBType DbType
        {
            get
            {
                return _dbType;
            }
        }

        public DbBase(string connectionStringName)
        {
            try
            {
                var connStr = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
                if (!string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName))
                    providerName = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;
                else
                    throw new Exception("ConnectionStrings中没有配置提供程序ProviderName！");
                dbFactory = DbProviderFactories.GetFactory(providerName);
                dbConnecttion = dbFactory.CreateConnection();
                dbConnecttion.ConnectionString = connStr;
                dbConnecttion.Open();
                SetParamPrefix();
            }
            catch (Exception ex)    
            {
                throw;
            }
        }


        private void SetParamPrefix()
        {
            string dbtype = (dbFactory == null ? dbConnecttion.GetType() : dbFactory.GetType()).Name;
            // 使用类型名判断
            if (dbtype.StartsWith("MySql")) _dbType = DBType.MySql;
            else if (dbtype.StartsWith("SqlCe")) _dbType = DBType.SqlServerCE;
            else if (dbtype.StartsWith("Npgsql")) _dbType = DBType.PostgreSQL;
            else if (dbtype.StartsWith("Oracle")) _dbType = DBType.Oracle;
            else if (dbtype.StartsWith("SQLite")) _dbType = DBType.SQLite;
            else if (dbtype.StartsWith("System.Data.SqlClient.")) _dbType = DBType.SqlServer;
            else if (dbtype.StartsWith("OleDb")) _dbType = DBType.Access;
            // else try with provider name
            else if (providerName.IndexOf("MySql", StringComparison.InvariantCultureIgnoreCase) >= 0) _dbType = DBType.MySql;
            else if (providerName.IndexOf("SqlServerCe", StringComparison.InvariantCultureIgnoreCase) >= 0) _dbType = DBType.SqlServerCE;
            else if (providerName.IndexOf("Npgsql", StringComparison.InvariantCultureIgnoreCase) >= 0) _dbType = DBType.PostgreSQL;
            else if (providerName.IndexOf("Oracle", StringComparison.InvariantCultureIgnoreCase) >= 0) _dbType = DBType.Oracle;
            else if (providerName.IndexOf("SQLite", StringComparison.InvariantCultureIgnoreCase) >= 0) _dbType = DBType.SQLite;
            else if (providerName.IndexOf("OleDb", StringComparison.InvariantCultureIgnoreCase) >= 0) _dbType = DBType.Access;

            if (_dbType == DBType.MySql && dbConnecttion != null && dbConnecttion.ConnectionString != null && dbConnecttion.ConnectionString.IndexOf("Allow User Variables=true") >= 0)
                paramPrefix = "?";
            if (_dbType == DBType.Oracle)
                paramPrefix = ":";
        }

        public void Dispose()
        {
            if (dbConnecttion != null)
            {
                try
                {
                    dbConnecttion.Dispose();
                }
                catch(Exception ex)
                {
                    throw;
                }
            }
        }
    }
    public enum DBType
    {
        /// <summary>
        /// SQL Server数据库
        /// </summary>
        SqlServer,

        /// <summary>
        /// SqlServerCE数据库
        /// </summary>
        SqlServerCE,

        /// <summary>
        /// MySql数据库
        /// </summary>
        MySql,

        /// <summary>
        /// PostgreSQL数据库
        /// </summary>
        PostgreSQL,

        /// <summary>
        /// Oracle数据库
        /// </summary>
        Oracle,

        /// <summary>
        /// SQLite数据库
        /// </summary>
        SQLite,

        /// <summary>
        /// Access数据库
        /// </summary>
        Access
    }
}
