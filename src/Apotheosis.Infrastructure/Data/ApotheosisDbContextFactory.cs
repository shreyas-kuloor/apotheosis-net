using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Apotheosis.Infrastructure.Data;

public class ApotheosisDbContextFactory : IDesignTimeDbContextFactory<ApotheosisDbContext>
{
    ApotheosisDbContext IDesignTimeDbContextFactory<ApotheosisDbContext>.CreateDbContext(string[] args) =>
        BuildApotheosisDbContext();

    private static ApotheosisDbContext BuildApotheosisDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApotheosisDbContext>();

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../"))
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("Apotheosis");

        _ = optionsBuilder.UseNpgsql(connectionString);
        return new ApotheosisDbContext(optionsBuilder.Options);
    }
}