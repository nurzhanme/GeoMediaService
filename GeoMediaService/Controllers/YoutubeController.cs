using GeoMediaService.DTOs;
using GeoMediaService.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeoMediaService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YoutubeController : ControllerBase
    {
        private readonly ILogger<YoutubeController> _logger;
        private readonly YoutubeService _youtubeService;

        public YoutubeController(ILogger<YoutubeController> logger, YoutubeService youtubeService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _youtubeService = youtubeService ?? throw new ArgumentNullException(nameof(youtubeService));
        }

        [HttpGet]
        [Route("GetChannelIdBy")]
        public async Task<IActionResult> GetChannelIdBy(string username)
        {
            await _youtubeService.Auth();
            return Ok(await _youtubeService.GetChannelIdBy(username));
        }

        [HttpGet]
        [Route("GetChannelVideo")]
        public async Task<IActionResult> GetChannelVideo(string channelId, DateTime publishedAfter = default, string? pageToken = default)
        {
            await _youtubeService.Auth();
            return Ok(await _youtubeService.GetChannelVideo(channelId, publishedAfter, pageToken));
        }
    }
}