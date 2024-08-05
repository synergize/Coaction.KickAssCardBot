using Coaction.KickAssCardBot.Services;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Coaction.KickAssCardBot
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<DiscordSocketClient>();
                    services.AddHostedService<BotService>();
                    services.AddScoped<CommandHandlingService>();
                    services.AddScoped<InteractionHandlingService>();
                    services.AddSingleton<CommandService>();
                    services.AddSingleton<InteractionService>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                });
    }
}
