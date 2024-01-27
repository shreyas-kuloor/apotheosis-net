using Apotheosis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Apotheosis.Infrastructure;

public static class MigrationManager
{
    public static void MigrateDatabase(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApotheosisDbContext>();
        dbContext.Database.Migrate();
    }
}
