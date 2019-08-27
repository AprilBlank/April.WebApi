using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using April.Util;
using April.Util.Entitys.Requests;
using April.Util.Entitys.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace April.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {

        /*
         * 第一步，握手传递文件大小，多少片，md5值，文件类型
         * 第二步，分片上传文件片段，存储文件
         * 第三步，合并所有文件，做md5校验
         */


        /// <summary>
        /// 请求上传文件
        /// </summary>
        /// <param name="requestFile">请求上传参数实体</param>
        /// <returns></returns>
        [HttpPost, Route("RequestUpload")]
        public MessageEntity RequestUploadFile([FromBody]RequestFileUploadEntity requestFile)
        {
            LogUtil.Debug($"RequestUploadFile 接收参数：{JsonConvert.SerializeObject(requestFile)}");
            MessageEntity message = new MessageEntity();
            if (requestFile.size <= 0 || requestFile.count <= 0 || string.IsNullOrEmpty(requestFile.filedata))
            {
                message.Code = -1;
                message.Msg = "参数有误";
            }
            else
            {
                //这里需要记录文件相关信息，并返回文件guid名，后续请求带上此参数
                string guidName = Guid.NewGuid().ToString("N");

                //前期单台服务器可以记录Cache，多台后需考虑redis或数据库
                CacheUtil.Set(guidName, requestFile, new TimeSpan(0, 10, 0), true);

                message.Code = 0;
                message.Msg = "";
                message.Data = new { filename = guidName };
            }
            return message;
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Upload")]
        public async Task<MessageEntity> FileSave()
        {
            var files = Request.Form.Files;
            long size = files.Sum(f => f.Length);
            string fileName = Request.Form["filename"];

            int fileIndex = 0;
            int.TryParse(Request.Form["fileindex"], out fileIndex);
            LogUtil.Debug($"FileSave开始执行获取数据：{fileIndex}_{size}");
            MessageEntity message = new MessageEntity();
            if (size <= 0 || string.IsNullOrEmpty(fileName))
            {
                message.Code = -1;
                message.Msg = "文件上传失败";
                return message;
            }

            if (!CacheUtil.Exists(fileName))
            {
                message.Code = -1;
                message.Msg = "请重新请求上传文件";
                return message;
            }

            long fileSize = 0;
            string filePath = $".{AprilConfig.FilePath}{DateTime.Now.ToString("yyyy-MM-dd")}/{fileName}";
            string saveFileName = $"{fileName}_{fileIndex}";
            string dirPath = Path.Combine(filePath, saveFileName);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            foreach (var file in files)
            {
                //如果有文件
                if (file.Length > 0)
                {
                    fileSize = 0;
                    fileSize = file.Length;

                    using (var stream = new FileStream(dirPath, FileMode.OpenOrCreate))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }

            message.Code = 0;
            message.Msg = "";
            return message;
        }

        /// <summary>
        /// 文件合并
        /// </summary>
        /// <param name="fileInfo">文件参数信息[name]</param>
        /// <returns></returns>
        [HttpPost, Route("Merge")]
        public async Task<MessageEntity> FileMerge([FromBody]Dictionary<string, object> fileInfo)
        {
            MessageEntity message = new MessageEntity();
            string fileName = string.Empty;
            if (fileInfo.ContainsKey("name"))
            {
                fileName = fileInfo["name"].ToString();
            }
            if (string.IsNullOrEmpty(fileName))
            {
                message.Code = -1;
                message.Msg = "文件名不能为空";
                return message;
            }

            //最终上传完成后，请求合并返回合并消息
            try
            {
                RequestFileUploadEntity requestFile = CacheUtil.Get<RequestFileUploadEntity>(fileName);
                if (requestFile == null)
                {
                    message.Code = -1;
                    message.Msg = "合并失败";
                    return message;
                }
                string filePath = $".{AprilConfig.FilePath}{DateTime.Now.ToString("yyyy-MM-dd")}/{fileName}";
                string fileExt = requestFile.fileext;
                string fileMd5 = requestFile.filedata;
                int fileCount = requestFile.count;
                long fileSize = requestFile.size;

                LogUtil.Debug($"获取文件路径：{filePath}");
                LogUtil.Debug($"获取文件类型：{fileExt}");

                string savePath = filePath.Replace(fileName, "");
                string saveFileName = $"{fileName}{fileExt}";
                var files = Directory.GetFiles(filePath);
                string fileFinalName = Path.Combine(savePath, saveFileName);
                LogUtil.Debug($"获取文件最终路径：{fileFinalName}");
                FileStream fs = new FileStream(fileFinalName, FileMode.Create);
                LogUtil.Debug($"目录文件下文件总数：{files.Length}");

                LogUtil.Debug($"目录文件排序前：{string.Join(",", files.ToArray())}");
                LogUtil.Debug($"目录文件排序后：{string.Join(",", files.OrderBy(x => x.Length).ThenBy(x => x))}");
                byte[] finalBytes = new byte[fileSize];
                foreach (var part in files.OrderBy(x => x.Length).ThenBy(x => x))
                {
                    var bytes = System.IO.File.ReadAllBytes(part);

                    await fs.WriteAsync(bytes, 0, bytes.Length);
                    bytes = null;
                    System.IO.File.Delete(part);//删除分块
                }
                fs.Close();
                //这个地方会引发文件被占用异常
                fs = new FileStream(fileFinalName, FileMode.Open);
                string strMd5 = GetCryptoString(fs);
                LogUtil.Debug($"文件数据MD5：{strMd5}");
                LogUtil.Debug($"文件上传数据：{JsonConvert.SerializeObject(requestFile)}");
                fs.Close();
                Directory.Delete(filePath);
                //如果MD5与原MD5不匹配，提示重新上传
                if (strMd5 != requestFile.filedata)
                {
                    LogUtil.Debug($"上传文件md5：{requestFile.filedata},服务器保存文件md5：{strMd5}");
                    message.Code = -1;
                    message.Msg = "MD5值不匹配";
                    return message;
                }

                CacheUtil.Remove(fileInfo["name"].ToString());
                message.Code = 0;
                message.Msg = "";
            }
            catch (Exception ex)
            {
                LogUtil.Error($"合并文件失败，文件名称：{fileName}，错误信息：{ex.Message}");
                message.Code = -1;
                message.Msg = "合并文件失败,请重新上传";
            }
            return message;
        }

        /// <summary>
        /// 请求下载文件
        /// </summary>
        /// <param name="fileInfo">文件参数信息[name]</param>
        /// <returns></returns>
        [HttpPost, Route("RequestDownload")]
        public MessageEntity RequestDownloadFile([FromBody]Dictionary<string, object> fileInfo)
        {
            MessageEntity message = new MessageEntity();
            string fileName = string.Empty;
            string fileExt = string.Empty;
            if (fileInfo.ContainsKey("name"))
            {
                fileName = fileInfo["name"].ToString();
            }
            if (fileInfo.ContainsKey("ext"))
            {
                fileExt = fileInfo["ext"].ToString();
            }
            if (string.IsNullOrEmpty(fileName))
            {
                message.Code = -1;
                message.Msg = "文件名不能为空";
                return message;
            }
            //获取对应目录下文件，如果有，获取文件开始准备分段下载
            string filePath = $".{AprilConfig.FilePath}{DateTime.Now.ToString("yyyy-MM-dd")}/{fileName}";
            filePath = $"{filePath}{fileExt}";
            FileStream fs = null;
            try
            {
                if (!System.IO.File.Exists(filePath))
                {
                    //文件为空
                    message.Code = -1;
                    message.Msg = "文件尚未处理完";
                    return message;
                }
                fs = new FileStream(filePath, FileMode.Open);
                if (fs.Length <= 0)
                {
                    //文件为空
                    message.Code = -1;
                    message.Msg = "文件尚未处理完";
                    return message;
                }
                int shardSize = 1 * 1024 * 1024;//一次1M
                RequestFileUploadEntity request = new RequestFileUploadEntity();
                request.fileext = fileExt;
                request.size = fs.Length;
                request.count = (int)(fs.Length / shardSize);
                if ((fs.Length % shardSize) > 0)
                {
                    request.count += 1;
                }
                request.filedata = GetCryptoString(fs);

                message.Data = request;
            }
            catch (Exception ex)
            {
                LogUtil.Debug($"读取文件信息失败：{filePath}，错误信息：{ex.Message}");
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }

            return message;
        }

        /// <summary>
        /// 分段下载文件
        /// </summary>
        /// <param name="fileInfo">请求参数信息[index,name]</param>
        /// <returns></returns>
        [HttpPost, Route("Download")]
        public async Task<IActionResult> FileDownload([FromBody]Dictionary<string, object> fileInfo)
        {
            //开始根据片段来下载
            int index = 0;
            if (fileInfo.ContainsKey("index"))
            {
                int.TryParse(fileInfo["index"].ToString(), out index);
            }
            else
            {
                return Ok(new { code = -1, msg = "缺少参数" });
            }
            string fileName = string.Empty;
            string fileExt = string.Empty;
            if (fileInfo.ContainsKey("name"))
            {
                fileName = fileInfo["name"].ToString();
            }
            if (fileInfo.ContainsKey("ext"))
            {
                fileExt = fileInfo["ext"].ToString();
            }
            if (string.IsNullOrEmpty(fileName))
            {
                return Ok(new { code = -1, msg = "文件名不能为空" });
            }
            //获取对应目录下文件，如果有，获取文件开始准备分段下载
            string filePath = $".{AprilConfig.FilePath}{DateTime.Now.ToString("yyyy-MM-dd")}/{fileName}";
            filePath = $"{filePath}{fileExt}";
            if (!System.IO.File.Exists(filePath))
            {
                return Ok(new { code = -1, msg = "文件尚未处理" });
            }
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                if (fs.Length <= 0)
                {
                    return Ok(new { code = -1, msg = "文件尚未处理" });
                }
                int shardSize = 1 * 1024 * 1024;//一次1M
                int count = (int)(fs.Length / shardSize);
                if ((fs.Length % shardSize) > 0)
                {
                    count += 1;
                }
                if (index > count - 1)
                {
                    return Ok(new { code = -1, msg = "无效的下标" });
                }
                fs.Seek(index * shardSize, SeekOrigin.Begin);
                if (index == count - 1)
                {
                    //最后一片 = 总长 - (每次片段大小 * 已下载片段个数)
                    shardSize = (int)(fs.Length - (shardSize * index));
                }
                byte[] datas = new byte[shardSize];
                await fs.ReadAsync(datas, 0, datas.Length);
                //fs.Close();
                return File(datas, "application/x-gzip");
            }
        }

        /// <summary>
        /// 文件流加密
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        private string GetCryptoString(Stream fileStream)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] cryptBytes = md5.ComputeHash(fileStream);
            return GetCryptoString(cryptBytes);
        }

        private string GetCryptoString(byte[] cryptBytes)
        {
            //加密的二进制转为string类型返回
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < cryptBytes.Length; i++)
            {
                sb.Append(cryptBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}