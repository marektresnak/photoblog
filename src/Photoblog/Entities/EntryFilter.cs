using System.Collections.Generic;

namespace Photoblog.Entities {
    public class EntryFilter {

        public List<string> Tags { get; set; } = new List<string>();

        public int? Year { get; set; }

    }
}
