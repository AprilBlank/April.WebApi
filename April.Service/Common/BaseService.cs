using April.Util;
using April.Util.Entitys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace April.Service
{
    public class BaseService<T> : IBaseService<T> where T : class, new()
    {
        private BaseDbContext baseDb;
        protected SqlSugarClient db;

        public BaseService()
        {
            baseDb = new BaseDbContext(AprilConfig.MySqlConnectionString, 0);
            db = baseDb.Db;
        }

        /// <summary>
        /// 分页查询集合
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="strField">查询字段</param>
        /// <param name="filter">查询条件</param>
        /// <param name="strOrder">排序规则</param>
        /// <param name="totalCount">总数</param>
        /// <returns>数据集合</returns>
        public List<T> GetPageList(int pageIndex, int pageSize, string strField, SqlFilterEntity filter, string strOrder, out int totalCount)
        {
            totalCount = 0;
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            if (pageSize <= 0)
            {
                pageSize = 10;//暂定默认分页大小为10
            }
            if (string.IsNullOrEmpty(strField))
            {
                strField = "";
            }
            if (string.IsNullOrEmpty(strOrder))
            {
                strOrder = string.Format("ID asc");//这个地方我当时是在Config设置默认的排序
            }
            if (filter == null)
            {
                filter = new SqlFilterEntity();
            }
            return db.Queryable<T>().With(SqlWith.NoLock).Select(strField).WhereIF(!string.IsNullOrEmpty(filter.Filter), filter.Filter, filter.Value).OrderByIF(!string.IsNullOrEmpty(strOrder), strOrder).ToPageList(pageIndex, pageSize, ref totalCount);
        }

        /// <summary>
        /// 获取列表集合
        /// </summary>
        /// <param name="field">查询字段</param>
        /// <param name="filter">查询条件</param>
        /// <returns>数据集合</returns>
        public ISugarQueryable<T> GetList(string field, SqlFilterEntity filter)
        {
            if (string.IsNullOrEmpty(field))
            {
                field = "";
            }
            if (filter == null)
            {
                filter = new SqlFilterEntity();
            }
            return db.Queryable<T>().With(SqlWith.NoLock).Select(field).WhereIF(!string.IsNullOrEmpty(filter.Filter), filter.Filter, filter.Value);
        }
        /// <summary>
        /// 获取列表集合
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public ISugarQueryable<T> GetList(Expression<Func<T, bool>> where)
        {
            return db.Queryable<T>().With(SqlWith.NoLock).WhereIF(where != null, where);
        }

        /// <summary>
        /// 获取列表集合
        /// </summary>
        /// <returns>数据集合</returns>
        public ISugarQueryable<T> GetList(int top = 0)
        {
            if (top > 0)
            {
                return db.Queryable<T>().With(SqlWith.NoLock).Take(top);
            }
            else
            {
                return db.Queryable<T>().With(SqlWith.NoLock);
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="field">查询字段</param>
        /// <param name="filter">查询条件</param>
        /// <returns>对象</returns>
        public T GetEntity(SqlFilterEntity filter, string field = "")
        {
            if (string.IsNullOrEmpty(field))
            {
                field = "";
            }
            if (filter != null)
            {
                return db.Queryable<T>().With(SqlWith.NoLock).Select(field).WhereIF(!string.IsNullOrEmpty(filter.Filter), filter.Filter, filter.Value).First();
            }
            return default(T);
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <returns>执行结果</returns>
        public bool IsExists(SqlFilterEntity filter)
        {
            var result = db.Queryable<T>().With(SqlWith.NoLock).WhereIF(!string.IsNullOrEmpty(filter.Filter), filter.Filter, filter.Value).Count();
            return result > 0;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实例对象</param>
        /// <param name="ignoreColumns">排除列</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>自增id</returns>
        public int Insert(T entity, List<string> ignoreColumns = null, bool isLock = false)
        {
            if (ignoreColumns == null)
            {
                ignoreColumns = new List<string>();
            }
            var result = isLock ?
                db.Insertable(entity).With(SqlWith.UpdLock).IgnoreColumns(ignoreColumns.ToArray()).ExecuteReturnIdentity()
                : db.Insertable(entity).IgnoreColumns(ignoreColumns.ToArray()).ExecuteReturnIdentity();
            return result;
        }


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">实例对象</param>
        /// <param name="ignoreColumns">排除列</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>执行结果</returns>
        public bool Update(T entity, List<string> ignoreColumns = null, bool isLock = false)
        {
            if (ignoreColumns == null)
            {
                ignoreColumns = new List<string>();
            }
            var result = isLock ?
                db.Updateable(entity).With(SqlWith.UpdLock).IgnoreColumns(ignoreColumns.ToArray()).ExecuteCommand()
                : db.Updateable(entity).IgnoreColumns(ignoreColumns.ToArray()).ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="entity">实例对象</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>执行结果</returns>
        public bool Delete(T entity, bool isLock = false)
        {
            var result = isLock ?
                db.Deleteable<T>(entity).With(SqlWith.RowLock).ExecuteCommand().ObjToBool()
                : db.Deleteable<T>(entity).ExecuteCommand().ObjToBool();
            return result;
        }

        public void Dispose()
        {

        }
    }
}
