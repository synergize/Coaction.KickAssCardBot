namespace Coaction.KickAssCardBot.Exceptions
{
    [Serializable]
    public class UnableToConvertDiceInputException : Exception
    {
        public UnableToConvertDiceInputException(string diceInput) : base($"Unable to parse {diceInput}. Make sure your dice follow this example: 1d20")
        {

        }
    }
}
