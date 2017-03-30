using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using Dapper;
using EBS.Infrastructure;
using EBS.Infrastructure.Log;
namespace EBS.WinPos.Domain
{
   public class DapperContext
    {
       ILogger _log;
       IDbConnection _conn;
       public DapperContext()
       {
           _log = AppContext.Log;
       }
        public IDbConnection GetConnection()
        {          
            _conn = new SQLiteConnection(Config.ConnectionString);          
            return _conn;            
        }

        public T First<T>(string sql, object param)
        {
            var result = default(T);
            using (_conn = GetConnection())
            {
                _conn.Open();
                result= _conn.Query<T>(sql,param).FirstOrDefault();
                _conn.Close();
            }
            return result;
        }

        public IEnumerable<T> Query<T>(string sql,object param)
        {
            var result = default(IEnumerable<T>);
            using (_conn = GetConnection())
            {
                _conn.Open();
                result = _conn.Query<T>(sql,param);
                _conn.Close();
            }
            return result;
        }

        public T ExecuteScalar<T>(string sql, object param)
        {
            var result = default(T);           
            IDbTransaction tran = null;
            _conn = null;
            try
            {
                _conn = GetConnection();
                _conn.Open();
                tran = _conn.BeginTransaction();
                result = _conn.ExecuteScalar<T>(sql, param,tran);
                tran.Commit();
            }
            catch (Exception ex)
            {
                _log.Error(ex, sql);
                tran.Rollback();
            }
            finally
            {
                tran.Dispose();
                _conn.Close();
            }
            return result;
        }

        public int ExecuteSql(string sql,object param)
        {
            var rows = 0;
            IDbTransaction tran = null;
            _conn = null;
            try
            {
                _conn = GetConnection();
                _conn.Open();
                tran = _conn.BeginTransaction();
                rows = _conn.Execute(sql, param, tran);
                tran.Commit();
            }
            catch (Exception ex)
            {
                _log.Error(ex,sql);              
                tran.Rollback();
            }
            finally {
                tran.Dispose();
                _conn.Close();
            }
            return rows;
        }
    }
}
