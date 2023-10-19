using Apotheosis.Configuration;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Apotheosis
{
    public class Program
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public Program()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, _configuration);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public static void Main(string[] args) => new Program().RunAsync().GetAwaiter().GetResult();

        public async Task RunAsync()
        {
            var client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
            var discordOptions = _serviceProvider.GetRequiredService<DiscordOptions>();

            client.Log += Log;

            await client.LoginAsync(TokenType.Bot, discordOptions.BotToken);
            await client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }



        private static void ConfigureServices(ServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<DiscordOptions>(configuration.GetSection(nameof(DiscordOptions)));
            serviceCollection.AddSingleton<DiscordSocketClient>();
        }
    }
}