//using Discord.Interactions;

//namespace Coaction.KickAssCardBot.Commands.SlashCommands
//{
//    public class DiceRollingSlashCommand : InteractionModuleBase<SocketInteractionContext>
//    {
//        [SlashCommand("roll", "Example: 5d4")]
//        public async Task RollDice(string diceRoll)
//        {
//            try
//            {
//                var diceRolled = DiceRollingManager.CalculateDiceToRole(diceRoll.ToLower());

//                if (diceRolled != null)
//                {
//                    var diceTotal = diceRolled.Sum(Convert.ToInt32);
//                    var diceRolledOutput = diceRolled.Aggregate("", (current, dice) => current + $"{dice} + ");

//                    diceRolledOutput = diceRolledOutput.Trim();
//                    diceRolledOutput = diceRolledOutput.Trim('+');
//                    diceRolledOutput += $" = {diceTotal}";
//                    await Context.Interaction.RespondAsync($"{diceRoll}: {diceRolledOutput}");
//                }
//            }
//            catch (UnableToConvertDiceInputException e)
//            {
//                await Context.Interaction.RespondAsync(e.Message);
//                DiscordNetHelperLibrary.Logger.Log.Debug(e, $"User failed to roll {diceRoll} due to {e.Message}");
//            }
//        }
//    }
//}
