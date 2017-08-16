using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using NetCoreWeb.Models;
using Microsoft.EntityFrameworkCore;
using OfficeDevPnP.Core.Framework.Authentication;
using OfficeDevPnP.Core.Framework.Authentication.Events;
using Audit.Core;

namespace TechTalk
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // Add EF 7
            var connection = Configuration["Data:ConnectionString"];
            services.AddDbContext<FirstModel>(options => options.UseSqlServer(connection));

            //Add react
            //services.AddReact(); // Add React to the IoC container

            // Add framework services.
            services.AddMvc();

            //Add Session to the service collection
            services.AddSession();

            services.AddAuthentication(sharedOptions =>
                  sharedOptions.SignInScheme = SharePointAuthenticationDefaults.AuthenticationScheme);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var middleware = Audit.Core.Configuration.Setup()
            .UseSqlServer(config => config
            .ConnectionString(Configuration["Data:ConnectionString"])
            .Schema("dbo")
            .TableName("Events")
            .IdColumnName("EventId")
            .JsonColumnName("Data")
            .LastUpdatedColumnName("LastUpdatedDate"));



            //Added to enable SharePoint authentication
            ConfigureSharePointAuthentication(app);

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            
        }

        private void ConfigureSharePointAuthentication(IApplicationBuilder app)
        {
            //required to store SP Cache Key session data
            //must also call AddSession in the IServiceCollection
            app.UseSession();

            //OPTIONAL
            app.UseCookieAuthentication(
                new CookieAuthenticationOptions()
                {
                    AutomaticAuthenticate = false,
                    CookieHttpOnly = false, //set to false so we can read it from JavaScript
                    AutomaticChallenge = false,
                    AuthenticationScheme = "AspNet.ApplicationCookie",
                    ExpireTimeSpan = System.TimeSpan.FromDays(14),
                    LoginPath = "/account/login"
                }
            );

            //Add SharePoint authentication capabilities
            app.UseSharePointAuthentication(
                new SharePointAuthenticationOptions()
                {
                    ClientId = Configuration["SharePointAuthentication:ClientId"],
                    ClientSecret = Configuration["SharePointAuthentication:ClientSecret"],

                    AutomaticAuthenticate = true, //set to false if you prefer to manually call Authenticate on the handler.

                        //OPTIONAL: CookieAuthenticationScheme = "AspNet.ApplicationCookie",

                        //Handle events thrown by the auth handler
                        SharePointAuthenticationEvents = new SharePointAuthenticationEvents()
                    {
                        OnAuthenticationSucceeded = succeededContext =>
                        {
                            return Task.FromResult<object>(null);
                        },
                        OnAuthenticationFailed = failedContext =>
                        {
                            return Task.FromResult<object>(null);
                        }
                    }
                }
            );
        }
    }
}
