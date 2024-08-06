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
            var resourceName = $"{assembly.GetName().Name}.Resources.MtgEmojiIds.json";
            var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                throw new Exception("Unable to obtain MtgEmojiIds.json as an embedded resource.");
            }

            using var reader = new StreamReader(stream);
            var jsonFile = reader.ReadToEnd();

            return JsonConvert.DeserializeObject<MtgEmoji>(jsonFile) ?? new MtgEmoji();
        }
    }
}
