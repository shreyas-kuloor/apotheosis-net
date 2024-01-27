using Apotheosis.Core.Components.EmojiCounter.Interfaces;
using Apotheosis.Core.Components.EmojiCounter.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Apotheosis.Core.Components.EmojiCounter.DependencyInjection;

public static class EmojiCounterExtensions
{
    /// <summary>
    /// Adds emoji counter related services.
    /// </summary>
    /// <param name="services"> The instances of <see cref="IServiceCollection"/>.</param>
    public static void AddEmojiCounterServices(this IServiceCollection services)
    {
        services.AddScoped<IEmojiCounterService, EmojiCounterService>();
        services.AddSingleton<IEmojiCounterReactionHandler, EmojiCounterReactionHandler>();
    }
}