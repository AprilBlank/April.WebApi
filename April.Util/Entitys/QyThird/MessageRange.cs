using System;
using System.Collections.Generic;
using System.Text;

namespace April.Util.Entitys.QyThird
{
    public class MessageRange
    {
        private List<string> _Users = null;
        private List<string> _Tags = null;
        private List<string> _Departments = null;
        private bool _IsAll = false;
        /// <summary>
        /// 发送的用户，最多1000个
        /// </summary>
        public List<string> Users { get => _Users; set => _Users = value; }
        /// <summary>
        /// 发送的标签，最多100个
        /// </summary>
        public List<string> Tags { get => _Tags; set => _Tags = value; }
        /// <summary>
        /// 发送的部门，最多100个
        /// </summary>
        public List<string> Departments { get => _Departments; set => _Departments = value; }
        /// <summary>
        /// 是否全发
        /// </summary>
        public bool IsAll { get => _IsAll; set => _IsAll = value; }
    }
}
