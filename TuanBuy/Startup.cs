using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Channels;
using System.Threading.Tasks;
using Business.IServices;
using Business.Services;
using Data;
using Data.Entities;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.OpenApi.Models;
using TuanBuy.Models;
using TuanBuy.Models.Entities;
using TuanBuy.Models.Interface;
using Topic.Hubs;
using TuanBuy.Models.AppUtlity;

namespace TuanBuy
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
            services.AddControllersWithViews();
            //新增cookie驗證
            services.AddAuthentication(opt => 
                {
                    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }).AddCookie(opt =>
                {
                    //未登入時會自動導向這個網址
                    opt.LoginPath = new PathString("/Home/Login");
                    //因權限被拒絕時進入的網址
                    opt.AccessDeniedPath = new PathString("/Home/Index");
                }).AddFacebook(opt =>
                {
                    opt.AppId = "320771606641457";
                    opt.AppSecret = "45c376c9d3849f844f1276971acd55f6";
                })
                .AddGoogle(opt =>
                {
                    opt.ClientId = "924568647656-4j4di1veqsi11am0tlsr09jjsssl7hcv.apps.googleusercontent.com";
                    opt.ClientSecret = "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";
                });
            //注入HttpContext抓使用者資料
            services.AddHttpContextAccessor();
            //設定Redis Cache
            services.AddStackExchangeRedisCache(options =>
            {
                // Redis Server 的 IP 跟 Port
                //options.Configuration = "127.0.0.1:6379";
                options.Configuration = "tuanbuyredis.redis.cache.windows.net:6380,password=BFvMijldKRhmI0C1dcrCltsS1BYLwNLi3AzCaLDKszg=,ssl=True,abortConnect=False";
                options.InstanceName = "TuanWeb_";
            });
            services.AddSingleton<RedisProvider>();
            //弄個Swagger測試API
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TuanBuy API中心", Version = "v1" });
            });
            //EF CORE Context注入
            services.AddDbContext<TuanBuyContext>(option =>
                option.UseSqlServer(Configuration.GetConnectionString("TuanBuy")));
            //services.AddDbContext<Models.SqlDbServices>((builider) => 
            //    { builider.UseSqlServer(this.Configuration.GetConnectionString("TuanBuy")); });
            //倉儲模式注入
            services.AddTransient<GenericRepository<Product>>();
            services.AddTransient<GenericRepository<User>>();
            //注入Business服務
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserService, UsersService>();
            //加入HangFire
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("TuanBuy"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));
            services.AddHangfireServer();
            services.AddScoped<ITaskScheduling, TaskScheduling>();

            //services.AddSingleton<SqlDbServices>();
            services.AddSingleton<Topic.Hubs.UserService>();

            //調用websingnalr服務
            services.AddSignalR();

            // Add services to the container.
            services.AddHttpClient();

            //加入Session狀態服務
            services.AddSession(config =>
            {
                config.IdleTimeout = TimeSpan.FromDays(1);
            });


            //讓JSON裡面的中文字可以轉換過來
            services.AddControllersWithViews().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBackgroundJobClient backgroundJobs,IRecurringJobManager recurringJobManager,IServiceProvider serviceProvider)
        {
            //開發模式才能進去
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TuanBuyService v1"));
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            //使用靜態檔案
            app.UseStaticFiles();
            //使用HangFire
            app.UseHangfireDashboard();
            backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));
            recurringJobManager.AddOrUpdate(
                "寄發生日信",
                ()=>serviceProvider.GetService<ITaskScheduling>().DailyBirthday(), Hangfire.Cron.Monthly());
            recurringJobManager.AddOrUpdate(
                "商品下架",()=>serviceProvider.GetService<ITaskScheduling>().PullProduct(), Hangfire.Cron.Daily());

            //app.UseAuthorization();
            //使用Session Middleware
            app.UseSession();
            app.UseRouting();
            //使用權限註冊，這邊順序要Cookie -> Authentication -> Authorization
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHangfireDashboard();
            });
        }
    }
}
