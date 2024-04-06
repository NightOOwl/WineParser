using System.Collections.Concurrent;
using WineParser.Exstantions;

namespace WineParser
{
    public class MainPageParser
    {
        private readonly HtmlLoader htmlLoader;
        private string domainAdress;
        private string relativeAdress;
        private string postfix;
        private int city;

        public MainPageParser(HtmlLoader htmlLoader, string domainAdress, string relativeAdress, int city, string postfix = "")
        {
            this.htmlLoader = htmlLoader;
            this.domainAdress = domainAdress;
            this.relativeAdress = relativeAdress;
            this.postfix = postfix;
            this.city = city;
        }


        public async Task<ConcurrentBag<string>> ParseProductsLinksAsync()
        {
            string url = domainAdress + "/" + relativeAdress + "/" + postfix;
            var links = new ConcurrentBag<string>();

            string urlWithCityParameter = url + $"?setVisitorCityId={city}";
            int pageCount = await GetPageCountAsync(urlWithCityParameter);

            var pages = Enumerable.Range(1, pageCount);

            var options = new ParallelOptions() { MaxDegreeOfParallelism = 3 };

            var processedPageCount = 0;

            await Parallel.ForEachAsync(pages, options, async (page, cancellationToken) =>
            {
                postfix = $"page{page}/";
                string currentUrl = domainAdress + "/" + relativeAdress + "/" + postfix;
                var document = await htmlLoader.LoadHtml(currentUrl);
                var wineCards = document.QuerySelectorAll("div.snippet-middle");

                foreach (var card in wineCards)
                {
                    var link = domainAdress + card.QuerySelector("a.snippet-name.js-dy-slot-click").GetAttribute("href");
                    links.Add(link);
                    Console.WriteLine(link);
                }

                Interlocked.Increment(ref processedPageCount);   
                Console.WriteLine($"Страниц каталога обработано {processedPageCount}/{pageCount}");
            });

            return links;
        }

        private async Task<int> GetPageCountAsync(string url)
        {   
            var document = await htmlLoader.LoadHtml(url);        
         
            var navigationBar = document.QuerySelectorSafe("div.pagination");

            var pagination = navigationBar.QuerySelector(".pagination__next")
                .PreviousElementSibling
                .TextContent.Trim();

            return int.Parse(pagination);
        }
    }
}
