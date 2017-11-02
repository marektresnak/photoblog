using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Photoblog.Entities.ActionArguments {
    public class CreateEntryForm {

        public string Label { get; set; }

        public DateTime Date { get; set; }

        // Simple content-type validation is good enough for this type of app
        [ContentTypeValidation("image/jpeg", "image/png")]
        public List<IFormFile> Photos { get; set; }

    }
}
