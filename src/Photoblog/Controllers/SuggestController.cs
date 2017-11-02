using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Photoblog.Data;
using Photoblog.Entities.ActionArguments;
using Photoblog.Entities.Geocoding;
using Photoblog.Entities.Geoocoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photoblog.Controllers {
    public class SuggestController : Controller {

        readonly Settings _settings;

        public SuggestController(IOptions<Settings> optionsAccessor) {
            _settings = optionsAccessor.Value;
        }

        [HttpGet("api/suggestTags")]
        public async Task<IActionResult> SuggestTags([FromQuery] SuggestParameters suggestParameters) {
            var suggestedTags = new List<string>();

            if (suggestParameters.ContainsLocation) {
                suggestedTags.AddRange(
                    await GetLocationTagsAsync(suggestParameters));
            }

            return new ObjectResult(new {
                Tags = suggestedTags
            });
        }

        async Task<IEnumerable<string>> GetLocationTagsAsync(SuggestParameters suggestParameters) {
            var geocodingClient = new GeocodingClient(_settings.GoogleApiKey);
            var geocodingResult = await geocodingClient.GeocodeAsync(new GeocodingRequest {
                Latitude = suggestParameters.Lat.Value,
                Longitude = suggestParameters.Lon.Value
            });

            // Out of all geoocoding results, get the first 'Country' address component and take its short name
            var countryCode = geocodingResult.SelectMany(r => r.AddressComponents)
                .FirstOrDefault(ac => ac.Types.Contains(AddressComponentType.Country))
                    ?.ShortName?.ToLower();

            if (!String.IsNullOrEmpty(countryCode) &&
                !_settings.IgnoredCountries.Contains(countryCode)) {

                return new[] { countryCode };
            }

            return Enumerable.Empty<string>();
        }

    }
}
