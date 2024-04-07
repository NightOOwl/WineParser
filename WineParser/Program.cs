using Newtonsoft.Json;
using Polly;
using System.Collections.Concurrent;

namespace WineParser
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var Headers = new Dictionary<string, string>
            {
                { "User-Agent", "Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36" },
                { "Referer", "https://simplewine.ru/" }
            };
            var domainAdress = "https://simplewine.ru";
            var relativeAdress = "catalog/shampanskoe_i_igristoe_vino";

            var retryPolicy = Policy.Handle<Exception>()
                               .WaitAndRetryAsync(5,
                                   retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                   (exception, timeSpan, retryCount, context) =>
                                   {                                      
                                       Console.WriteLine($"Ошибка при выполнении запроса. Попытка {retryCount}...");
                                   });

            using var htmlLoader = new HtmlLoader(Headers);

            var mainPageParser = new MainPageParser(htmlLoader, domainAdress, relativeAdress, 5);
            var productLinks = await retryPolicy.ExecuteAsync(mainPageParser.ParseProductsLinksAsync);
            var products = new ConcurrentBag<Product>();
            var productParser = new ProductParser();
            var options = new ParallelOptions() { MaxDegreeOfParallelism = 3 };
            var processedProductCount = 0;

            await Parallel.ForEachAsync(productLinks, options, async (url, cancellationToken) =>
            {
                try
                {
                    var html = await retryPolicy.ExecuteAsync(async () => await htmlLoader.LoadHtml(url));
                    var product = await retryPolicy.ExecuteAsync(async () => await productParser.ParseProductAsync(html, url));
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
            await fileWriter.WriteToFileAsync("products.json", productsJson);
        }
       
    }
}
