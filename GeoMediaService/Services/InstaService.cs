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

        public async Task<List<InstaLocationRespoonse>> GetLocationMedia(string place, InstaLocationTopOrRecent topOrRecent)
        {
            var searchLocation = await _instaApi.LocationProcessor.SearchLocationAsync(0, 0, place);
            if (!searchLocation.Succeeded)
            {
                throw new InstaException(searchLocation.Info.Message);
            }

            var result = new List<InstaLocationRespoonse>();

            foreach (var instaLocationShort in searchLocation.Value)
            {
                var instaLocationResponse = new InstaLocationRespoonse();
                instaLocationResponse.ExternalId = instaLocationShort.ExternalId;
                instaLocationResponse.ExternalSource = instaLocationShort.ExternalSource;
                instaLocationResponse.Title = instaLocationShort.Name;
                
                IResult<InstaSectionMedia> locationFeed;
                if (topOrRecent == InstaLocationTopOrRecent.Top)
                {
                    locationFeed = await _instaApi.LocationProcessor.GetTopLocationFeedsAsync(long.Parse(instaLocationShort.ExternalId), PaginationParameters.MaxPagesToLoad(1));
                }
                else
                {
                    locationFeed = await _instaApi.LocationProcessor.GetRecentLocationFeedsAsync(long.Parse(instaLocationShort.ExternalId), PaginationParameters.MaxPagesToLoad(1));
                }


                if (!locationFeed.Succeeded)
                {
                    continue;
                }

                instaLocationResponse.PostUrls = locationFeed.Value.Medias.Select(x => $"https://www.instagram.com/p/{x.Code}/");
                result.Add(instaLocationResponse);
            }

            return result;
        }
    }
}
