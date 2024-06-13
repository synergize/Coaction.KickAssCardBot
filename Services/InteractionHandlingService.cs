using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;

namespace Coaction.KickAssCardBot.Services
{
    internal class InteractionHandlingService
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _serviceProvider;

        public InteractionHandlingService(DiscordSocketClient client, InteractionService interactionService,
            IServiceProvider serviceProvider)
        {
            _client = client;
            _interactionService = interactionService;
            _serviceProvider = serviceProvider;
        }

        public async Task InitializeAsync()
        {
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

            _client.InteractionCreated += HandleInteraction;
            _client.Ready += ClientOnReady;
        }

        private async Task ClientOnReady()
        {
            await _interactionService.RegisterCommandsToGuildAsync(489193860698734596);
            _client.Ready -= ClientOnReady;
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                var context = new SocketInteractionContext(_client, arg);
                await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to handle Interaction due to: {e.Message}"); }
        }
    }
}
