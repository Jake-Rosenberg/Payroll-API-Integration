using AD_Integration_Application.Config;
using AD_Integration_Application.Services;
using Domain.Entities.AD;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using UKG_Integration_Application.Database.BaseRepo;

IServiceConfiguration serviceConfiguration = new ServiceConfiguration();
var serviceProvider = serviceConfiguration.ConfigureServices();

var adService = serviceProvider.GetRequiredService<IADService>();
var adBaseRepo = serviceProvider.GetRequiredService<IBaseRepository<ADProfile>>();
var mssqlContext = serviceProvider.GetRequiredService<IMSSQLContext>();


// STEPS:

// 1. Get all employees from AD
var pdpEmployees = await adService.GetUsers();

// 2. Truncate AD_Employees table
await mssqlContext.TruncateTables();

// 3. Save them to DB
await adBaseRepo.AddRangeAsync(pdpEmployees, CancellationToken.None);
await adBaseRepo.SaveChangesAsync();