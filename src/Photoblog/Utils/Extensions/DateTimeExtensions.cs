using System;

namespace Photoblog.Utils.Extensions {
    public static class DateTimeExtensions {

        public static long ToUnixTimestamp(this DateTime dateTime) {
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }

    }
}
