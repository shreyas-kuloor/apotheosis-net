var builder = Host.CreateDefaultBuilder(args)
    .UseDiscordGateway(options =>
    {
        options.Intents = GatewayIntents.AllNonPrivileged | GatewayIntents.MessageContent;
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
    services.AddApotheosisDbContext(configuration.GetConnectionString("Apotheosis"));
    services.AddGcpDotServices(configuration);
    services.AddTextToSpeechServices(configuration);
    services.AddAiChatServices(configuration);
    services.AddMediaRequestServices(configuration);
    services.AddRankServices(configuration);
    services.AddRecapServices(configuration);
    services.AddFeatureFlagServices(configuration);
    services.AddGatewayEventHandlers(typeof(Program).Assembly);
    services.AddServicesWithAttributes(typeof(Program).Assembly);
});


var host = builder.Build()
    .AddModules(typeof(Program).Assembly)
    .UseGatewayEventHandlers();

MigrationManager.MigrateDatabase(host.Services);

await host.RunAsync();
