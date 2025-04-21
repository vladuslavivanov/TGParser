using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Testcontainers.PostgreSql;
using TGParser.DAL;

namespace TGParser.Migrations;

internal class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    private static readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithDatabase("testdb")
        .WithUsername("testuser")
        .WithPassword("testpassword")
        .WithImage("postgres:17.4")
        .Build();

    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

        if (_postgresContainer.State != DotNet.Testcontainers.Containers.TestcontainersStates.Running)
        {
            _postgresContainer.StartAsync().GetAwaiter().GetResult();
        }

        optionsBuilder.UseNpgsql(_postgresContainer.GetConnectionString());

        return new DataContext(optionsBuilder.Options);
    }
}
