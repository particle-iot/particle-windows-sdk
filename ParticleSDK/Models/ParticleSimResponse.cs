using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Particle.SDK.Models
{
    /// <summary>
    /// Helper class from Particle Cloud Device Vitals request
    /// </summary>
    public class ParticleSimResponse
    {
        [JsonProperty("id")]
        public string Id { get; internal set; }
        [JsonProperty("activations_count")]
        public int ActivationsCount { get; internal set; }
        [JsonProperty("base_country_code")]
        public string BaseCountryCode { get; internal set; }
        [JsonProperty("base_monthly_rate")]
        public double BaseMonthlyRate { get; internal set; }
        [JsonProperty("deactivations_count")]
        public int DeactivationCount { get; internal set; }
        [JsonProperty("first_activated_on")]
        public string FirstActivated { get; internal set; }
        [JsonProperty("last_activated_on")]
        public string LastActivated { get; internal set; }
        [JsonProperty("last_activated_via")]
        public string LastActivatedVia { get; internal set; }
        [JsonProperty("last_status_change")]
        public string LastStatusChange { get; internal set; }
        [JsonProperty("last_status_change_action_error")]
        public string LastStatusChangeActionError { get; internal set; }
        [JsonProperty("msisdn")]
        public string MSISDN { get; internal set; }
        [JsonProperty("overage_monthly_rate")]
        public double OverageMonthlyRate { get; internal set; }
        [JsonProperty("status")]
        public string Status { get; internal set; }
        [JsonProperty("user_id")]
        public string UserId { get; internal set; }
        [JsonProperty("product_id")]
        public string ProductId { get; internal set; }
        [JsonProperty("carrier")]
        public string Carrier { get; internal set; }
        [JsonProperty("last_device_id")]
        public string LastDeviceId { get; internal set; }
        [JsonProperty("last_device_name")]
        public string LastDeviceName { get; internal set; }
        [JsonProperty("owner")]
        public string Owner { get; internal set; }
    }
}
