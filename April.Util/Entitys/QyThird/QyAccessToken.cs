using System;
using System.Collections.Generic;
using System.Text;

namespace April.Util.Entitys
{
    public class QyAccessToken
    {
        private string _Access_Token = string.Empty;
        private int _Expires_In = 0;
        private DateTime _Expire_Time = DateTime.Now;
        private int _ErrCode = 0;
        private string _ErrMsg = string.Empty;

        public string Access_Token { get => _Access_Token; set => _Access_Token = value; }
        public int Expires_In { get => _Expires_In; set => _Expires_In = value; }
        public DateTime Expire_Time { get => _Expire_Time; set => _Expire_Time = value; }
        public int ErrCode { get => _ErrCode; set => _ErrCode = value; }
        public string ErrMsg { get => _ErrMsg; set => _ErrMsg = value; }
    }
}
