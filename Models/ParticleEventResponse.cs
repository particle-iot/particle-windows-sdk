using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Particle.SDK.Models
{
    /// <summary>
    /// Helper class from Particle Cloud Event Stream request
    /// </summary>
    public class ParticleEventResponse
    {
        public string Name { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
        [JsonProperty("ttl")]
        public int TTL { get; set; }
        [JsonProperty("published_at")]
        public DateTime PublishedAt { get; set; }
        [JsonProperty("coreid")]
        public string DeviceId { get; set; }
    }

    /// <summary>
    /// Collection class from Particle Cloud Event Stream request
    /// </summary>
    public class ParticleEventResponseCollection : List<ParticleEventResponse>
    {
        public ParticleEventResponseCollection()
        {
        }
    }
}
