using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace April.Util
{
    public class RedisUtil
    {
        private static RedisCache _redisCache = null;
        private static RedisCacheOptions options = null;
        /// <summary>
        /// 初始化Redis
        /// </summary>
        public static void InitRedis()
        {
            if (AprilConfig.IsOpenCache)
            {
                _redisCache = new RedisCache(GetOptions());
            }
        }
        /// <summary>
        /// 获取配置项信息
        /// </summary>
        /// <returns></returns>
        protected static RedisCacheOptions GetOptions()
        {
            options = new RedisCacheOptions();
            options.Configuration = AprilConfig.RedisConnectionString;
            options.InstanceName = "April.Redis";
            return options;
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="ExprireTime">过期时间</param>
        public static void Add(string key, object value, int ExprireTime = 10)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            string strValue = string.Empty;
            try
            {
                strValue = JsonConvert.SerializeObject(value);
            }
            catch (Exception ex)
            {
                LogUtil.Error($"Redis.Add转换失败:{ex.Message}");
            }
            if (!string.IsNullOrEmpty(strValue))
            {
                _redisCache.SetString(key, strValue, new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(ExprireTime)
                });
            }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string Get(string key, string defaultValue = "")
        {
            if (string.IsNullOrEmpty(key))
            {
                return defaultValue;
            }
            string value = _redisCache.GetString(key);
            if (string.IsNullOrEmpty(value))
            {
                value = defaultValue;
            }
            return value;
        }
        /// <summary>
        /// 获取数据（对象）
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            string value = Get(key);
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }
            T obj = default(T);
            try
            {
                obj = JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                LogUtil.Error($"Redis.Get转换失败：{ex.Message},数据：{value}");
            }
            return obj;
        }
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key">键</param>
        public static void Remove(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _redisCache.Remove(key);
            }
        }
        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="key">键</param>
        public static void Refresh(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _redisCache.Refresh(key);
            }
        }
        /// <summary>
        /// 重置数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expireTime">过期时间</param>
        public static void Replace(string key, object value, int expireTime = 10)
        {
            if (!string.IsNullOrEmpty(key))
            {
                Remove(key);
                Add(key, value, expireTime);
            }
        }
    }
}
