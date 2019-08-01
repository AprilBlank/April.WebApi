using April.Entity;
using April.Service.Interfaces;
using April.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace April.Service.Implements
{
    public class StudentService : BaseService<StudentEntity>, IStudentService
    {
        public void Test()
        {
            LogUtil.Debug("StudentService Test");
        }
    }
}
