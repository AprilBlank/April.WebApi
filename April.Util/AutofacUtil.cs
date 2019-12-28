using Autofac;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace April.Util
{
    public class AutofacUtil
    {
        public static ILifetimeScope Container { get; set; }

        /// <summary>
        /// 获取服务(Single)
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <returns></returns>
        public static T GetService<T>() where T : class
        {
            return Container.Resolve<T>();
        }

        /// <summary>
        /// 获取服务(请求生命周期内)
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <returns></returns>
        public static T GetScopeService<T>() where T : class
        {
            return (T)GetService<IHttpContextAccessor>().HttpContext.RequestServices.GetService(typeof(T));
        }
    }
}
