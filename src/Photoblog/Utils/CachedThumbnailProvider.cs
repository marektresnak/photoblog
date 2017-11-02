using Microsoft.Extensions.Options;
using Photoblog.Data;
using Photoblog.Utils.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.IO;

namespace Photoblog.Utils {
    public class CachedThumbnailProvider : IThumbnailProvider {

        readonly IDataStore _dataStore;
        readonly Settings _settings;

        public CachedThumbnailProvider(IDataStore dataStore, IOptions<Settings> optionsAccessor) {
            _dataStore = dataStore;
            _settings = optionsAccessor.Value;
        }

        /// <inheritdoc />
        public Stream OpenThumbnailReadStream(string imageName, int width, int height) {
            var thumbnailFileName = $"{Path.GetFileNameWithoutExtension(imageName)}_{width}x{height}.{Path.GetExtension(imageName)}";
            var thumbnailFilePath = Path.Combine(_settings.ThumbnailDirectory, thumbnailFileName);

            if (!File.Exists(thumbnailFilePath)) {
                GenerateThumbnail(thumbnailFilePath, imageName, width, height);
            }

            return File.OpenRead(thumbnailFilePath);
        }

        void GenerateThumbnail(string thumbnailFilePath, string imageName, int width, int height) {
            using (var imageStream = _dataStore.GetEntryImageReadStream(imageName))
            using (var image = Image.Load(imageStream)) {
                // Resize image to fit the given bounding box and maintain original aspect ratio
                image.Mutate(i => i.Resize(new ResizeOptions {
                    // Result cannot be larger than the original image
                    Size = new Size(Math.Min(width, image.Width), Math.Min(height, image.Height)),
                    Mode = ResizeMode.Max
                }));

                // Fix orientation on thumbnails
                image.FixOrientation();

                // Create thumbnail directory if it doesn't exist
                if (!Directory.Exists(_settings.ThumbnailDirectory)) {
                    Directory.CreateDirectory(_settings.ThumbnailDirectory);
                }

                image.Save(thumbnailFilePath);
            }
        }

    }
}
