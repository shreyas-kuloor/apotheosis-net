using Apotheosis.Core.Components.AiChat.DependencyInjection;
using Apotheosis.Core.Components.Audio.DependencyInjection;
using Apotheosis.Core.Components.Client.DependencyInjection;
using Apotheosis.Core.Components.Converse.DependencyInjection;
using Apotheosis.Core.Components.DateTime.DependencyInjection;
using Apotheosis.Core.Components.GCPDot.DependencyInjection;
using Apotheosis.Core.Components.ImageGen.DependencyInjection;
using Apotheosis.Core.Components.Logging.DependencyInjection;
using Apotheosis.Core.Components.TextToSpeech.DependencyInjection;
using Apotheosis.Core.Components.Client.Interfaces;
using Apotheosis.Core.Configuration;
using Apotheosis.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Apotheosis.Core.Components.EmojiCounter.DependencyInjection;

namespace Apotheosis.Core
{
    public sealed class Program
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public Program()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            MigrationManager.MigrateDatabase(_serviceProvider);
        }

        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var clientService = _serviceProvider.GetRequiredService<IClientService>();
            await clientService.RunAsync();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder => builder.AddConsole());
            services.AddLoggingServices();
            services.AddApotheosisDbContext(_configuration.GetConnectionString("Apotheosis"));
            services.AddClientServices(_configuration.GetSection(nameof(AppSettings.Client)).GetValue<string>("BotToken")!);
            services.AddGcpDotServices(_configuration.GetSection(nameof(AppSettings.GcpDot)));
            services.AddTextToSpeechServices(_configuration.GetSection(nameof(AppSettings.TextToSpeech)));
            services.AddAudioServices();
            services.AddImageGenServices(_configuration.GetSection(nameof(AppSettings.ImageGen)));
            services.AddDateTimeServices();
            services.AddAiChatServices(_configuration.GetSection(nameof(AppSettings.AiChat)));
            services.AddConverseServices(_configuration.GetSection(nameof(AppSettings.Converse)));
            services.AddEmojiCounterServices();
        }
    }
}
