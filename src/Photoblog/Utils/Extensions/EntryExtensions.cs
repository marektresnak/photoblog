using Photoblog.Entities;
using System.Linq;

namespace Photoblog.Utils.Extensions {
    public static class EntryExtensions {

        public static bool MatchesFilter(this Entry entry, EntryFilter filter) {
            // Ignore all filter properties that don't have a value

            if (filter.Year != null && entry.Date.Year != filter.Year) {
                return false;
            }

            if (filter.Tags.Count > 0 && !entry.Tags.Any(t => filter.Tags.Contains(t))) {
                return false;
            }

            return true;
        }

    }
}
