using System.Reflection;
using Coaction.KickAssCardBot.Models.Scryfall;
using Newtonsoft.Json;

namespace Coaction.KickAssCardBot.Helpers
{
    public static class EmojiHelper
    {
        public static MtgEmoji GetSavedDiscordEmojis()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{assembly.GetName().Name}.MtgEmojiIds.json";

            Stream? stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                return null;
            }

            using StreamReader reader = new StreamReader(stream);
            string jsonFile = reader.ReadToEnd();

            return JsonConvert.DeserializeObject<MtgEmoji>(jsonFile);
        }
    }
}
