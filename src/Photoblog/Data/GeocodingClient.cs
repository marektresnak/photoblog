using Newtonsoft.Json;
using Photoblog.Entities.Geocoding;
using Photoblog.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace Photoblog.Data {
    /// <summary>
    /// Simple client for Google Geoocoding API
    /// </summary>
    public class GeocodingClient {

        const string RequestUrlBase = "https://maps.googleapis.com/maps/api/geocode/json";

        readonly string _apiKey;

        public GeocodingClient(string googleApiKey) {
            _apiKey = googleApiKey;
        }

        public async Task<List<GeocodingResult>> GeocodeAsync(GeocodingRequest geocodingRequest) {
            var requestUrl = String.Format(CultureInfo.InvariantCulture,
                $"{RequestUrlBase}?latlng={{0}},{{1}}&key={{2}}", geocodingRequest.Latitude, geocodingRequest.Longitude, _apiKey);

            try {
                using (var client = new HttpClient()) {
                    var response = await client.GetStringAsync(requestUrl);
                    return JsonConvert.DeserializeObject<GeocodingResponse>(response).Results;
                }
            } catch (Exception e) {
                // Wrap this inside another exception so it's easier to identify
                throw new GeocodingException(e);
            }
        }

        class GeocodingResponse {

            public List<GeocodingResult> Results { get; set; }

        }

    }
}
