using System;

namespace Photoblog.Exceptions {
    public class NotFoundException : Exception {

        public string TargetType { get; }

        public string TargetIdentifier { get; }

        public NotFoundException(string targetType, string targetIdentifier, Exception innerException = null) 
            : base($"{targetType} [{targetIdentifier}] could not be found.", innerException) {

            TargetType = targetType;
            TargetIdentifier = targetIdentifier;
        }

    }
}
