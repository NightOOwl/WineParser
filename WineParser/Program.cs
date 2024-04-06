
namespace WineParser
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var Headers = new Dictionary<string, string>
            {
                { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36" },
                { "Referer", "https://simplewine.ru/" }
            };

            var Parameters = new Dictionary<string, string>
            {
                { "setVisitorCityId", "2" }
            };
            var domainAdress = "https://simplewine.ru";
            var relativeAdress = "catalog/shampanskoe_i_igristoe_vino";


            var htmlLoader = new HtmlLoader(Headers, Parameters);

            var mainPageParser = new MainPageParser(htmlLoader,domainAdress,relativeAdress);

            var productLinks = await mainPageParser.ParseProductsLinksAsync();
            List<Product> products = new List<Product>();

            var productParser = new ProductParser();
            foreach (var link in productLinks)
            {
               var html = await htmlLoader.LoadHtml(link);
               var product = await productParser.ParseProductAsync(html, link);
               products.Add(product);
            }
        }
    }
}
