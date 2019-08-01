using AspectCore.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace April.Util.Attributes
{
    public class AprilLogAttribute : AbstractInterceptorAttribute
    {
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            LogUtil.Debug("AprilLogAttribute begin");
            await next(context);
            LogUtil.Debug("AprilLogAttribute end");
        }
    }
}
