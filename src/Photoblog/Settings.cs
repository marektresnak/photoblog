using System.Collections.Generic;
using System.IO;

namespace Photoblog {
    public class Settings {

        public string AppSecret { get; set; }

        public bool EnableEntryCache { get; set; }

        public List<string> IgnoredCountries { get; set; } = new List<string>();

        public string GoogleApiKey { get; set; }

        public List<AppUser> Users { get; set; } = new List<AppUser>();

        public string WorkingDirectory { get; set; }

        public string ImageDirectory => Path.Combine(WorkingDirectory, "images");

        public string ThumbnailDirectory => Path.Combine(WorkingDirectory, "tmp", "thumbnails");

        public string DataFilePath => Path.Combine(WorkingDirectory, "entries.data");

        public string LoggingDirectory => Path.Combine(WorkingDirectory, "logs");

    }

    public class AppUser {

        public string UserName { get; set; }

        public string Password { get; set; }

    }
}
