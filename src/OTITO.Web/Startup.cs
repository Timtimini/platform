using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OTITO.Web.Controllers;
using OTITO_Services;
using OTITO_Services.Model;
using OTITO.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OTITO.Web.Models.Email;
using reCAPTCHA.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.Facebook;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace OTITO.Web
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

            services.AddScoped<IUserService, UserService>();

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));


            services.AddDbContext<OtitoDBContext>(options =>
                options.UseMySQL(Configuration.GetConnectionString("otito")));

            
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


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

            services.AddScoped<ITestService, TestService>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddTransient<CleanupPlainTextPasswordsService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            services.Configure<RecaptchaSettings>(Configuration.GetSection("RecaptchaSettings"));
            services.AddTransient<IRecaptchaService, RecaptchaService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
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


            app.UseCookiePolicy();

            app.UseStaticFiles();


            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("openotito-firebase-adminsdk-9fhtt-26f125a701.json"),
            });

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

            using (var scope = app.ApplicationServices.CreateScope())
            {
                // TODO: we can remove this after it's been run but it's not the end of the world if it's still around
                var cleanup = scope.ServiceProvider.GetService<CleanupPlainTextPasswordsService>();
                cleanup.Apply();
            }
        }
    }
}