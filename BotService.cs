using Coaction.KickAssCardBot.Services;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Coaction.KickAssCardBot;

public class BotService : IHostedService
{
    private readonly ILogger<BotService> _logger;
    private readonly DiscordSocketClient _client;
    private readonly CommandHandlingService _commandHandlingService;
    private readonly InteractionHandlingService _interactionHandlingService;
    private readonly IConfiguration _configuration;


    public BotService(ILogger<BotService> logger, IConfiguration configuration, DiscordSocketClient client, CommandHandlingService commandHandlingService, InteractionHandlingService interactionHandlingService)
    {
        _configuration = configuration;
        _logger = logger;
        _client = client;
        _commandHandlingService = commandHandlingService;
        _interactionHandlingService = interactionHandlingService;
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

    private Task LogAsync(LogMessage log)
    {
        _logger.LogInformation(log.ToString());
        return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.Log += LogAsync;
        _client.MessageReceived += _commandHandlingService.MessageReceivedAsync;
        _client.InteractionCreated += _interactionHandlingService.HandleInteractionAsync;
        _client.SelectMenuExecuted += _interactionHandlingService.Client_SelectMenuExecuted;
        _client.ButtonExecuted += _interactionHandlingService.Client_ButtonExecuted;
        _client.Ready += OnReady;
        await _commandHandlingService.InitializeAsync();
        await _interactionHandlingService.InitializeAsync();

        //var token = _configuration["DiscordToken"];
        var token = @"OTA1NjY4NjgyNTYxMjQ1MjA2.GPPbXL.hWAFWxkdV8x1bG5A0R4XC0rcF3AQeJDlhP7Ams";
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _client.LogoutAsync();
    }
}