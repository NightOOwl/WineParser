namespace WineParser
{
    public class Product
    {
        public Product(string name, float price, float? oldPrice,
            float rating, string volume, int articul,
            string region, string url, List<string> pictures)
        {
            Name = name;
            Price = price;
            OldPrice = oldPrice;
            Rating = rating;
            Volume = volume;
            Articul = articul;
            Region = region;
            Url = url;
            PictureUrls = pictures;
        }
        public string Name { get; set; }
        public float Price { get; set; }
        public float? OldPrice { get; set; }
        public float Rating { get; set; }
        public string Volume { get; set; }
        public int Articul { get; set; }
        public string Region { get; set; }
        public string Url { get; set; }
        public List<string> PictureUrls { get; set; }
    }
}
