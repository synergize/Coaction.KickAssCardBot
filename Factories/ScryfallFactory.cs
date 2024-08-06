namespace Coaction.KickAssCardBot.Factories
{
    public static class ScryfallFactory
     {
        public static HttpClient GetSryfallHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
            client.DefaultRequestHeaders.Add("Accept", "application/json;q=0.9,*/*;q=0.8");

            return client;
        }
    }
}
