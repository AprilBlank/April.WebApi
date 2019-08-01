using April.Entity;
using April.Util.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace April.Service.Interfaces
{
    public interface IStudentService : IBaseService<StudentEntity>
    {
        [AprilLog]
        void Test();
    }
}
