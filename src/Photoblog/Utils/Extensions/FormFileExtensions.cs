using Microsoft.AspNetCore.Http;
using System.IO;

namespace Photoblog.Utils.Extensions {
    public static class FormFileExtensions {

        public static byte[] GetBytes(this IFormFile formFile) {
            using (var ms = new MemoryStream()) {
                formFile.CopyTo(ms);
                return ms.ToArray();
            }
        }

    }
}
