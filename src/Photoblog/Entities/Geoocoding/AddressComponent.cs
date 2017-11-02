using Newtonsoft.Json;
using System.Collections.Generic;

namespace Photoblog.Entities.Geocoding {
    public class AddressComponent {

        [JsonProperty("long_name")]
        public string LongName { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        public List<string> Types { get; set; }

    }
}
