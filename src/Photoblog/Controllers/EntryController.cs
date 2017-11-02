using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Photoblog.Data;
using Photoblog.Entities;
using Photoblog.Entities.ActionArguments;
using Photoblog.Utils;
using Photoblog.Utils.Extensions;
using System;
using System.IO;
using System.Linq;

namespace Photoblog.Controllers {
    public class EntryController : Controller {

        readonly IDataStore _dataStore;
        readonly IThumbnailProvider _thumbnailProvider;

        public EntryController(IDataStore dataStore, IThumbnailProvider thumbnailProvider) {
            _dataStore = dataStore;
            _thumbnailProvider = thumbnailProvider;
        }

        [HttpGet("api/entries")]
        public IActionResult List(int skip = 0, int? take = null) {
            return new ObjectResult(new {
                Entries = _dataStore.ListEntries(skip: skip, take: take)
            });
        }

        [HttpGet("api/image")]
        [ResponseCache(Duration = 60 * 60 * 24 * 30)]
        public IActionResult GetImageThumbnail(string imageName, int width = Int32.MaxValue, int height = Int32.MaxValue) {
            var contentTypeProvider = new FileExtensionContentTypeProvider();
            if (!contentTypeProvider.TryGetContentType(imageName, out string contentType)) {
                return BadRequest();
            }

            // FileStreamResult will dispose of a given stream
            return new FileStreamResult(
                _thumbnailProvider.OpenThumbnailReadStream(imageName, width, height), contentType);
        }

        [HttpPost("api/entries")]
        public IActionResult Create([FromForm] CreateEntryForm entryForm) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var newEntry = new NewEntryDto {
                Label = entryForm.Label,
                Date = entryForm.Date,
                Tags = TagHelper.ExtractFromText(entryForm.Label),
                Images = entryForm.Photos
                    .Select(p =>
                        new NewEntryImageDto(p.GetBytes(), Path.GetExtension(p.FileName)))
                    .ToList()
            };

            // Create a new entry and return it to the caller
            return new ObjectResult(
                _dataStore.AddEntry(newEntry));
        }

    }
}
