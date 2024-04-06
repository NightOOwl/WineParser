using AngleSharp;
using AngleSharp.Dom;

namespace WineParser
{
    public class HtmlLoader : IDisposable
    {
        private readonly HttpClient client;

        public HtmlLoader(Dictionary<string, string> headers)
        {
            client = new HttpClient();

            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }           
        }

        public void Dispose()
        {
            client.Dispose();
        }

        public async Task<IDocument> LoadHtml(string url)
        {          
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
