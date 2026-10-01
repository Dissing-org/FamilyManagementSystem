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
        var connectionString = configuration.GetConnectionString("ReceiptDatabase") ?? "Data Source=receipts.db";

        services.AddDbContext<ReceiptDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IReceiptRepository, ReceiptRepository>();
        services.AddScoped<IInsurancePolicyRepository, InsurancePolicyRepository>();

        services.Configure<GoogleDriveOptions>(
            configuration.GetSection(GoogleDriveOptions.SectionName));

        services.AddScoped<IReceiptFileStorageService, GoogleDriveStorageService>();

        return services;
    }
}
