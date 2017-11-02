using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Photoblog.Entities;
using Photoblog.Exceptions;
using Photoblog.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Photoblog.Data {
    public class FileDataStore : IDataStore {

        const string EntryCacheKey = nameof(FileDataStore) + "_" + nameof(GetAllEntries) + "_Result";

        readonly object SaveEntryLock = new object();

        readonly IMemoryCache _cache;
        readonly Settings _settings;

        public FileDataStore(IMemoryCache cache, IOptions<Settings> optionsAccessor) {
            _cache = cache;
            _settings = optionsAccessor.Value;
        }

        /// <inheritdoc />
        public ICollection<Entry> ListEntries(EntryFilter filter = null, int skip = 0, int? take = null) {
            IEnumerable<Entry> allEntries = GetAllEntries();

            if (filter != null) {
                allEntries = allEntries
                    .Where(e => e.MatchesFilter(filter));
            }

            allEntries = allEntries.Skip(skip);

            if (take != null) {
                allEntries = allEntries.Take(take.Value);
            }

            return allEntries.ToList();
        }

        /// <inheritdoc />
        public Stream GetEntryImageReadStream(string imageName) {
            var imagePath = GetEntryImagePath(imageName);
            if (!File.Exists(imagePath)) {
                throw new NotFoundException("EntryImage", imageName);
            }

            return File.OpenRead(imagePath);
        }

        /// <inheritdoc />
        public Entry AddEntry(NewEntryDto newEntry) {
            // Make sure no other thread gets the same ID for a new entry
            lock (SaveEntryLock) {
                VerifyDataFileExists();

                var entryId = GetNewEntryId();
                var entry = new Entry {
                    Id = entryId,
                    Label = newEntry.Label,
                    Date = newEntry.Date,
                    Tags = newEntry.Tags,
                    Images = SaveEntryImages(entryId, newEntry.Images)
                };

                var jsonEntry = JsonConvert.SerializeObject(entry,
                        new JsonSerializerSettings {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });

                // Append a new line
                File.AppendAllText(_settings.DataFilePath, $"{Environment.NewLine}{jsonEntry}");

                _cache.Remove(EntryCacheKey);

                return entry;
            }
        }

        List<string> SaveEntryImages(int entryId, List<NewEntryImageDto> images) {
            // Make sure the directory for storing images exists
            if (!Directory.Exists(_settings.ImageDirectory)) {
                Directory.CreateDirectory(_settings.ImageDirectory);
            }

            var imageNames = new List<string>();

            for (var i = 0; i < images.Count; i++) {
                var image = images[i];
                var imageName = $"{entryId}-{i + 1}{image.Extension}";
                var imagePath = GetEntryImagePath(imageName);

                File.WriteAllBytes(imagePath, image.Data);

                imageNames.Add(imageName);
            }

            return imageNames;
        }

        int GetNewEntryId() {
            var allEntries = GetAllEntries();

            // Max() doesn't work on empty collections
            return allEntries.Count > 0
                ? allEntries.Max(e => e.Id) + 1
                : 1;
        }

        string GetEntryImagePath(string imageName) {
            return Path.Combine(_settings.ImageDirectory, imageName);
        }

        List<Entry> GetAllEntries() {
            // Serve from cache if its enabled
            return _settings.EnableEntryCache
                ? _cache.GetOrCreate(EntryCacheKey, ci => LoadAllEntries())
                : LoadAllEntries();
        }

        List<Entry> LoadAllEntries() {
            // Make sure the file exists before reading data from it
            VerifyDataFileExists();

            // Each line of the data file contains a JSON-serialized entry
            return File.ReadAllLines(_settings.DataFilePath)
                .Where(l => !String.IsNullOrEmpty(l))
                .Select(JsonConvert.DeserializeObject<Entry>)
                .ToList();
        }

        void VerifyDataFileExists() {
            if (!File.Exists(_settings.DataFilePath)) {
                var dataFileDir = Path.GetDirectoryName(_settings.DataFilePath);

                // Make sure the entire path exists
                Directory.CreateDirectory(dataFileDir);
                File.Create(_settings.DataFilePath).Dispose();
            }
        }

    }
}
