using Newtonsoft.Json;
using System;

namespace Particle.SDK.Models
{
    /// <summary>
    /// Helper class from Particle Cloud Ledger request scope data
    /// </summary>
    public class Scope
    {
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("value")]
        public string value { get; set; }
    }

    /// <summary>
    /// Helper class from Particle Cloud Ledger Instance
    /// </summary>
    public class ParticleLedgerInstance
    {
        [JsonProperty("scope")]
        public Scope Scope { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("size_bytes")]
        public int Size { get; set; }
        [JsonProperty("data")]
        public dynamic Data { get; set; }
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Helper class from Particle Cloud Ledger Instance response
    /// </summary>
    public class ParticleLedgerInstanceResponse
    {
        [JsonProperty("instance")]
        public ParticleLedgerInstance Instance { get; set; }
    }
}
