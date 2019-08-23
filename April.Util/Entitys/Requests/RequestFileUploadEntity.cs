using System;
using System.Collections.Generic;
using System.Text;

namespace April.Util.Entitys.Requests
{
    /// <summary>
    /// 文件请求上传实体
    /// </summary>
    public class RequestFileUploadEntity
    {
        private long _size = 0;
        private int _count = 0;
        private string _filedata = string.Empty;
        private string _fileext = string.Empty;
        private string _factoryid = string.Empty;
        private string _filename = string.Empty;

        /// <summary>
        /// 文件大小
        /// </summary>
        public long size { get => _size; set => _size = value; }
        /// <summary>
        /// 片段数量
        /// </summary>
        public int count { get => _count; set => _count = value; }
        /// <summary>
        /// 文件md5
        /// </summary>
        public string filedata { get => _filedata; set => _filedata = value; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public string fileext { get => _fileext; set => _fileext = value; }
        /// <summary>
        /// 工厂id
        /// </summary>
        public string factoryid { get => _factoryid; set => _factoryid = value; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string filename { get => _filename; set => _filename = value; }
    }
}
