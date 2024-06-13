//using System;
//using System.IO;
//using System.Net;
//using System.Threading.Tasks;

//namespace MTGBot.Helpers.Scryfall
//{
//    static class DownloadSymbols
//    {
//        public static async Task DownloadAllScryfallSymbols(string directoryName = "MtgSymbols")
//        {
//            var directoryPath = Path.Combine(Path.GetFullPath(Directory.GetCurrentDirectory()), directoryName);
//            var symbols = await GetScryFallData.PullScryfallSymbology();
//            var webClient = new WebClient();

//            foreach (var symbol in symbols.Data)
//            {
//                try
//                {
//                    var symbolName = symbol.Symbol.Replace("{", "").Replace("}", "").Replace("/", "");
//                    webClient.DownloadFile(new Uri(symbol.Svg_uri), Path.Combine(directoryPath, $"{symbolName}.svg"));
//                }
//                catch (Exception e)
//                {
//                    Logger.Log.Debug($"Couldn't Download Symbol: {symbol.Symbol}, URI: {symbol.Svg_uri}", e);
//                }
//            }
//        }
//    }
//}
