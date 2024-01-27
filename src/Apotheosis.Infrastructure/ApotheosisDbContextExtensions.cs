using Apotheosis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Apotheosis.Infrastructure;

public static class ApotheosisDbContextExtensions
{
    /// <summary>
    /// Adds infrastructure related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    public static void AddApotheosisDbContext(this IServiceCollection services, string? connectionString)
    {   
        services.AddDbContext<ApotheosisDbContext>(options => 
            options.UseNpgsql(connectionString));
    }
}