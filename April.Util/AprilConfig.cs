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

        private static string _AllowUrl = string.Empty;
        /// <summary>
        /// 链接白名单（可不做身份验证）
        /// </summary>
        public static List<string> AllowUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_AllowUrl))
                {
                    _AllowUrl = Configuration["AllowUrl"];
                }
                List<string> listUrls = new List<string>();
                if (!string.IsNullOrEmpty(_AllowUrl))
                {
                    string[] urls = System.Text.RegularExpressions.Regex.Split(_AllowUrl, ",");
                    if (urls.Length > 0)
                    {
                        foreach (string url in urls)
                        {
                            if (!listUrls.Contains(url))
                            {
                                listUrls.Add(url);
                            }
                        }
                    }
                    
                }
                return listUrls;
            }
        }

        private static string _FilePath = string.Empty;
        /// <summary>
        /// 文件路径
        /// </summary>
        public static string FilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_FilePath))
                {
                    _FilePath = Configuration["CommonSettings:FilePath"];
                }
                return _FilePath;
            }
        }

        private static string _IsOpenCache = string.Empty;
        /// <summary>
        /// 是否使用Redis
        /// </summary>
        public static bool IsOpenCache
        {
            get
            {
                if (string.IsNullOrEmpty(_IsOpenCache))
                {
                    _IsOpenCache = Configuration["Redis:IsOpenRedis"];
                }
                if (_IsOpenCache.ToLower() == "true")
                {
                    return true;
                }
                return false;
            }
        }

        private static string _RedisConnectionString = string.Empty;
        /// <summary>
        /// Redis默认连接串
        /// </summary>
        public static string RedisConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_RedisConnectionString))
                {
                    _RedisConnectionString = Configuration["Redis:ConnectionString"];
                }
                return _RedisConnectionString;
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
