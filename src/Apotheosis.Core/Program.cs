using Apotheosis.Core.Configuration;
using Apotheosis.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord;
using NetCord.Services.ApplicationCommands;
using NetCord.Hosting.Services;
using NetCord.Gateway;
using Apotheosis.Core.Components.AiChat.Extensions;
using Apotheosis.Core.Components.Audio.Extensions;
using Apotheosis.Core.Components.Converse.Extensions;
using Apotheosis.Core.Components.DateTime.Extensions;
using Apotheosis.Core.Components.EmojiCounter.Extensions;
using Apotheosis.Core.Components.GcpDot.Extensions;
using Apotheosis.Core.Components.ImageGen.Extensions;
using Apotheosis.Core.Components.Logging.Extensions;
using Apotheosis.Core.Components.TextToSpeech.Extensions;

var builder = Host.CreateDefaultBuilder(args)
    .UseDiscordGateway(options =>
    {
        options.Configuration = new()
        {
            Intents = GatewayIntents.AllNonPrivileged | GatewayIntents.MessageContent
        };
    })
    .UseApplicationCommandService<SlashCommandInteraction, SlashCommandContext>();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

builder.ConfigureServices(services =>
{
    services.AddLoggingServices();
    services.AddApotheosisDbContext(configuration.GetConnectionString("Apotheosis"));
    services.AddGcpDotServices(configuration.GetSection(nameof(AppSettings.GcpDot)));
    services.AddTextToSpeechServices(configuration.GetSection(nameof(AppSettings.TextToSpeech)));
    services.AddAudioServices();
    services.AddImageGenServices(configuration.GetSection(nameof(AppSettings.ImageGen)));
    services.AddDateTimeServices();
    services.AddAiChatServices(configuration.GetSection(nameof(AppSettings.AiChat)));
    services.AddConverseServices(configuration.GetSection(nameof(AppSettings.Converse)));
    services.AddEmojiCounterServices();
    services.AddGatewayEventHandlers(typeof(Program).Assembly);
});


var host = builder.Build()
    .AddModules(typeof(Program).Assembly)
    .UseGatewayEventHandlers();

MigrationManager.MigrateDatabase(host.Services);

await host.RunAsync();
