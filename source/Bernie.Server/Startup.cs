using System;
using System.Collections.Generic;
using System.Linq;
using Bernie.Server.Core;
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
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddAntiforgery();

            var clock = new SystemClock();
            var log = new SystemLog();
            var system = new SecuritySystem(clock, log, new NotificationService());
            services.AddInstance<ISecuritySystem>(system);
            services.AddInstance<IRecentEventLog>(log);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());
            
            app.UseCookieAuthentication(options => 
            {
                options.AuthenticationScheme = "BernieCookie";
                options.LoginPath = new PathString("/User/Login");
                options.AccessDeniedPath = new PathString("/User/Login");
                options.AutomaticAuthenticate = true;
                options.AutomaticChallenge = true;
            });

            app.UseDeveloperExceptionPage();

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

    public class NotificationService : INotificationService
    {
        public void RaiseWarning()
        {
            Console.WriteLine("WARNING");
        }

        public void RaiseAlarm()
        {
            Console.WriteLine("ALARM");
        }

        public void Cancel()
        {

        }
    }

    public interface IRecentEventLog
    {
        List<LogEvent> GetRecentEvents();
    }

    public class SystemLog : ILog, IRecentEventLog
    {
        private readonly Queue<LogEvent> recentEvents = new Queue<LogEvent>();

        public void Append(LogCategory category, string message)
        {
            recentEvents.Enqueue(new LogEvent(DateTimeOffset.UtcNow, category, message));
        }

        public List<LogEvent> GetRecentEvents()
        {
            return recentEvents.ToList();
        }
    }

    public class SystemClock : IClock
    {
        public DateTimeOffset Now => DateTimeOffset.UtcNow;

        public IDisposable In(TimeSpan timespan, Action callback)
        {
            callback();
            return null;
        }
    }

    public class LogEvent
    {
        public LogEvent(DateTimeOffset when, LogCategory category, string message)
        {
            When = when;
            Category = category;
            Message = message;
        }

        public DateTimeOffset When { get; }
        public LogCategory Category { get; set; }
        public string Message { get; }
    }
}
