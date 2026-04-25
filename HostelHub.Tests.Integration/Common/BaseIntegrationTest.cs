using System.Net.Http.Headers;
using HostelHub.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Xunit;

namespace HostelHub.Tests.Integration.Common;

[Collection("Integration")]
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    protected readonly IntegrationTestFactory Factory;
    protected readonly HttpClient Client;
    private Respawner _respawner = null!;
    private SqlConnection _connection = null!;

    protected BaseIntegrationTest(IntegrationTestFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Open a connection for Respawn
        _connection = new SqlConnection(Factory.DbConnectionString);
        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" },
            DbAdapter = DbAdapter.SqlServer
        });

        await _respawner.ResetAsync(_connection);
    }

    public async Task DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
    }

    protected void AuthenticateAs(string token, string tenantId)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        Client.DefaultRequestHeaders.Remove("X-Tenant-ID");
        Client.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId);
    }
}

[CollectionDefinition("Integration")]
public class IntegrationCollection : ICollectionFixture<IntegrationTestFactory> { }
