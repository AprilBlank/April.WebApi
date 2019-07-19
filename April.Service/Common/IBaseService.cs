using April.Util.Entitys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace April.Service
{
    public interface IBaseService<T> : IDisposable
    {
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
        List<T> GetPageList(int pageIndex, int pageSize, string strField, SqlFilterEntity filter, string strOrder, out int totalCount);

        /// <summary>
        /// 获取列表集合
        /// </summary>
        /// <param name="field">查询字段</param>
        /// <param name="filter">查询条件</param>
        /// <returns>数据集合</returns>
        ISugarQueryable<T> GetList(string field, SqlFilterEntity filter);

        /// <summary>
        /// 获取列表集合
        /// </summary>
        /// <returns>数据集合</returns>
        ISugarQueryable<T> GetList(int top = 0);

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="field">查询字段</param>
        /// <param name="filter">查询条件</param>
        /// <returns>对象</returns>
        T GetEntity(SqlFilterEntity filter, string field = "");

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <returns>执行结果</returns>
        bool IsExists(SqlFilterEntity filter);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实例对象</param>
        /// <param name="ignoreColumns">排除列</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>自增id</returns>
        int Insert(T entity, List<string> ignoreColumns = null, bool isLock = false);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">实例对象</param>
        /// <param name="ignoreColumns">排除列</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>执行结果</returns>
        bool Update(T entity, List<string> ignoreColumns = null, bool isLock = false);

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="entity">实例对象</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>执行结果</returns>
        bool Delete(T entity, bool isLock = false);

    }
}
