using April.Util;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace April.WebApi.Filters
{
    public class ExceptionFilter
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public ExceptionFilter(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) //发生异常
            {
                context.Response.StatusCode = 500;
                LogUtil.Error($"response exception:{ex.Message}");// {ex.StackTrace}
                await ResponseUtil.HandleExceptionAsync(500, "服务器错误");
            }
        }
    }
}
