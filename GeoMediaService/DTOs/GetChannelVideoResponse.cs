namespace GeoMediaService.DTOs
{
    public class GetChannelVideoResponse
    {
        public IEnumerable<YoutubeVideoDto> Videos { get; set; }
        public string PrevPageToken { get; set; }
        public string NextPageToken { get; set; }

    }
}
