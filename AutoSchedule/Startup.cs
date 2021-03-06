using AutoSchedule.Common;
using AutoSchedule.Dtos.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewLife.Caching;
using System;
using System.Diagnostics;
using System.Text;

namespace AutoSchedule
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SqlLiteContext>(options => options.UseSqlite(Configuration.GetConnectionString("SqlLite")), ServiceLifetime.Transient);
            //services.AddDbContext<SqlLiteContext>(options => options.UseSqlite(Configuration.GetConnectionString("SqlLite")));

            //自定义注册服务
            services.AddExtendService(configure =>
            {
                configure.UseAutoTaskJob();
                configure.UseIOCJobFactory();
                configure.UseISchedulerFactory();
                configure.UseQuartzStartup();
            });
            //自定义服务获取类
            GetServiceByOther(services);

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            //getService(services);
            //.AddRazorRuntimeCompilation();
        }

        private void GetServiceByOther(IServiceCollection services)
        {
            GetContext.ServiceProvider = services.BuildServiceProvider();
        }

        private void getService()
        {
            //启动的时候清空Redis
            var ss = GetContext.ServiceProvider;
            var quartzStartup = (QuartzStartup)ss.GetService(typeof(QuartzStartup));
            quartzStartup.rds.Clear();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            //lifetime.ApplicationStarted.Register(UnRegService1);
            //lifetime.ApplicationStopping.Register(UnRegService);//应用停止后从服务中心注销
            //app.Run(r => r.Response.WriteAsync("没想到吧",Encoding.GetEncoding("utf-8")));

            //清空任务计划Redis缓存
            getService();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            
        }

        private void UnRegService()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = "notepad.exe";
            Process.Start(processInfo);
        }

        private void UnRegService1()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = "notepad.exe";
            Process.Start(processInfo);
        }
    }
}