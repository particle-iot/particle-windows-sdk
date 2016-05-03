using Newtonsoft.Json;

namespace Particle.SDK.Models
{
    /// <summary>
    /// Helper class from general Particle Cloud request
    /// </summary>
    internal class ParticleGenericResponse
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
