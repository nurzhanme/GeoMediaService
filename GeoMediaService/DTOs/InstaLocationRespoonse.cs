namespace GeoMediaService.DTOs
{
    public class InstaLocationRespoonse
    {
        public string ExternalId { get; set; }
        public string ExternalSource { get; set; }
        public string Title { get; set; }
        public IEnumerable<string> PostUrls { get; set; }
    }
}
