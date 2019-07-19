using April.Service.Implements;
using April.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace April.Service.Common.Depends
{
    public class ServiceInjection
    {
        public static void ConfigureRepository(IServiceCollection services)
        {
            services.AddSingleton<IStudentService, StudentService>();
        }
    }
}
