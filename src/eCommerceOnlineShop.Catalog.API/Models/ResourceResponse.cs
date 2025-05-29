namespace eCommerceOnlineShop.Catalog.API.Models
{
    public class Link
    {
        public string Href { get; set; } = string.Empty;
        public string Rel { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
    }

    public class ResourceResponse<T>
    {
        public T Data { get; set; } = default!;
        public List<Link> Links { get; set; } = [];
    }
}