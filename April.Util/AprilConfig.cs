using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace April.Util
{
    public class AprilConfig
    {
        public static IServiceProvider ServiceProvider;
        public static IConfiguration Configuration;

        public static void InitConfig(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        private static string _MySqlConnectionString = string.Empty;
        /// <summary>
        /// MySql默认连接串
        /// </summary>
        public static string MySqlConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_MySqlConnectionString))
                {
                    _MySqlConnectionString = Configuration["DefaultSqlConnectionString:MySql"];
                }
                return _MySqlConnectionString;
            }
        }

        /// <summary>
        /// 统一请求页面实体
        /// </summary>
        public static HttpContext HttpCurrent
        {
            get
            {
                object factory = ServiceProvider.GetService(typeof(IHttpContextAccessor));
                HttpContext context = ((IHttpContextAccessor)factory).HttpContext;
                return context;
            }
        }
    }
}
