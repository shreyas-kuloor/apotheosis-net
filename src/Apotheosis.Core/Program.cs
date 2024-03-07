using Apotheosis.Core.Features.AiChat.Extensions;
using Apotheosis.Core.Features.Converse.Extensions;
using Apotheosis.Core.Features.GcpDot.Extensions;
using Apotheosis.Core.Features.ImageGen.Extensions;
using Apotheosis.Core.Features.Logging.Extensions;
using Apotheosis.Core.Features.TextToSpeech.Extensions;

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
    services.AddGcpDotServices(configuration);
    services.AddTextToSpeechServices(configuration);
    services.AddImageGenServices(configuration);
    services.AddAiChatServices(configuration);
    services.AddConverseServices(configuration);
    services.AddGatewayEventHandlers(typeof(Program).Assembly);
    services.AddServicesWithAttributes(typeof(Program).Assembly);
});


var host = builder.Build()
    .AddModules(typeof(Program).Assembly)
    .UseGatewayEventHandlers();

MigrationManager.MigrateDatabase(host.Services);

await host.RunAsync();
