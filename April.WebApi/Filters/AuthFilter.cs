using April.Util;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace April.WebApi.Filters
{
    public class AuthFilter
    {
        private readonly RequestDelegate _next;

        public AuthFilter(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            if (context.Request.Method == "OPTIONS")
            {
                return _next(context);
            }
            var headers = context.Request.Headers;
            //检查头文件是否有jwt token
            if (!headers.ContainsKey("Authorization"))
            {
                string path = context.Request.Path.Value;
                if (!AprilConfig.AllowUrl.Contains(path) && path.IndexOf("swagger") < 0 && !path.EndsWith(".js"))
                {
                    //这里做下相关的身份校验
                    return ResponseUtil.HandleExceptionAsync(401, "请登录");
                    
                    //判断是否有权限查看(在身份验证后判断对应的权限，这个方法后续再写)
                    return ResponseUtil.HandleExceptionAsync(-2, "无权访问");

                }
            }
            return _next(context);
        }
    }
}
