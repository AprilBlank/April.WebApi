using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace April.Util.Attributes
{
    public class AprilLogAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            LogUtil.Debug("AprilLogAttribute end");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            LogUtil.Debug("AprilLogAttribute begin");
            
        }
    }
}
