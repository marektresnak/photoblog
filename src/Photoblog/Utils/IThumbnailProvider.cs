using System.IO;

namespace Photoblog.Utils {
    public interface IThumbnailProvider {

        /// <summary>
        /// Returns an image thumbnail with the given dimension that is not larger than the original image
        /// </summary>
        Stream OpenThumbnailReadStream(string imageName, int width, int height);

    }
}