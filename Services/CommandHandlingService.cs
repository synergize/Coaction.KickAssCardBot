using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Coaction.KickAssCardBot.Services;

public class CommandHandlingService
{
    private readonly CommandService _commands;
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _services;
    private readonly ILogger<CommandHandlingService> _logger;

    public CommandHandlingService(ILogger<CommandHandlingService> logger, CommandService commands, DiscordSocketClient client, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _commands = commands;
        _services = serviceProvider;
        _client = client;

        _commands.CommandExecuted += CommandExecutedAsync;
        _client.MessageReceived += MessageReceivedAsync;
    }

    public async Task InitializeAsync()
    {
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }

    public async Task MessageReceivedAsync(SocketMessage rawMessage)
    {
        if (rawMessage is SocketUserMessage {Source: MessageSource.User} message)
        {
            var argPos = 0;

            if (!message.HasStringPrefix(Environment.GetEnvironmentVariable("DISCORD_BOT_COMMAND_PREFIX") ?? "!", ref argPos)
                || !message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);

            await _commands.ExecuteAsync(context, argPos, _services);
        }
    }

    public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
    {
        if (!command.IsSpecified)
            return;

        if (result.IsSuccess)
            return;

        await context.Channel.SendMessageAsync($"Command execution failed due to: {result}");
        _logger.LogError($"Command execution failed due to: {result}");
    }
}