using Apotheosis.Components.Audio.Interfaces;
using Apotheosis.Components.Audio.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Apotheosis.Components.Audio.DependencyInjection;

public static class AudioExtensions
{
    /// <summary>
    /// Adds client related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    public static void AddAudioServices(this IServiceCollection services)
    {
        services.AddScoped<IAudioStreamService, AudioStreamService>();
        services.AddSingleton<IVoiceClientService, VoiceClientService>();
    }
}