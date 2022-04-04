using GeoMediaService.DTOs;
using GeoMediaService.Enum;
using GeoMediaService.Exceptions;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;

namespace GeoMediaService.Services
{
    public class InstaService
    {
        private IInstaApi? _instaApi;

        public async Task<(bool, string)> Login(string username, string password)
        {

            _instaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(UserSessionData.ForUsername(username).WithPassword(password))
                .SetRequestDelay(RequestDelay.FromSeconds(2, 2))
                .Build();

            await _instaApi.SendRequestsBeforeLoginAsync();
            var result = await _instaApi.LoginAsync();

            if (!result.Succeeded || !_instaApi.IsUserAuthenticated)
            {
                throw new InstaException(result.Info.Message);
            }

            await _instaApi.SendRequestsAfterLoginAsync();

            var sessionData = _instaApi.GetStateDataAsString();
            return (true, sessionData);
        }

        public async Task Login(string sessionData)
        {

            _instaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(UserSessionData.Empty)
                .SetRequestDelay(RequestDelay.FromSeconds(2, 2))
                .Build();

            await _instaApi.LoadStateDataFromStringAsync(sessionData);

            if (!_instaApi.IsUserAuthenticated) throw new InstaException("Not Authorized");
        }

        public async Task<IEnumerable<InstaLocationResponse>> SearchLocation(string place)
        {
            var searchLocation = await _instaApi!.LocationProcessor.SearchLocationAsync(0, 0, place);
            if (!searchLocation.Succeeded)
            {
                throw new InstaException(searchLocation.Info.Message);
            }

            return searchLocation.Value.Select(x => new InstaLocationResponse
            {
                ExternalId = x.ExternalId,
                ExternalSource = x.ExternalSource,
                Title = x.Name
            });
        }

        public async Task<IEnumerable<string>> GetLocationFeeds(long locationId, InstaLocationTopOrRecent topOrRecent = InstaLocationTopOrRecent.Top)
        {
            IResult<InstaSectionMedia> locationFeed;
                
            if (topOrRecent == InstaLocationTopOrRecent.Top) 
            { 
                locationFeed = await _instaApi!.LocationProcessor.GetTopLocationFeedsAsync(locationId, PaginationParameters.MaxPagesToLoad(1));
            }
            else
            {
                locationFeed = await _instaApi!.LocationProcessor.GetRecentLocationFeedsAsync(locationId, PaginationParameters.MaxPagesToLoad(1));
            }

            if (!locationFeed.Succeeded)
            {
                throw new InstaException(locationFeed.Info.Message);
            }

            return locationFeed.Value.Medias.Select(x => $"https://www.instagram.com/p/{x.Code}/");
        }

    }
}
