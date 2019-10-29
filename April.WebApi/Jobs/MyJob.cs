using April.Util;
using Quartz;
using System.Threading.Tasks;

namespace April.WebApi.Jobs
{
    public class MyJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                LogUtil.Debug("执行MyJob");
            });
        }
    }
}
