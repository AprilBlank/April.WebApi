using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace April.Util
{
    public class ResponseUtil
    {
        /// <summary>
        /// 回调
        /// </summary>
        /// <param name="statusCode">html状态码</param>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static Task HandleExceptionAsync(int statusCode, string msg)
        {
            var data = new { code = statusCode, msg = msg };
            string text = JsonConvert.SerializeObject(data);
            var response = AprilConfig.HttpCurrent.Response;
            if (string.IsNullOrEmpty(response.ContentType))
            {
                //跨域的时候注意，不带header没法接收回调
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Credentials", "true");
                //因为这个是json
                response.ContentType = "application/json;charset=utf-8;";
                response.StatusCode = 200;
                //response.ContentLength = text.Length;
                return response.WriteAsync(text);
            }
            else
            {
                return response.WriteAsync(text);
            }
        }
    }
}
