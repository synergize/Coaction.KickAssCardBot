using Coaction.KickAssCardBot.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Coaction.KickAssCardBot;

public class Startup
{
    private DiscordSocketClient _client;

    public async Task Initialize()
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        builder.Services.AddSingleton<DiscordSocketClient>();
        builder.Services.AddSingleton<CommandService>();
        builder.Services.AddSingleton<CommandHandlingService>();
        builder.Services.AddSingleton<InteractionService>();
        builder.Services.AddSingleton<InteractionHandlingService>();
        var host = builder.Build();
        _client = host.Services.GetRequiredService<DiscordSocketClient>();

        _client.Ready += OnReady;
        _client.Log += LogAsync;
        host.Services.GetRequiredService<CommandService>().Log += LogAsync;

        await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("TEST_DISCORD_BOT_TOKEN"));
        await _client.StartAsync();

        await host.Services.GetRequiredService<CommandHandlingService>().InitializeAsync();
        await host.Services.GetRequiredService<InteractionHandlingService>().InitializeAsync();

        await host.RunAsync();
    }

    private Task OnReady()
    {
        foreach (var guild in _client.Guilds)
        {
            Console.WriteLine($"- {guild.Name}");
        }

        _client.SetGameAsync(Environment.GetEnvironmentVariable("DISCORD_BOT_ACTIVITY") ?? "Monitoring GTA Server(s).",
            type: ActivityType.CustomStatus);
        
        Console.WriteLine($"Activity set to '{_client.Activity.Name}'");

        return Task.CompletedTask;
    }

    private static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());

        return Task.CompletedTask;
    }
}