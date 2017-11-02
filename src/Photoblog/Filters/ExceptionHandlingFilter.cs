using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Photoblog.Exceptions;

namespace Photoblog.Filters {
    public class ExceptionHandlingFilter : ExceptionFilterAttribute {

        readonly ILogger<ExceptionHandlingFilter> _logger;

        public ExceptionHandlingFilter(ILogger<ExceptionHandlingFilter> logger) {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context) {
            int statusCode = 500;

            // Don't log NotFoundException
            if (context.Exception is NotFoundException) {
                statusCode = 404;
            } else {
                _logger.LogError(context.Exception, context.Exception.Message);
            }

            var errorObject = new {
                ErrorMessage = context.Exception.Message
            };
            context.Result = new ObjectResult(errorObject) {
                StatusCode = statusCode
            };
            context.ExceptionHandled = true;
        }


    }
}
