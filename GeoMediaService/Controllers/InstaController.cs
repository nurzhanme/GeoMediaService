using GeoMediaService.DTOs;
using GeoMediaService.Enum;
using GeoMediaService.Options;
using GeoMediaService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GeoMediaService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstaController : ControllerBase
    {
        private readonly ILogger<InstaController> _logger;
        private readonly InstaOptions _options;
        private readonly InstaService _instaService;

        public InstaController(ILogger<InstaController> logger, IOptions<InstaOptions> options, InstaService instaService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _instaService = instaService ?? throw new ArgumentNullException(nameof(instaService));
        }

        [HttpGet]
        [Route("SearchLocation")]
        public async Task<IActionResult> SearchLocation(string place)
        {
            await _instaService.Login(_options.Username!, _options.Password!);
            return Ok(await _instaService.SearchLocation(place));
        }

        [HttpGet]
        public async Task<IActionResult> Media(long locationId, InstaLocationTopOrRecent topOrRecent = InstaLocationTopOrRecent.Top)
        {
            await _instaService.Login(_options.Username!, _options.Password!);
            return Ok(await _instaService.GetLocationFeeds(locationId, topOrRecent));
        }
    }
}