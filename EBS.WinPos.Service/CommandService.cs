using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.WinPos.Domain;
using EBS.Infrastructure;
using EBS.Infrastructure.Log;
namespace EBS.WinPos.Service
{
   public class CommandService
    {
        ILogger _log;
        DapperContext _dbContext;

        public CommandService()
        {
            _log = AppContext.Log;
            _dbContext = new DapperContext();
        }

        public int ExecuteCommand(string sql)
        {
            return _dbContext.ExecuteSql(sql, null);
        }
    }
}
