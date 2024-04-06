using Newtonsoft.Json;
using System.Collections.Concurrent;

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
            var domainAdress = "https://simplewine.ru";
            var relativeAdress = "catalog/shampanskoe_i_igristoe_vino";

            using var htmlLoader = new HtmlLoader(Headers);

            var mainPageParser = new MainPageParser(htmlLoader, domainAdress, relativeAdress, 2);
            var productLinks = await mainPageParser.ParseProductsLinksAsync();
            var products = new ConcurrentBag<Product>();
            var productParser = new ProductParser();
            var options = new ParallelOptions() { MaxDegreeOfParallelism = 3 };
            var processedProductCount = 0;

            await Parallel.ForEachAsync(productLinks, options, async (url, cancellationToken) =>
            {
                try
                {
                    var html = await htmlLoader.LoadHtml(url);
                    var product = await productParser.ParseProductAsync(html, url);
                    products.Add(product);
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync($"Не удалось собрать необходимую информацию по товару: {url}");
                    return;
                }
                Interlocked.Increment(ref processedProductCount);
                await Console.Out.WriteLineAsync($"Прогресс: {processedProductCount}/{productLinks.Count}");
            });

            var productsJson = JsonConvert.SerializeObject(products, Formatting.Indented);

            var fileWriter = new FileWriter();
            await fileWriter.WriteToFileAsync("productsSPb.json", productsJson);
        }
       
    }
}
