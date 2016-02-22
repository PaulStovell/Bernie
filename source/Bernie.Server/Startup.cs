using Bernie.Server.Core;
using Bernie.Server.Model;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bernie.Server
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                // Everyone who runs Bernie needs to make their own version of this file
                .AddJsonFile("appsettings.bernie.json");
            
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddAntiforgery();

            services.AddInstance<ISensorAuthenticator>(new SensorAuthenticator(Configuration["SensorAuthentication:UrlToken"]));
            services.AddInstance<IUserAuthenticator>(new UserAuthenticator(Configuration.Get<UserAuthenticationOptions>("UserAuthentication")));

            var clock = new SystemClock();
            var log = new SystemLog();
            var notificationConfig = Configuration.Get<NotificationConfiguration>("Notifications");
            var system = new SecuritySystem(clock, log, new NotificationService(new TextMessageService(notificationConfig), notificationConfig));
            services.AddInstance<ISecuritySystem>(system);
            services.AddInstance<IRecentEventLog>(log);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseExceptionHandler("/System/Error");
            app.UseStatusCodePagesWithReExecute("/System/Error/{0}");
            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());
            
            app.UseCookieAuthentication(options => 
            {
                options.AuthenticationScheme = "BernieCookie";
                options.LoginPath = new PathString("/User/SignIn");
                options.AccessDeniedPath = new PathString("/User/SignIn");
                options.AutomaticAuthenticate = true;
                options.AutomaticChallenge = true;
            });
            
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=System}/{action=Index}/{id?}");
            });
        }
        
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
