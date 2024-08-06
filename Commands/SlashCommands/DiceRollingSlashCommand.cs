using Coaction.KickAssCardBot.Exceptions;
using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace Coaction.KickAssCardBot.Commands.SlashCommands
{
    public class DiceRollingSlashCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ILogger<BotService> _logger;

        public DiceRollingSlashCommand(ILogger<BotService> logger)
        {
            _logger = logger;
        }

        [SlashCommand("roll", "Example: 5d4")]
        public async Task RollDice(string diceRoll)
        {
            try
            {
                var dice = diceRoll.Split('d');

                if (dice.Length != 2)
                {
                    throw new UnableToConvertDiceInputException(diceRoll);
                }

                var random = new Random(DateTime.UtcNow.Millisecond);
                var diceRolled = new List<int>();
                var tryNumberOfDice = int.TryParse(dice[0], out var numberOfDice);
                var tryTypeOfDice = int.TryParse(dice[1], out var typeOfDice);


                if (tryNumberOfDice && tryTypeOfDice)
                {
                    for (var i = 0; i < numberOfDice; i++)
                    {
                        var rolledValue = random.Next(1, typeOfDice);
                        diceRolled.Add(rolledValue);
                    }
                }
                else
                {
                    throw new UnableToConvertDiceInputException(diceRoll);
                }

                if (diceRolled.Count > 0)
                {
                    var diceTotal = diceRolled.Sum(Convert.ToInt32);
                    var diceRolledOutput = diceRolled.Aggregate("", (current, dice) => current + $"{dice} + ");

                    diceRolledOutput = diceRolledOutput.Trim();
                    diceRolledOutput = diceRolledOutput.Trim('+');
                    diceRolledOutput += $" = {diceTotal}";
                    await Context.Interaction.RespondAsync($"{diceRoll}: {diceRolledOutput}");
                }
            }
            catch (UnableToConvertDiceInputException e)
            {
                await Context.Interaction.RespondAsync(e.Message);
                _logger.LogWarning($"Failed to convert dice rolling input due to {e.Message}", e);
            }
            catch (Exception e) 
            {
                _logger.LogError($"Failed to convert dice rolling input due to {e.Message}", e);
            }
        }
    }
}
