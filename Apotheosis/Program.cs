using Apotheosis.Components.Client.DependencyInjection;
using Apotheosis.Components.Client.Interfaces;
using Apotheosis.Components.Logging.DependencyInjection;
using Apotheosis.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
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
            services.AddClientServices(_configuration.GetSection(nameof(AppSettings.Client)));
        }
    }
}
