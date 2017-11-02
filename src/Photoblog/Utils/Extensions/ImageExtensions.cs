using SixLabors.ImageSharp;
using SixLabors.ImageSharp.MetaData.Profiles.Exif;
using System.Collections.Generic;

namespace Photoblog.Utils.Extensions {
    public static class ImageExtensions {

        /// <summary>
        /// Rotates an image to the 'upright' position using orientation value specified in EXIF
        /// </summary>
        public static void FixOrientation(this Image<Rgba32> image) {
            var rotationTable = new Dictionary<ushort, int> {
                [6] = 90, [3] = 180, [8] = 270
            };

            var exif = image.MetaData.ExifProfile;
            var orientation = exif?.GetValue(ExifTag.Orientation)?.Value as ushort?;

            if (orientation != null &&
                rotationTable.TryGetValue(orientation.Value, out int rotationDegrees)) {

                image.Mutate(i => i.Rotate(rotationDegrees));

                // Make sure orientation value in EXIF matches the current image orientation
                exif.SetValue(ExifTag.Orientation, (ushort)0);
            }
        }

    }
}
