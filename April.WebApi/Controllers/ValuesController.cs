using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using April.Util;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace April.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        // GET api/values
        /// <summary>
        /// 这就是个测试接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            LogUtil.Info("测试");
            //CacheUtil.Set("cachetest", "fwejio2123", new TimeSpan(0, 0, 10));//10s
            //SessionUtil.SetSession("test", "test");
            CookieUtil.SetCookies("apirlcookietest", "这是个中文测试");
            return new string[] { "value1", "value2" };
        }
        /// <summary>
        /// 这就是个测试接口1
        /// </summary>
        /// <param name="id">就是个id</param>
        /// <returns></returns>
        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            string value = string.Empty;
            //value = CacheUtil.Get<string>("cachetest");
            //value = SessionUtil.GetSession("test");
            value = CookieUtil.GetCookies("apirlcookietest");
            return value;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
