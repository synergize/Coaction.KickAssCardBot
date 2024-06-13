using Coaction.KickAssCardBot.Enums;

namespace Coaction.KickAssCardBot.Extensions
{
    public static class StringExtensions
    {
        public static string CapitalizeFirstLetter(this string word)
        {
            return char.ToUpper(word[0]) + word.Substring(1);
        }

        public static string CapitalizeFirstLetter(this MtgFormatsEnum format)
        {
            return CapitalizeFirstLetter(format.ToString());
        }

        public static MtgFormatsEnum ToEnum(this string formatName)
        {
            switch (formatName)
            {
                case "standard":
                    return MtgFormatsEnum.standard;
                case "modern":
                        return MtgFormatsEnum.modern;
                case "pioneer":
                        return MtgFormatsEnum.pioneer;
                case "pauper":
                        return MtgFormatsEnum.pauper;
                case "legacy":
                        return MtgFormatsEnum.legacy;
                case "vintage":
                        return MtgFormatsEnum.vintage;
                default:
                    throw new NotImplementedException($"{formatName} isn't yet supported");
            }
        }

        public static string ToScryfallString(this GameType gameType)
        {
            return gameType.ToString().ToLower();
        }

        public static string Multiply(this char characterName, int multiplyNumber)
        {
            return String.Concat(Enumerable.Repeat(characterName, multiplyNumber));
        }
    }
}
