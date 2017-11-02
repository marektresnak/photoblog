using System;

namespace Photoblog.Exceptions {
    public class GeocodingException : Exception {

        public GeocodingException(Exception innerException)
            : base($"Geoocoding failed with the following exception: {innerException.Message}", innerException)  {
        }

    }
}
