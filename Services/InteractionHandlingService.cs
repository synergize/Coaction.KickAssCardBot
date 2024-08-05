using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Coaction.KickAssCardBot.Services
{
    public class InteractionHandlingService
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InteractionHandlingService> _logger;

        public InteractionHandlingService(ILogger<InteractionHandlingService> logger, DiscordSocketClient client, InteractionService interactionService,
            IServiceProvider serviceProvider)
        {
            _client = client;
            _interactionService = interactionService;
            _serviceProvider = serviceProvider;
            _logger = logger;

            _client.Ready += ClientOnReadyAsync;
            _client.InteractionCreated += HandleInteractionAsync;
        }

        public async Task InitializeAsync()
        {
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        }

        public async Task ClientOnReadyAsync()
        {
            if (_client.Guilds.Count > 0)
            {
                foreach (var guild in _client.Guilds)
                {
                    await _interactionService.RegisterCommandsToGuildAsync(guild.Id);
                }
            }
            else
            {
                await _interactionService.RegisterCommandsGloballyAsync();
            }
        }

        public async Task HandleInteractionAsync(SocketInteraction arg)
        {
            try
            {
                var context = new SocketInteractionContext(_client, arg);
                await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to handle Interaction due to: {e.Message}");
            }
        }
    }
}
