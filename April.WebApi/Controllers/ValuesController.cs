using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using April.Entity;
using April.Service.Interfaces;
using April.Util;
using April.Util.Entitys;
using April.Util.Entitys.QyThird;
using April.WebApi.Jobs;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Quartz;

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
            string value2 = string.Empty;

            #region ========日志测试========
            //日志测试
            //LogUtil.Info("测试");
            #endregion

            #region ========Cache测试========
            //Cache测试
            //CacheUtil.Set("cachetest", "fwejio2123", new TimeSpan(0, 0, 10));//10s
            #endregion

            #region ========Session测试========
            //Session测试
            //SessionUtil.SetSession("test", "test");
            #endregion

            #region ========Cookie测试========
            //Cookie测试
            //CookieUtil.SetCookies("apirlcookietest", "这是个中文测试");
            #endregion

            #region ========SqlSugar测试========
            //StudentEntity entity = new StudentEntity();
            ////新增
            //entity.Name = "小明";
            //entity.Age = 18;
            //entity.Number = "007";
            //entity.Sex = 0;
            //entity.Address = "大洛阳";

            //_service.Insert(entity);

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

            #endregion

            #region ========Aop测试========
            _service.Test();
            #endregion

            #region ========企业微信测试========
            //企业微信信息发送
            //QyThridUtil.GetAccessToken();
            //MessageRange range = new MessageRange();
            //range.Users = new List<string>();
            //range.Users.Add("10001");
            //QyThridUtil.SendMessage("我就是来测试", range, AprilEnums.MessageType.Text);
            #endregion

            #region ========Redis测试========
            ////添加
            //StudentEntity student = new StudentEntity();
            //student.ID = 3;
            //student.Name = "小明";
            //student.Number = "201245";
            //student.Sex = 0;
            //student.Age = 18;
            //student.Address = "洛阳市";
            //RedisUtil.Add("student_1", student);
            ////获取
            //StudentEntity student1 = RedisUtil.Get<StudentEntity>("student_1");
            //value2 = JsonConvert.SerializeObject(student1);
            ////覆盖
            //student.Name = "小红";
            //student.Age = 16;
            //student.Address = "不知道哪个村";
            //student.Sex = 1;
            //RedisUtil.Replace("student_1", student);
            ////删除
            //RedisUtil.Remove("student_1");
            #endregion


            return new string[] { "value1", value2 };
        }

        [HttpGet]
        [Route("QuartzTest")]
        public void QuartzTest(int type)
        {
            JobKey jobKey = new JobKey("demo","group1");
            switch (type)
            {
                //添加任务
                case 1:
                    var trigger = TriggerBuilder.Create()
                            .WithDescription("触发器描述")
                            .WithIdentity("test")
                            //.WithSchedule(CronScheduleBuilder.CronSchedule("0 0/30 * * * ? *").WithMisfireHandlingInstructionDoNothing())
                            .WithSimpleSchedule(x=>x.WithIntervalInSeconds(5).RepeatForever().WithMisfireHandlingInstructionNextWithRemainingCount())
                            .Build();
                    QuartzUtil.Add(typeof(MyJob), jobKey, trigger);
                    break;
                //暂停任务
                case 2:
                    QuartzUtil.Stop(jobKey);
                    break;
                //恢复任务
                case 3:
                    QuartzUtil.Resume(jobKey);
                    break;
            }
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
