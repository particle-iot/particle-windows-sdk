using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Particle.SDK.Models;
using Particle.SDK.RestApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Particle.SDK
{
    /// <summary>
    /// Class for communicating with Particle Cloud
    /// </summary>
    public class ParticleCloud : ParticleRestApi
    {
        #region Internal Constants

        internal static readonly string ParticleApiVersion = "v1";

        internal static readonly string ParticleApiPahtLogin = "oauth/token";
        internal static readonly string ParticleApiPathSignup = "users";
        internal static readonly string ParticleApiPathPasswordReset = "user/password-reset";
        internal static readonly string ParticleApiPathUser = "user";
        internal static readonly string ParticleApiPathDevices = "devices";
        internal static readonly string ParticleApiPathClaimCode = "device_claims";
        internal static readonly string ParticleApiPathSimDataUsage = "sims/{0}/data_usage";
        internal static readonly string ParticleApiPathEvents = "events";
        internal static readonly string ParticleApiPathDevicesEvents = "devices/events";
        internal static readonly string ParticleApiPathDeviceEvents = "devices/{0}/events";
        internal static readonly string ParticleApiPathCustomerSignup = "orgs/{0}/customers";
        internal static readonly string ParticleApiPathCustomerPasswordReset = "orgs/{0}/customers/reset_password";
        internal static readonly string ParticleApiPathOrganizationClaimCode = "orgs/{0}/products/{1}/device_claims";

        #endregion

        #region Private Members

        private Dictionary<string, ParticleEventGroup> particleEventGroups = null;
        private Dictionary<Guid, ParticleEventHandlerData> particleEventHandlerDatas = null;

        #endregion

        #region Private Static Members

        private static ParticleCloud particleCloud = null;

        #endregion

        #region Static Properties

        /// <summary>
        /// Instance of a static shared ParticleCloud for simplicity
        /// </summary>
        public static ParticleCloud SharedCloud
        {
            get
            {
                if (particleCloud == null)
                    particleCloud = new ParticleCloud();

                return particleCloud;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of ParticleCloud
        /// </summary>
        /// <param name="accessToken">A token from a prevous OAuth call</param>
        public ParticleCloud(string accessToken = null)
        {
            SetOAuthClient();

            if (accessToken != null)
            {
                particleAuthentication = new ParticleAuthenticationResponse();
                particleAuthentication.AccessToken = accessToken;
            }
        }

        #endregion

        #region Public Auth Methods

        /// <summary>
        /// Create a token for a user via OAuth
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <param name="password">Password for the user</param>
        /// <param name="expiresIn">Seconds in which to expire the token. Zero for never</param>
        /// <returns>Returns a ParticleAuthenticationResponse</returns>
        public async Task<ParticleAuthenticationResponse> CreateTokenAsync(string username, string password, int expiresIn = 0)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            var data = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"username", username},
                {"password", password},
                {"expires_in", expiresIn.ToString()}
            };

            try
            {
                var responseContent = await PostDataAsync(ParticleApiPahtLogin, data, true);
                var results = JsonConvert.DeserializeObject<ParticleAuthenticationResponse>(responseContent);
                if (results != null && !string.IsNullOrWhiteSpace(results.AccessToken))
                    return results;

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Login a user and save the state if valid
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <param name="password">Password for the user</param>
        /// <param name="expiresIn">Seconds in which to expire the token. Zero for never</param>
        /// <returns>Returns true if the user credentials are valid</returns>
        public async Task<bool> LoginAsync(string username, string password, int expiresIn = 0)
        {
            var results = await CreateTokenAsync(username, password, expiresIn);
            if (results != null)
            {
                particleAuthentication = results;
                logedInUsername = username;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Request a password reset email be sent to the user
        /// </summary>
        /// <param name="username">Username to request password reset for</param>
        /// <returns>Returns true if the username exists</returns>
        public async Task<bool> RequestPasswordResetAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));

            var data = new Dictionary<string, string>
            {
                {"username", username}
            };

            try
            {
                var responseContent = await PostDataAsync($"{ParticleApiVersion}/{ParticleApiPathPasswordReset}", data);
                var results = JsonConvert.DeserializeObject<ParticleGenericResponse>(responseContent);
                if (results != null && results.Ok)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Set the authentication values from previous values
        /// </summary>
        /// <param name="accessToken">Previously saved access token</param>
        /// <param name="expires_in">Previously saved expires in value</param>
        /// <param name="refresh_token">Previously saved refresh token</param>
        public void SetAuthentication(string accessToken, int? expires_in = null, string refresh_token = null)
        {
            particleAuthentication = new ParticleAuthenticationResponse();
            particleAuthentication.AccessToken = accessToken;
            if (expires_in != null)
                particleAuthentication.ExpiresIn = expires_in.Value;
            if (refresh_token != null)
                particleAuthentication.RefreshToken = refresh_token;
        }

        /// <summary>
        /// Create a new user and save the state
        /// </summary>
        /// <param name="username">Username of new user</param>
        /// <param name="password">Password for new user</param>
        /// <returns>Returns true if the new user is signed up</returns>
        public async Task<bool> SignupAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            var data = new Dictionary<string, string>
            {
                {"username", username},
                {"password", password},
            };

            try
            {
                var responseContent = await PostDataAsync($"{ParticleApiVersion}/{ParticleApiPathSignup}", data);
                var result = JToken.Parse(responseContent);
                if ((bool)result["ok"] == true)
                {
                    return await LoginAsync(username, password);
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate a token and save the state if valid
        /// </summary>
        /// <param name="token">A token from a prevous OAuth call</param>
        /// <returns>Returns true if the token is valid</returns>
        public async Task<bool> TokenLoginAsync(string token)
        {
            particleAuthentication = new ParticleAuthenticationResponse();
            particleAuthentication.AccessToken = token;

            try
            {
                var responseContent = await GetDataAsync($"{ParticleApiVersion}/{ParticleApiPathUser}");
                return true;
            }
            catch
            {
                Logout();
                return false;
            }
        }

        #endregion

        #region Public Device Methods

        /// <summary>
        /// Claim a device
        /// </summary>
        /// <param name="deviceId">Device ID to claim</param>
        /// <returns>Returns true if a device could be claimed</returns>
        public async Task<bool> ClaimDeviceAsync(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentNullException(nameof(deviceId));

            var data = new Dictionary<string, string>
            {
                {"id", deviceId}
            };

            try
            {
                var responseContent = await PostDataAsync($"{ParticleApiVersion}/{ParticleApiPathDevices}", data);
                var result = JToken.Parse(responseContent);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Create a claim code for sending to a device in listening mode
        /// </summary>
        /// <returns>Returns a claim code</returns>
        public async Task<string> CreateClaimCodeAsync()
        {
            try
            {
                var responseContent = await PostDataAsync($"{ParticleApiVersion}/{ParticleApiPathClaimCode}");
                var result = JToken.Parse(responseContent);
                return (string)result["claim_code"];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Flash a compiled firmware to a device
        /// A return of true only means it was sent to the device, not that flash is successful
        /// </summary>
        /// <param name="particeDevice">ParticleDevice</param>
        /// <param name="firmwareStream">Stream of compiled binary</param>
        /// <param name="filename">Filename of compiled binary</param>
        /// <returns>Returns true if binary is sent to device</returns>
        public async Task<bool> DeviceFlashBinaryAsync(ParticleDevice particeDevice, Stream firmwareStream, string filename)
        {
            return await DeviceFlashBinaryAsync(particeDevice.Id, firmwareStream, filename);
        }

        /// <summary>
        /// Gets a device
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <returns>Returns a ParticleDevice</returns>
        public async Task<ParticleDevice> GetDeviceAsync(string deviceId)
        {
            try
            {
                ParticleDevice device = new ParticleDevice(deviceId, this);
                await device.RefreshAsync();

                return device;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a list of all users devices
        /// </summary>
        /// <returns>Returns a List of ParticleDevices</returns>
        public async Task<List<ParticleDevice>> GetDevicesAsync()
        {
            try
            {
                var jsonSerializer = new JsonSerializer();
                jsonSerializer.DateTimeZoneHandling = DateTimeZoneHandling.Local;

                var responseContent = await GetDataAsync($"{ParticleApiVersion}/{ParticleApiPathDevices}");
                var result = JToken.Parse(responseContent);

                List<ParticleDevice> devices = new List<ParticleDevice>();
                foreach (JObject device in (JArray)result)
                {
                    ParticleDeviceResponse deviceState = device.ToObject<ParticleDeviceResponse>(jsonSerializer);
                    ParticleDevice particleDevice = new ParticleDevice(deviceState, this);
                    devices.Add(particleDevice);
                }

                foreach (ParticleDevice device in devices)
                {
                    if (device.Connected)
                        await device.RefreshAsync();
                }

                return new List<ParticleDevice>(devices);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Public Organization Methods

        /// <summary>
        /// Create a new user in an organization
        /// </summary>
        /// <param name="organizationSlug">Organization slug</param>
        /// <param name="email">Email of new user</param>
        /// <param name="password">Password for new user</param>
        /// <returns>Returns true if the new user is signed up</returns>
        public async Task<bool> SignupWithCustomerAsync(string organizationSlug, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(organizationSlug))
                throw new ArgumentNullException(nameof(organizationSlug));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            var data = new Dictionary<string, string>
            {
                {"email", email},
                {"password", password}
            };

            try
            {
                string path = string.Format(ParticleApiPathCustomerSignup, organizationSlug);
                var responseContent = await PostDataAsync($"{ParticleApiVersion}/{path}", data, true);
                var results = JsonConvert.DeserializeObject<ParticleAuthenticationResponse>(responseContent);
                if (!string.IsNullOrWhiteSpace(results.AccessToken))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Request a password reset email be sent to the user of the organization
        /// </summary>
        /// <param name="organizationSlug">Organization slug</param>
        /// <param name="email">Username to request password reset for</param>
        /// <returns>Returns true if the username exists</returns>
        public async Task<bool> RequestPasswordResetForCustomerAsync(string organizationSlug, string email)
        {
            if (string.IsNullOrWhiteSpace(organizationSlug))
                throw new ArgumentNullException(nameof(organizationSlug));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException(nameof(email));

            var data = new Dictionary<string, string>
            {
                {"email", email}
            };

            try
            {
                string path = string.Format(ParticleApiPathCustomerPasswordReset, organizationSlug);
                var responseContent = await PostDataAsync($"{ParticleApiVersion}/{path}", data);
                var results = JsonConvert.DeserializeObject<ParticleGenericResponse>(responseContent);
                if (results != null && results.Ok)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Create a claim code for sending to a device in listening mode for products in an organization
        /// </summary>
        /// <param name="organizationSlug">Organization slug</param>
        /// <param name="productSlug">Pruduct slug</param>
        /// <param name="activationCode">activation code</param>
        /// <returns>Returns a claim code</returns>
        public async Task<string> CreateClaimCodeForOrganizationAsync(string organizationSlug, string productSlug, string activationCode = null)
        {
            if (string.IsNullOrWhiteSpace(productSlug))
                throw new ArgumentNullException(nameof(productSlug));
            if (string.IsNullOrWhiteSpace(organizationSlug))
                throw new ArgumentNullException(nameof(organizationSlug));

            var data = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(activationCode))
                data["activation_code"] = activationCode;

            try
            {
                string path = string.Format(ParticleApiPathOrganizationClaimCode, organizationSlug, productSlug);
                var responseContent = await PostDataAsync($"{ParticleApiVersion}/{path}", data);
                var result = JToken.Parse(responseContent);
                return (string)result["claim_code"];
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Public Event Methods

        /// <summary>
        /// Publish an event to the cloud
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        /// <param name="eventData">String data to send with the event</param>
        /// <param name="isPrivate">Boolean boolean flag determining if this event is private or not</param>
        /// <param name="ttl">Time To Live in seconds</param>
        /// <returns></returns>
        public async Task<bool> PublishEventAsync(string eventName, string eventData = "", bool isPrivate = true, int ttl = 60)
        {
            if (string.IsNullOrWhiteSpace(eventName))
                throw new ArgumentNullException(nameof(eventName));

            var data = new Dictionary<string, string>
            {
                {"name", eventName},
                {"data", eventData},
                {"private", isPrivate.ToString().ToLower()},
                {"ttl", ttl.ToString()},
            };

            try
            {
                var responseContent = await PostDataAsync($"{ParticleApiVersion}/{ParticleApiPathDevicesEvents}", data);
                var result = JToken.Parse(responseContent);
                if ((bool)result["ok"] == true)
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a new long running task to listen for all events
        /// </summary>
        /// <param name="eventHandler">ParticleEventHandler to call when new event arrives</param>
        /// <param name="eventNamePrefix">Prefix to monitor on event stream</param>
        /// <returns>Returns GUID reference to long running event task</returns>
        public async Task<Guid> SubscribeToAllEventsWithPrefixAsync(ParticleEventHandler eventHandler, string eventNamePrefix = "")
        {
            if (string.IsNullOrWhiteSpace(eventNamePrefix))
                return await SubscribeToEventAsync($"{ParticleApiVersion}/{ParticleApiPathEvents}", eventHandler);
            else
                return await SubscribeToEventAsync($"{ParticleApiVersion}/{ParticleApiPathEvents}/{eventNamePrefix}", eventHandler);
        }

        /// <summary>
        /// Creates a new long running task to listen for events on a specific users device
        /// </summary>
        /// <param name="eventHandler">ParticleEventHandler to call when new event arrives</param>
        /// <param name="device">ParticleDevice to limit event stream to</param>
        /// <param name="eventNamePrefix">Prefix to monitor on event stream</param>
        /// <returns>Returns GUID reference to long running event task</returns>
        public async Task<Guid> SubscribeToDeviceEventsWithPrefixAsync(ParticleEventHandler eventHandler, ParticleDevice device, string eventNamePrefix = "")
        {
            return await SubscribeToDeviceEventsWithPrefixAsync(eventHandler, device.Id, eventNamePrefix);
        }

        /// <summary>
        /// Creates a new long running task to listen for events on a specific users device
        /// </summary>
        /// <param name="eventHandler">ParticleEventHandler to call when new event arrives</param>
        /// <param name="deviceId">ParticleDevice ID to limit event stream to</param>
        /// <param name="eventNamePrefix">Prefix to monitor on event stream</param>
        /// <returns>Returns GUID reference to long running event task</returns>
        public async Task<Guid> SubscribeToDeviceEventsWithPrefixAsync(ParticleEventHandler eventHandler, string deviceId, string eventNamePrefix = "")
        {
            string path = string.Format(ParticleApiPathDeviceEvents, deviceId);

            if (string.IsNullOrWhiteSpace(eventNamePrefix))
                return await SubscribeToEventAsync($"{ParticleApiVersion}/{path}", eventHandler);
            else
                return await SubscribeToEventAsync($"{ParticleApiVersion}/{path}/{eventNamePrefix}", eventHandler);
        }

        /// <summary>
        /// Creates a new long running task to listen for events on users devices
        /// </summary>
        /// <param name="eventHandler">ParticleEventHandler to call when new event arrives</param>
        /// <param name="eventNamePrefix">Prefix to monitor on event stream</param>
        /// <returns>Returns GUID reference to long running event task</returns>
        public async Task<Guid> SubscribeToDevicesEventsWithPrefixAsync(ParticleEventHandler eventHandler, string eventNamePrefix = "")
        {
            if (string.IsNullOrWhiteSpace(eventNamePrefix))
                return await SubscribeToEventAsync($"{ParticleApiVersion}/{ParticleApiPathDevicesEvents}", eventHandler);
            else
                return await SubscribeToEventAsync($"{ParticleApiVersion}/{ParticleApiPathDevicesEvents}/{eventNamePrefix}", eventHandler);
        }

        /// <summary>
        /// Removes ParticleEventHandler linked to a specified GUID, stoping the handler from receiving events
        /// and if it's the last one shutting down the long running event 
        /// </summary>
        /// <param name="eventListenerID">GUID from a previous call to subscribe to an event</param>
        public void UnsubscribeFromEvent(Guid eventListenerID)
        {
            if (particleEventHandlerDatas == null)
                return;

            if (!particleEventHandlerDatas.ContainsKey(eventListenerID))
                return;

            var particleEventHandlerData = particleEventHandlerDatas[eventListenerID];
            particleEventHandlerDatas.Remove(eventListenerID);

            var particleEventGroup = particleEventGroups[particleEventHandlerData.Path];
            particleEventGroup.RemoveHandler(particleEventHandlerData.EventHandler);

            if (!particleEventGroup.HasHandlers)
                particleEventGroups.Remove(particleEventHandlerData.Path);
        }

        #endregion

        #region Private Event Methods

        /// <summary>
        /// Creates a new long running task to listen for events
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud event endpoint</param>
        /// <param name="eventHandler">ParticleEventHandler to call when new event arrives</param>
        /// <returns>Returns GUID reference to long running event task</returns>
        private async Task<Guid> SubscribeToEventAsync(string path, ParticleEventHandler eventHandler)
        {
            var guid = Guid.NewGuid();
            bool newEvent = false;

            if (particleEventGroups == null)
            {
                particleEventGroups = new Dictionary<string, ParticleEventGroup>();
                particleEventHandlerDatas = new Dictionary<Guid, ParticleEventHandlerData>();
            }

            if (!particleEventGroups.ContainsKey(path))
            {
                particleEventGroups.Add(path, new ParticleEventGroup());
                newEvent = true;
            }

            particleEventHandlerDatas.Add(guid, new ParticleEventHandlerData(path, eventHandler));

            var particleEventGroup = particleEventGroups[path];
            particleEventGroup.AddHandler(eventHandler);

            if (newEvent)
                await Task.Factory.StartNew(() => ListenForEventAsync(path, particleEventGroup).ConfigureAwait(false), TaskCreationOptions.LongRunning);

            return guid;
        }

        #endregion
    }
}
