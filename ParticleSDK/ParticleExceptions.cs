using System;

namespace Particle.SDK
{
    /// <summary>
    /// Simple Exception class for particle cloud returning 404 Not Found
    /// </summary>
    public class ParticleNotFoundException : Exception
    {
        public ParticleNotFoundException()
        {
        }

        public ParticleNotFoundException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Simple Exception class for particle cloud returning 400 Bad Request
    /// </summary>
    public class ParticleRequestBadRequestException : Exception
    {
        public ParticleRequestBadRequestException()
        {
        }

        public ParticleRequestBadRequestException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Simple Exception class for particle cloud returning 401 Unauthorized
    /// </summary>
    public class ParticleUnauthorizedException : Exception
    {
        public ParticleUnauthorizedException()
        {
        }

        public ParticleUnauthorizedException(string message)
            : base(message)
        {
        }
    }
}
