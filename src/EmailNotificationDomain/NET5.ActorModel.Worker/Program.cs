using System;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NET5.ActorModel.Actor;
using NET5.ActorModel.Core;
using NET5.ActorModel.Infrastructure;
using Serilog;
using Serilog.Events;

namespace NET5.ActorModel.Worker
{
    public class Program
    {
        private const string LogPath = @"C:\temp\workerservice\LogFile.txt";
        public static IServiceProvider ServiceProvider { get; private set; }
        public static void Main(string[] args)
        {
             Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(LogPath)
                .CreateLogger();

            try
            {
                Log.Information("Starting up the service");

                IHost build = CreateHostBuilder(args).Build();

                using var actorSystem = ActorSystem.Create("test-actor-system");
                actorSystem.UseServiceProvider(build.Services);
            
                var actor = actorSystem.ActorOf(actorSystem.DI().Props<NotificationActor>());

                Console.WriteLine("Enter message");
                while (true)
                {
                    var message = Console.ReadLine();
                    if (message == "q") break;
                    actor.Tell(message);
                }
                Console.ReadLine();
                actorSystem.Stop(actor);

                build.Run();

                return;
                }
            catch (Exception ex)
            {
                Log.Fatal(ex, "There was a problem starting the service");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
           
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>(); 
                    services.AddScoped<IEmailNotification, EmailNotification>();
                    services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
                    services.AddScoped<NotificationActor>();
                    services.AddScoped<LogNotificationActor>();                
                })
                .UseSerilog();
        }
    }
}
