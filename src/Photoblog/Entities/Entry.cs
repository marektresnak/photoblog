using System;
using System.Collections.Generic;

namespace Photoblog.Entities {
    public class Entry {

        public int Id { get; set; }

        public string Label { get; set; }

        public DateTime Date { get; set; }

        public List<string> Images { get; set; }

        public List<string> Tags { get; set; }

    }
}
