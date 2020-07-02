using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OTITO_Services;
using OTITO_Services.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using OTITO.Web.Models.Email;
using reCAPTCHA.AspNetCore;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Hosting;

namespace OTITO.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("openotito-firebase-adminsdk-9fhtt-26f125a701.json"),
            });
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITestService, TestService>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddTransient<IRecaptchaService, RecaptchaService>();

            var connectionString = Configuration.GetConnectionString("otito");
            services.AddDbContext<OtitoDBContext>(o => o.UseMySQL(connectionString));

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddGoogle(options =>
                {
                    options.ClientId = "953290115905-02dopo16f8idrahud135am79hq3i4g94.apps.googleusercontent.com";
                    options.ClientSecret = "_bCB6cy_Kv5zJx-JlDB2jj4Q";
                })
                .AddFacebook(options =>
                {
                    options.AppId = "378002696398093";
                    options.AppSecret = "e0cece940a5bf61bf6cc9c8e5e89d0e8";
                })
                .AddCookie(options => { options.LoginPath = "/Users/Login/"; });

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<RecaptchaSettings>(Configuration.GetSection("RecaptchaSettings"));

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            app.UseStaticFiles();

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseRobotsTxt(builder =>
                builder
                    .AddSection(section =>
                        section
                            .AddComment("Allow")
                            .AddUserAgent("*")
                            .Allow("/")
                    )
                    .AddSitemap("https://otito.io/sitemap.xml")
            );
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            


        }
    }
}