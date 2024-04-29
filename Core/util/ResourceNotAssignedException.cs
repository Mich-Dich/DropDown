using System;

namespace Core {

    public class ResourceNotAssignedException : Exception {
        public ResourceNotAssignedException(string message)
            : base(message) {
        }
    }
}