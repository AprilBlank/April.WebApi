using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace April.Entity
{
    [SugarTable("test_Student")]
    public class StudentEntity
    {
        private int _ID = -1;
        private string _Name = string.Empty;
        private string _Number = string.Empty;
        private int _Age = 0;
        private int _Sex = 0;
        private string _Address = string.Empty;

        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get => _ID; set => _ID = value; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get => _Name; set => _Name = value; }
        /// <summary>
        /// 学号
        /// </summary>
        public string Number { get => _Number; set => _Number = value; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get => _Age; set => _Age = value; }
        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get => _Sex; set => _Sex = value; }
        /// <summary>
        /// 家庭住址
        /// </summary>
        [SugarColumn(ColumnName = "test_Address")]
        public string Address { get => _Address; set => _Address = value; }
    }
}
