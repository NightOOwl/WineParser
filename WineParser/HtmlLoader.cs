using AngleSharp;
using AngleSharp.Dom;

namespace WineParser
{
    public class HtmlLoader
    {
        private readonly Dictionary<string,string> queryParameters;
        private readonly HttpClient client;

        public HtmlLoader(Dictionary<string, string> headers, Dictionary<string, string> queryParameters)
        {
            client = new HttpClient();

            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            this.queryParameters = queryParameters;
        }


        public async Task<IDocument> LoadHtml(string url)
        {
            //var queryString = string.Join("&", queryParameters.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
            url = $"{url}?{queryParameters.Select(x=>x.Key)}={queryParameters.Select(x=>x.Value)}";

            var response = await client.GetAsync(url);

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();
            var document = await context.OpenAsync(req => req.Content(html));

            return document;
        } 
    }
    
}
