namespace Coaction.KickAssCardBot.Helpers.Scryfall
{
    public static class FormatUserInput
    {
        public static string FormatEntry(string entry)
        {
            if (entry.Contains('[') || entry.Contains(']'))
            {
                entry = entry.TrimStart().TrimEnd().ToUpper();
                entry = entry.Remove(0, entry.Contains('?') ? 3 : 2);
                var count = entry.Length - 2;
                entry = entry.Remove(count, 2);
                if (entry.Contains(" "))
                {
                    entry = entry.Replace(" ", "+");
                }
            }
            return entry;
        }
    }
}
