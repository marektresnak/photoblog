using System;
using System.Collections.Generic;

namespace Photoblog.Entities {
    public class NewEntryDto {

        public string Label { get; set; }

        public DateTime Date { get; set; }

        public List<NewEntryImageDto> Images { get; set; }

        public List<string> Tags { get; set; }

    }
}
