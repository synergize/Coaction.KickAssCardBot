namespace Coaction.KickAssCardBot.Helpers.Scryfall
{
    public static class LegalityDictionary
    {
        public static Dictionary<string, string> Legality = new()
        {
            {"not_legal", "Not Legal"},
            {"legal", "Legal"},
            {"banned", "Banned"},
            {"restricted", "Restricted"}
        };
    }
}
