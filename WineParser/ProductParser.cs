using AngleSharp.Dom;
using System.Text.RegularExpressions;

namespace WineParser
{
    public class ProductParser
    {
        public async Task<Product> ParseProductAsync(IDocument document, string productUrl)
        {
            // Получаем название товара
            string name = document.QuerySelector("h1").TextContent.Trim();
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            // Получаем актуальную стоимость товара
            var priceString = document.QuerySelector("div.product-buy__price").TextContent.Replace(" ", "").Replace(".", ",");
            var price = float.Parse(Regex.Match(priceString, @"\d+").Value);

            // Получаем старую стоимость товара
            var oldPriceString = document.QuerySelector("div.product-buy__old-price.product-buy__with-one")?.TextContent.Replace(" ", "").Replace(".", ",");
            var oldPrice = oldPriceString != null
                ? float.Parse(Regex.Match(oldPriceString, @"\d+").Value)
                : (float?)null;

            // Получаем рейтинг товара
            var rating = float.Parse(document.QuerySelector("p.rating-stars__value").TextContent.Replace(".", ",").Trim());

            // Получаем объем товара
            var volumeRegexPattern = new Regex(".*Объем:.*");
            var volume = document.QuerySelectorAll("dt")
                .First(dt => volumeRegexPattern.IsMatch(dt.TextContent)).NextElementSibling.TextContent.Trim();
            ArgumentException.ThrowIfNullOrWhiteSpace(volume);

            // Получаем артикул товара
            var articulString = document.QuerySelector("span.product-page__article.js-copy-article").TextContent;
            var articul = int.Parse(Regex.Match(articulString, @"\d+").Value);

            // Получаем регион товара 
            var region = document.QuerySelector("button.location__current.dropdown__toggler").TextContent.Trim();
            ArgumentException.ThrowIfNullOrWhiteSpace(region);

            // Получаем ссылки на картинки 
            var pictures = document.QuerySelectorAll("div.product-slider__item.swiper-slide")
                .Select(picDiv => picDiv.QuerySelector("source").GetAttribute("data-srcset").Split(" ")[2])
                .ToList();
            

            var product = new Product(name, price, oldPrice, rating, volume, articul, region, productUrl, pictures);           
            return product;
        }
    }
}
