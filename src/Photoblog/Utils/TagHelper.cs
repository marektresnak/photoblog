using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Photoblog.Utils {
    public static class TagHelper {

        public static List<string> ExtractFromText(string text) {
            if (String.IsNullOrEmpty(text)) {
                return new List<string>();
            }

            var matches = Regex.Matches(text, "#([\\p{L}]+)");
            return matches
                .Select(m => m.Value.TrimStart('#'))
                .ToList();
        }

    }
}
