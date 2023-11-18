using Apotheosis.Components.ImageUpload.Configuration;
using Apotheosis.Components.ImageUpload.Interfaces;
using Apotheosis.Components.ImageUpload.Network;
using Apotheosis.Components.ImageUpload.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Apotheosis.Components.ImageUpload.DependencyInjection;

public static class ImageUploadExtensions
{
    /// <summary>
    /// Adds client related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    /// <param name="imageUploadSection">An instance of <see cref="IConfigurationSection"/>.</param>
    public static void AddImageUploadServices(
        this IServiceCollection services,
        IConfigurationSection imageUploadSection)
    {
        services.Configure<ImageUploadSettings>(imageUploadSection);
        AddHttpClient<IImageUploadNetworkDriver, ImgurNetworkDriver>(services);
        services.AddScoped<IImageUploadService, ImageUploadService>();
    }

    private static void AddHttpClient<TClient, TImplementation>(
        IServiceCollection services)
        where TClient : class
        where TImplementation : class, TClient
    {
        services.AddHttpClient<TClient, TImplementation>()
            .AddPolicyHandler((handlerServices, _) => HttpPolicyExtensions
                .HandleTransientHttpError()
                .RetryAsync(
                    ImgurNetworkDriver.Retries,
                    (result, _) =>
                    {
                        var logger = handlerServices.GetService<ILogger<ImgurNetworkDriver>>();
                        logger?.LogError(result.Exception, "[{Source}] {Message}", nameof(ImgurNetworkDriver), result.Result?.ToString());
                    }))
            .AddPolicyHandler(_ => Policy.TimeoutAsync<HttpResponseMessage>(ImgurNetworkDriver.Timeout));
    }
}