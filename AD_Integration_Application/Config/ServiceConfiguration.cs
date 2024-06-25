using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using AD_Integration_Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using UKG_Integration_Application.Database.BaseRepo;
using Domain.Entities.AD;

namespace AD_Integration_Application.Config
{

    public interface IServiceConfiguration
    {
        IServiceProvider ConfigureServices();
    }

    public class ServiceConfiguration() : IServiceConfiguration
    {
        public IServiceProvider ConfigureServices()
        {
            var hostBuilder = CreateHostBuilder(Array.Empty<string>());
            var host = hostBuilder.Build();
            return host.Services;
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(APPSETTINGS_PATH);
                    config.AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<IADService, ADService>();
                    //services.AddMemoryCache();
                    services.AddDbContext<IMSSQLContext, MSSQLContext>();
                    services.AddScoped<IBaseRepository<ADProfile>, BaseRepository<ADProfile>>();
                    //services.AddAutoMapper(Assembly.GetExecutingAssembly());
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        logging.AddEventLog(builder =>
                        {
                            builder.SourceName = "UkgApp";
                        });
                    }
                });
        }
    }

    
}