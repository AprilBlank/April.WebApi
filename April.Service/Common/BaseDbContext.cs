using System;
using System.Collections.Generic;
using April.Util;
using SqlSugar;

namespace April.Service
{
    public class BaseDbContext
    {
        public SqlSugarClient Db;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connStr">数据库连接串</param>
        /// <param name="sqlType">数据库类型</param>
        public BaseDbContext(string connStr, int sqlType = 1)
        {
            InitDataBase(connStr, sqlType);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serverIp">服务器IP</param>
        /// <param name="user">用户名</param>
        /// <param name="pass">密码</param>
        /// <param name="dataBase">数据库</param>
        public BaseDbContext(string serverIp, string user, string pass, string dataBase)
        {
            string connStr = $"server={serverIp};user id={user};password={pass};persistsecurityinfo=True;database={dataBase}";
            InitDataBase(connStr);
        }

        /// <summary>
        /// 初始化数据库连接
        /// </summary>
        /// <param name="listConn">连接字符串</param>
        private void InitDataBase(string connStr, int sqlType = 1)
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = connStr,
                DbType = (DbType)sqlType,
                IsAutoCloseConnection = true,
                //SlaveConnectionConfigs = slaveConnectionConfigs
            });
            Db.Ado.CommandTimeOut = 30000;//设置超时时间
            Db.Aop.OnLogExecuted = (sql, pars) => //SQL执行完事件
            {
                //这里可以查看执行的sql语句跟参数
            };
            Db.Aop.OnLogExecuting = (sql, pars) => //SQL执行前事件
            {
                //这里可以查看执行的sql语句跟参数
            };
            Db.Aop.OnError = (exp) =>//执行SQL 错误事件
            {
                //这里可以查看执行的sql语句跟参数
            };
            Db.Aop.OnExecutingChangeSql = (sql, pars) => //SQL执行前 可以修改SQL
            {
                return new KeyValuePair<string, SugarParameter[]>(sql, pars);
            };
        }
        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTran()
        {
            Db.Ado.BeginTran();
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            Db.Ado.CommitTran();
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTran()
        {
            Db.Ado.RollbackTran();
        }
    }
}