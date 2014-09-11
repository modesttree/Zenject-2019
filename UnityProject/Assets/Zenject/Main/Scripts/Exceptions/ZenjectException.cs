using System;

namespace ModestTree.Zenject.Api.Exceptions
{
    public class ZenjectException : Exception
    {
        public ZenjectException(string message)
            : base(message)
        {
        }

        public ZenjectException(
            string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class ZenjectResolveException : ZenjectException
    {
        public ZenjectResolveException(string message)
            : base(message)
        {
        }

        public ZenjectResolveException(
            string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class ZenjectBindException : ZenjectException
    {
        public ZenjectBindException(string message)
            : base(message)
        {
        }

        public ZenjectBindException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

