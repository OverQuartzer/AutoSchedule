using AutoSchedule.Common;
using AutoSchedule.Dtos.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewLife.Caching;
using System;

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

            //�Զ���ע�����
            services.AddExtendService(configure =>
            {
                configure.UseAutoTaskJob();
                configure.UseIOCJobFactory();
                configure.UseISchedulerFactory();
                configure.UseQuartzStartup();
            });
            //�Զ�������ȡ��
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
            //������ʱ�����Redis
            var ss = GetContext.ServiceProvider;
            var quartzStartup = (QuartzStartup)ss.GetService(typeof(QuartzStartup));
            quartzStartup.rds.Clear();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //�������ƻ�Redis����
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
    }
}