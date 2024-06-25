using Domain.Entities.UKG;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence.Common;
using Persistence.Context;
using Refit;
using System.Reflection;
using System.Runtime.InteropServices;
using UKG_Integration_Application.API_UKG;
using UKG_Integration_Application.Database.BaseRepo;
using UKG_Integration_Application.Database.DAL;
using UKG_Integration_Application.Model;
using UKG_Integration_Application.Services;
using UKG_Integration_Application.Services.Interfaces;
using UKG_Integration_Application.Services.PersistenceServices;
using UKG_Integration_Application.Validators;

namespace UKG_Integration_Application.Config
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

                    services.AddTransient<AuthorizationHandler>();
                    services.AddTransient<ConnectionStringService>();
                    services.AddTransient<IAccessTokenService, AccessTokenService>();
                    services.AddTransient<IAuthService, AuthService>();
                    services.AddTransient<IIdHyperfindService, IdHyperfindService>();
                    services.AddTransient<IEmployeeDataService, EmployeeDataService>();
                    services.AddTransient<IBatchingService, BatchingService>();
                    services.AddTransient<IPersonIdService, PersonIdService>();
                    services.AddTransient<IWorkedShiftDAL, WorkedShiftDAL>();
                    services.AddTransient<IPaycodeEditDAL, PaycodeEditDAL>();
                    services.AddTransient<IScheduleShiftDAL, ScheduleShiftDAL>();
                    services.AddTransient<ID_LateDataService, D_LateDataService>();
                    services.AddTransient<IHoursWorkedDataService, HoursWorkedDataService>();
                    services.AddTransient<ITimecardDataService, TimecardDataService>();
                    services.AddTransient<IScheduleShiftService, ScheduleShiftService>();
                    services.AddTransient<IWorkedShiftService, WorkedShiftService>();
                    services.AddTransient<IPaycodeEditService, PaycodeEditService>();
                    services.AddTransient<IEmployeeService, EmployeeService>();
                    services.AddTransient<StoredProcedureService>();
                    services.AddScoped<IValidator<TokenDTO>, TokenValidator>();
                    services.AddScoped<IValidator<HyperfindDTO>, HyperfindValidator>();
                    services.AddMemoryCache();
                    services.AddDbContext<IMSSQLContext, MSSQLContext>();

                    services.AddScoped<IBaseRepository<Employee>, BaseRepository<Employee>>();
                    services.AddScoped<IBaseRepository<D_LatePaycodeEdits>, BaseRepository<D_LatePaycodeEdits>>();
                    services.AddScoped<IBaseRepository<HoursWorked>, BaseRepository<HoursWorked>>();
                    services.AddScoped<IBaseRepository<WorkedShift>, BaseRepository<WorkedShift>>();
                    services.AddScoped<IBaseRepository<PaycodeEdit>, BaseRepository<PaycodeEdit>>();
                    services.AddScoped<IBaseRepository<ScheduleShift>, BaseRepository<ScheduleShift>>();

                    services.AddRefitClient<IUkgAccessToken>()
                        .ConfigureHttpClient(client => 
                            client.BaseAddress = new Uri(BASE_URL));
                    services.AddRefitClient<IUkgApi>()
                        .ConfigureHttpClient(client => 
                            client.BaseAddress = new Uri(BASE_URL))
                        .AddHttpMessageHandler<AuthorizationHandler>();

                    services.AddAutoMapper(Assembly.GetExecutingAssembly());

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
