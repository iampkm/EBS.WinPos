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

       public DapperContext()
       {
           _log = AppContext.Log;
       }
        public static IDbConnection GetConnection()
        {
            return new SQLiteConnection(Config.ConnectionString);
        }

        public T First<T>(string sql, object param)
        {
            var result = default(T);
            using (IDbConnection conn = GetConnection())
            {
                conn.Open();
                result= conn.Query<T>(sql,param).FirstOrDefault();
                conn.Close();
            }
            return result;
        }

        public IEnumerable<T> Query<T>(string sql,object param)
        {
            var result = default(IEnumerable<T>);
            using (IDbConnection conn = GetConnection())
            {
                conn.Open();
                result = conn.Query<T>(sql,param);
                conn.Close();
            }
            return result;
        }

        public T ExecuteScalar<T>(string sql, object param)
        {
            var result = default(T);           
            IDbTransaction tran = null;
            IDbConnection conn = null;
            try
            {
                conn = GetConnection();
                conn.Open();
                tran = conn.BeginTransaction();
                result = conn.ExecuteScalar<T>(sql, param,tran);
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
                conn.Close();
            }
            return result;
        }

        public int ExecuteSql(string sql,object param)
        {
            var rows = 0;
            IDbTransaction tran = null;
            IDbConnection conn = null;
            try
            {
                conn = GetConnection();
                conn.Open();
                tran = conn.BeginTransaction();
                rows = conn.Execute(sql, param, tran);
                tran.Commit();
            }
            catch (Exception ex)
            {
                _log.Error(ex,sql);              
                tran.Rollback();
            }
            finally {
                tran.Dispose();
                conn.Close();
            }
            return rows;
        }
    }
}
