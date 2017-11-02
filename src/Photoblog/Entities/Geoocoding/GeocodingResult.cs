using Newtonsoft.Json;
using System.Collections.Generic;

namespace Photoblog.Entities.Geocoding {
    public class GeocodingResult {

        [JsonProperty("address_components")]
        public List<AddressComponent> AddressComponents { get; set; }

    }
}
