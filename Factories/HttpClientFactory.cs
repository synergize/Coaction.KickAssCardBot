namespace Coaction.KickAssCardBot.Factories
{
    public static class HttpClientFactory
     {
        public static HttpClient GetValidHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
            client.DefaultRequestHeaders.Add("Accept", "application/json;q=0.9,*/*;q=0.8");

            return client;
        }

        public static HttpClient GetValidMtgMeleeClient() {

            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new System.Net.CookieContainer()
            };
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:134.0) Gecko/20100101 Firefox/134.0");
            client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");

            return client;
        }
    }
}
