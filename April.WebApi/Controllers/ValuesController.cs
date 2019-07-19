using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using April.Entity;
using April.Service.Interfaces;
using April.Util;
using April.Util.Entitys;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace April.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private readonly IStudentService _service;

        public ValuesController(IStudentService service)
        {
            _service = service;
        }

        // GET api/values
        /// <summary>
        /// 这就是个测试接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            //日志测试
            //LogUtil.Info("测试");

            //Cache测试
            //CacheUtil.Set("cachetest", "fwejio2123", new TimeSpan(0, 0, 10));//10s

            //Session测试
            //SessionUtil.SetSession("test", "test");

            //Cookie测试
            //CookieUtil.SetCookies("apirlcookietest", "这是个中文测试");

            
            StudentEntity entity = new StudentEntity();
            //新增
            entity.Name = "小明";
            entity.Age = 18;
            entity.Number = "007";
            entity.Sex = 0;
            entity.Address = "大洛阳";

            _service.Insert(entity);

            //修改
            //SqlFilterEntity filter = new SqlFilterEntity();
            //filter.Append($"ID=@ID");
            //filter.Add("@ID", 1);
            //entity = _service.GetEntity(filter);
            //if (entity != null)
            //{
            //    //entity.Name = "我被修改了";
            //    //_service.Update(entity);

            //    _service.Delete(entity);
            //}


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
            //value = CookieUtil.GetCookies("apirlcookietest");

            int count = 0;
            List<StudentEntity> lists = _service.GetPageList(id, 10, "", null, "", out count);

            value = JsonConvert.SerializeObject(lists);

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
