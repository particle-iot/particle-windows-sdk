using Newtonsoft.Json;
using System;

namespace Particle.SDK.Models
{
    /// <summary>
    /// Helper class from Particle Cloud Variable request coreInfo data
    /// </summary>
    public class CoreInfo
    {
        [JsonProperty("last_app")]
        public string LastApp { get; set; }
        [JsonProperty("last_heard")]
        public DateTime LastHeard { get; set; }
        [JsonProperty("connected")]
        public bool Connected { get; set; }
        [JsonProperty("last_handshake_at")]
        public DateTime LastHandshakeAt { get; set; }
        [JsonProperty("deviceID")]
        public string DeviceID { get; set; }
        [JsonProperty("product_id")]
        public int ProductId { get; set; }
    }

    /// <summary>
    /// Helper class from Particle Cloud Variable request
    /// </summary>
    public class ParticleVariableResponse
    {
        [JsonProperty("cmd")]
        public string Command { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("result")]
        public dynamic Result { get; set; }
        [JsonProperty("coreInfo")]
        public CoreInfo CoreInfo { get; set; }
    }
}
