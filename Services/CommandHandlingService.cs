﻿using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Coaction.KickAssCardBot.Services;

public class CommandHandlingService
{
    private readonly CommandService _commands;
    private readonly DiscordSocketClient _discord;
    private readonly IServiceProvider _services;

    public CommandHandlingService(IServiceProvider services)
    {
        _commands = services.GetRequiredService<CommandService>();
        _discord = services.GetRequiredService<DiscordSocketClient>();
        _services = services;

        _commands.CommandExecuted += CommandExecutedAsync;
        _discord.MessageReceived += MessageReceivedAsync;
    }

    public async Task InitializeAsync()
    {
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }

    private async Task MessageReceivedAsync(SocketMessage rawMessage)
    {
        if (rawMessage is not SocketUserMessage { Source: MessageSource.User } message)
            return;
        
        var argPos = 0;

        if (!message.HasStringPrefix(Environment.GetEnvironmentVariable("DISCORD_BOT_COMMAND_PREFIX") ?? "!", ref argPos)
            || !message.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            return;

        var context = new SocketCommandContext(_discord, message);

        await _commands.ExecuteAsync(context, argPos, _services);
    }

    private static async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
    {
        if (!command.IsSpecified)
            return;

        if (result.IsSuccess)
            return;

        await context.Channel.SendMessageAsync($"Command execution failed due to: {result}");
    }
}