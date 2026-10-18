using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FamilyManagement.Application.Interfaces;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Infrastructure.Persistence;
using FamilyManagement.Infrastructure.Persistence.Repositories;
using FamilyManagement.Infrastructure.Storage;

namespace FamilyManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ReceiptDbContext>((sp, options) =>
        {
            var config = sp.GetService<IConfiguration>() ?? configuration;
            var tenant = sp.GetService<ITenantContext>();
            var isDemo = tenant?.Role == WorkspaceRole.Demo;

            var connectionString = isDemo
                ? (config.GetConnectionString("DemoDatabase") ?? "Data Source=demo.db")
                : (config.GetConnectionString("ReceiptDatabase") ?? "Data Source=receipts.db");

            options.UseSqlite(connectionString);
        });

        services.AddScoped<IReceiptRepository, ReceiptRepository>();
        services.AddScoped<IInsurancePolicyRepository, InsurancePolicyRepository>();
        services.AddScoped<IChildProfileRepository, FamilyManagement.Infrastructure.Repositories.ChildProfileRepository>();
        services.AddScoped<IGrowthMeasurementRepository, FamilyManagement.Infrastructure.Repositories.GrowthMeasurementRepository>();
        services.AddScoped<IChildMilestoneRepository, FamilyManagement.Infrastructure.Repositories.ChildMilestoneRepository>();
        services.AddScoped<IVehicleRepository, FamilyManagement.Infrastructure.Repositories.VehicleRepository>();
        services.AddScoped<IMileageLogRepository, FamilyManagement.Infrastructure.Repositories.MileageLogRepository>();
        services.AddScoped<IVehicleServiceRepository, FamilyManagement.Infrastructure.Repositories.VehicleServiceRepository>();
        services.AddScoped<IFreezerRepository, FreezerRepository>();
        services.AddScoped<IFreezerItemRepository, FreezerItemRepository>();

        services.Configure<GoogleDriveOptions>(
            configuration.GetSection(GoogleDriveOptions.SectionName));

        services.AddSingleton<IGoogleDriveAuthService, GoogleDriveAuthService>();
        services.AddScoped<IReceiptFileStorageService, GoogleDriveStorageService>();

        services.AddSingleton<IDemoDataSeeder, DemoDataSeeder>();
        services.AddHostedService<DemoResetHostedService>();

        return services;
    }
}
