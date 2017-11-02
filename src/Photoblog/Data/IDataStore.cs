using Photoblog.Entities;
using System.Collections.Generic;
using System.IO;

namespace Photoblog.Data {
    /// <summary>
    /// Data store is responsible for storing and accessing data (including images)
    /// </summary>
    public interface IDataStore {

        /// <summary>
        /// Returns all entries that pass the given filter
        /// </summary>
        ICollection<Entry> ListEntries(EntryFilter filter = null, int skip = 0, int? take = null);

        /// <summary>
        /// Adds a new entry
        /// </summary>
        Entry AddEntry(NewEntryDto newEntry);

        /// <summary>
        /// Returns a read-only stream for the original entry image
        /// </summary>
        Stream GetEntryImageReadStream(string imageName);

    }
}
