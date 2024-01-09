using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Particle.SDK.Models
{
    /// <summary>
    /// Helper class from Particle Cloud Device Information request
    /// </summary>
    public class ParticleDeviceResponse
    {
        [JsonProperty("id")]
        public string Id { get; internal set; }
        [JsonProperty("name")]
        public string Name { get; internal set; }
        [JsonProperty("last_app")]
        public string LastApp { get; internal set; }
        [JsonProperty("last_ip_address")]
        public string LastIPAddress { get; internal set; }
        [JsonProperty("last_heard")]
        public DateTime LastHeard { get; internal set; }
        [JsonProperty("requires_deep_update")]
        public bool RequiresDeepUpdate { get; internal set; }
        [JsonProperty("product_id")]
        public int ProductId { get; internal set; }
        [JsonProperty("online")]
        public bool Online { get; internal set; }
        [JsonProperty("platform_id")]
        public int PlatformId { get; internal set; }
        [JsonProperty("cellular")]
        public bool Cellular { get; internal set; }
        [JsonProperty("status")]
        public string Status { get; internal set; }
        [JsonProperty("last_iccid")]
        public string LastICCID { get; internal set; }
        [JsonProperty("imei")]
        public string IMEI { get; internal set; }
        [JsonProperty("current_build_target")]
        public string CurrentBuildTarget { get; internal set; }
        [JsonProperty("variables")]
        public Dictionary<string, string> Variables { get; internal set; }
        [JsonProperty("functions")]
        public string[] Functions { get; internal set; }
        [JsonProperty("cc3000_patch_version")]
        public string CC3000PatchVersion { get; internal set; }
    }
}
