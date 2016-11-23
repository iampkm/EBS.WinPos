using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using Dapper;
namespace EBS.WinPos.Domain
{
   public class DapperContext
    {       
        public static IDbConnection GetConnection()
        {            
            return new SQLiteConnection(Config.ConnectionString);
        }

        public T First<T>(string sql)
        {
            var result = default(T);
            using (IDbConnection conn = GetConnection())
            {
                conn.Open();
                result= conn.Query<T>(sql).FirstOrDefault();
                conn.Close();
            }
            return result;
        }

        public IEnumerable<T> Query<T>(string sql)
        {
            var result = default(IEnumerable<T>);
            using (IDbConnection conn = GetConnection())
            {
                conn.Open();
                result = conn.Query<T>(sql);
                conn.Close();
            }
            return result;
        }

        public T ExecuteScalar<T>(string sql)
        {
            var result = default(T);
            using (IDbConnection conn = GetConnection())
            {
                conn.Open();
                var tran = conn.BeginTransaction();      
                result = conn.ExecuteScalar<T>(sql);
                if (result== null)
                {
                    tran.Commit();
                }
                else
                {
                    tran.Rollback();
                }
                conn.Close();
            }
            return result;
        }

        public int ExecuteSql(string sql,object param)
        {
            var rows = 0;
            using (IDbConnection conn = GetConnection())
            {
                conn.Open();
                var tran= conn.BeginTransaction();               
                rows = conn.Execute(sql,param,tran);
                if (rows > 0)
                {
                    tran.Commit();
                }
                else {
                    tran.Rollback();
                }
                conn.Close();
            }
            return rows;
        }
    }
}
