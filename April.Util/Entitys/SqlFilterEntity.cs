using System;
using System.Collections.Generic;
using System.Text;

namespace April.Util.Entitys
{
    public class SqlFilterEntity
    {
        private string _Filter = string.Empty;
        private Dictionary<string, object> _Value = null;
        /// <summary>
        /// 查询条件
        /// </summary>
        public string Filter { get => _Filter; set => _Filter = value; }
        /// <summary>
        /// 查询参数
        /// </summary>
        public Dictionary<string, object> Value { get => _Value; set => _Value = value; }

        /// <summary>
        /// 添加查询条件
        /// </summary>
        /// <param name="filter">条件</param>
        /// <param name="relation">关系</param>
        public void Append(string filter, string relation = "and")
        {
            if (string.IsNullOrEmpty(filter))
            {
                return;
            }
            if (Filter.Length > 0)
            {
                Filter += relation;
            }
            Filter += filter;
        }
        /// <summary>
        /// 添加查询参数
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Add(string key, object value)
        {
            if (string.IsNullOrEmpty(key) || value == null)
            {
                return;
            }
            if (Value == null)
            {
                Value = new Dictionary<string, object>();
            }
            if (Value.ContainsKey(key))
            {
                Value[key] = value;
            }
            else
            {
                Value.Add(key, value);
            }
        }
    }
}
