using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace April.Util
{
    public class SessionUtil
    {
        /// <summary>
        /// 设置Session
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetSession(string key, string value)
        {
            AprilConfig.HttpCurrent.Session.SetString(key, value);
        }
        /// <summary>
        /// 获取Session
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回对应的值</returns>
        public static string GetSession(string key, string defaultValue = "")
        {
            string value = AprilConfig.HttpCurrent.Session.GetString(key);
            if (string.IsNullOrEmpty(value))
            {
                value = defaultValue;
            }
            return value;
        }
    }
}
