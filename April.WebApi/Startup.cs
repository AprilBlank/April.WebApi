using April.Service.Common.Depends;
using April.Util;
using April.WebApi.Filters;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace April.WebApi
{
    public class Startup
    {
        private static readonly List<string> _Assemblies = new List<string>()
        {
            "April.Service"
        };

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            repository = LogManager.CreateRepository("AprilLog");

            XmlConfigurator.Configure(repository, new FileInfo("Config/log4net.config"));//配置文件路径可以自定义
            BasicConfigurator.Configure(repository);

            AprilConfig.InitConfig(configuration);
            RedisUtil.InitRedis();
        }

        //log4net日志
        public static ILoggerRepository repository { get; set; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // ServiceInjection.ConfigureRepository(services);

            services.AddControllers();
            //任务调度
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>()
                .AddScoped<IHttpContextAccessor, HttpContextAccessor>();

            #region Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1.1.0",
                    Title = "April WebAPI",
                    Description = "后台框架",
                    //TermsOfService = "None",
                    Contact = new OpenApiContact { Name = "Blank", Email = "1829027193@qq.com", Url = new Uri("https://www.cnblogs.com/AprilBlank/") }
                });
                // 为 Swagger JSON and UI设置xml文档注释路径
                var basePath = Path.GetDirectoryName(AppContext.BaseDirectory);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                var xmlPath = Path.Combine(basePath, "April.xml");
                options.IncludeXmlComments(xmlPath);
            });
            #endregion

            #region Session
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = "April.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(2000);//设置session的过期时间
                options.Cookie.HttpOnly = true;//设置在浏览器不能通过js获得该cookie的值,实际场景根据自身需要
                options.Cookie.IsEssential = true;
            });
            #endregion

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", p =>
                {
                    p.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                    //.AllowCredentials();
                });
            });

            //只是示例，具体根据自身需要
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSome", p =>
                 {
                     p.WithOrigins("https://www.baidu.com")
                     .WithMethods("GET", "POST")
                     .WithHeaders(HeaderNames.ContentType, "x-custom-header");
                 });
            });

            services.AddHttpClient();

            //services.AddAspectCoreContainer();
            //services.BuildAspectInjectorProvider();

        }

        public void ConfigureContainer(ContainerBuilder container)
        {
            var assemblys = _Assemblies.Select(x => Assembly.Load(x)).ToList();
            List<Type> allTypes = new List<Type>();
            assemblys.ForEach(aAssembly =>
            {
                allTypes.AddRange(aAssembly.GetTypes());
            });

            // 通过Autofac自动完成依赖注入
            container.RegisterTypes(allTypes.ToArray())
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .InstancePerDependency();

            // 注册Controller
            container.RegisterAssemblyTypes(typeof(Startup).GetTypeInfo().Assembly)
                .Where(t => typeof(Controller).IsAssignableFrom(t) && t.Name.EndsWith("Controller", StringComparison.Ordinal))
                .PropertiesAutowired();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionFilter>();
            app.UseMiddleware<AuthFilter>();

            AprilConfig.ServiceProvider = app.ApplicationServices;
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelp V1");
                options.InjectJavascript("/jquery.min.js");
                options.InjectJavascript("/swagger_zh.js");
            });
            #endregion



            app.UseCors("AllowAll");

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            AutofacUtil.Container = app.ApplicationServices.GetAutofacRoot();
        }
    }
}
