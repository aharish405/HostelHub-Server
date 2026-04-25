using HostelHub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HostelHub.Tests.Integration.Common;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Using your specified SQL Server instance for integration tests (different DB name)
    public string DbConnectionString => "Data Source=tcp:HARRISH-PC;Initial Catalog=HostelHub_Tests;User ID=sa;Password=Admin@12345;Connect Timeout=30;Encrypt=False";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(DbConnectionString));
        });
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Ensure we start fresh
        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.EnsureDeletedAsync();
    }
}
