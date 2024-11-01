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
        [JsonProperty("last_handshake")]
        public DateTime LastHandshake { get; internal set; }
        [JsonProperty("product_id")]
        public int ProductId { get; internal set; }
        [JsonProperty("online")]
        public bool Online { get; internal set; }
        [JsonProperty("platform_id")]
        public int PlatformId { get; internal set; }
        [JsonProperty("cellular")]
        public bool Cellular { get; internal set; }
        [JsonProperty("notes")]
        public string Notes { get; internal set; }
        [JsonProperty("firmware_updates_enabled")]
        public bool FirmwareUpdatesEnabled { get; internal set; }
        [JsonProperty("firmware_updates_forced")]
        public bool FirmwareUpdatesForced { get; internal set; }
        [JsonProperty("status")]
        public string Status { get; internal set; }
        [JsonProperty("serial_number")]
        public string SerialNumber { get; internal set; }
        [JsonProperty("iccid")]
        public string ICCID { get; internal set; }
        [JsonProperty("imei")]
        public string IMEI { get; internal set; }
        [JsonProperty("mac_wifi")]
        public string WiFiMAC { get; internal set; }
        [JsonProperty("mobile_secret")]
        public string MobileSecret { get; internal set; }
        [JsonProperty("system_firmware_version")]
        public string SystemFirmwareVersion { get; internal set; }
        [JsonProperty("firmware_product_id")]
        public int FirmwareProductId { get; internal set; }
        [JsonProperty("firmware_version")]
        public int FirmwareVersion { get; internal set; }
        [JsonProperty("development")]
        public bool Development { get; internal set; }
        [JsonProperty("quarantined")]
        public bool Quarantined { get; internal set; }
        [JsonProperty("denied")]
        public bool Denied { get; internal set; }
        [JsonProperty("variables")]
        public Dictionary<string, string> Variables { get; internal set; }
        [JsonProperty("functions")]
        public string[] Functions { get; internal set; }
    }
}
