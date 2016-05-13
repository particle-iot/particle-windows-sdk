using Newtonsoft.Json;

namespace Particle.SDK.Models
{
    /// <summary>
    /// Helper class from Particle Cloud Authentication request
    /// </summary>
    public class ParticleAuthenticationResponse
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
