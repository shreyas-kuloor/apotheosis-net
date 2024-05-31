using Apotheosis.Core.Features.MediaRequest.Extensions;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Services.ComponentInteractions;

var builder = Host.CreateDefaultBuilder(args)
    .UseDiscordGateway(options =>
    {
        options.Configuration = new()
        {
            Intents = GatewayIntents.AllNonPrivileged | GatewayIntents.MessageContent
        };
    })
    .UseApplicationCommands<SlashCommandInteraction, SlashCommandContext>()
    .UseComponentInteractions<StringMenuInteraction, StringMenuInteractionContext>()
    .UseComponentInteractions<ButtonInteraction, ButtonInteractionContext>();

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
    services.AddMediaRequestServices(configuration);
    services.AddGatewayEventHandlers(typeof(Program).Assembly);
    services.AddServicesWithAttributes(typeof(Program).Assembly);
});


var host = builder.Build()
    .AddModules(typeof(Program).Assembly)
    .UseGatewayEventHandlers();

MigrationManager.MigrateDatabase(host.Services);

await host.RunAsync();
