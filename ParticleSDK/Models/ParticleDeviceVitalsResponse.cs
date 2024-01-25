using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Particle.SDK.Models
{
    public class ParticleDeviceVitalsIdentity
    {
        public string mobile_country_code { get;  set; }
        public string mobile_network_code { get;  set; }
        public string location_area_code { get;  set; }
        public string cell_id { get;  set; }
    }

    public class ParticleDeviceVitalsCellular
    {
        public string radio_access_technology { get;  set; }
        [JsonProperty("operator")]
        public string operator_name { get;  set; }
        public ParticleDeviceVitalsIdentity cell_global_identity { get;  set; }
    }

    public class ParticleDeviceVitalsSignal
    {
        public string at { get;  set; }
        public float strength { get;  set; }
        public string strength_units { get;  set; }
        public int strengthv { get;  set; }
        public string strengthv_units { get;  set; }
        public string strengthv_type { get;  set; }
        public float quality { get;  set; }
        public string quality_units { get;  set; }
        public int qualityv { get;  set; }
        public string qualityv_units { get;  set; }
        public string qualityv_type { get;  set; }
    }
    public class ParticleDeviceVitalsConnection
    {
        public string status { get;  set; }
        public string error { get;  set; }
        public string disconnects { get;  set; }
        public string attempts { get;  set; }
        public string disconnect_reason { get;  set; }
    }

    public class ParticleDeviceVitalsNetwork
    {
        public ParticleDeviceVitalsCellular cellular { get;  set; }
        public ParticleDeviceVitalsSignal signal { get;  set; }
        public ParticleDeviceVitalsConnection connection { get;  set; }
    }

    public class ParticleDeviceVitalsCoap
    {
        public int transmit { get;  set; }
        public int retransmit { get;  set; }
        public int unack { get;  set; }
        public int round_trip { get;  set; }
    }
    public class ParticleDeviceVitalsPublish
    {
        public int rate_limited { get;  set; }
    }

    public class ParticleDeviceVitalsCloud
    {
        public ParticleDeviceVitalsConnection connection { get;  set; }
        public ParticleDeviceVitalsCoap coap { get;  set; }
        public ParticleDeviceVitalsPublish publish { get;  set; }
    }

    public class ParticleDeviceVitalsBattery
    {
        public object charge { get;  set; }
        public string state { get;  set; }
    }

    public class ParticleDeviceVitalsPower
    {
        public ParticleDeviceVitalsBattery battery { get;  set; }
        public string source { get;  set; }
    }

    public class ParticleDeviceVitalsMemory
    {
        public string used { get;  set; }
        public string total { get;  set; }
    }

    public class ParticleDeviceVitalsSystem
    {
        public string uptime { get;  set; }
        public ParticleDeviceVitalsMemory memory { get;  set; }
    }

    public class ParticleDeviceVitalsDevice
    {
        public ParticleDeviceVitalsNetwork network { get;  set; }
        public ParticleDeviceVitalsCloud cloud { get;  set; }
        public ParticleDeviceVitalsPower power { get;  set; }
        public ParticleDeviceVitalsSystem system { get;  set; }
    }

    public class ParticleDeviceVitalsServiceDevice
    {
        public string status { get;  set; }
    }

    public class ParticleDeviceVitalsServicePublish
    {
        public string sent { get;  set; }
    }

    public class ParticleDeviceVitalsServiceCloud
    {
        public string uptime { get;  set; }
        public ParticleDeviceVitalsServicePublish publish { get;  set; }
    }

    public class ParticleDeviceVitalsServiceCoap
    {
        public string round_trip { get;  set; }
    }

    public class ParticleDeviceVitalsService
    {
        public ParticleDeviceVitalsServiceDevice device { get; set; }
        public ParticleDeviceVitalsServiceCloud cloud { get; set; }
        public ParticleDeviceVitalsServiceCoap coap { get; set; }
    }

    public class ParticleDeviceVitalsPayload
    {
        public ParticleDeviceVitalsDevice device { get; set; }
        public ParticleDeviceVitalsService service { get; set; }
    }

    public class ParticleDeviceVitalsDiagnostics
    {
        public string deviceID { get; set; }
        public string updated_at { get; set; }
        public ParticleDeviceVitalsPayload payload { get; set; }
    }

    /// <summary>
    /// Helper class from Particle Cloud Device Vitals request
    /// </summary>
    public class ParticleDeviceVitalsResponse
    {
        public ParticleDeviceVitalsDiagnostics diagnostics { get; set; }
    }
}
