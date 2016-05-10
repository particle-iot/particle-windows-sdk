using Newtonsoft.Json;
using Particle.SDK.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Particle.SDK.RestApi
{
    /// <summary>
    /// Base class for communicating with Particle Cloud via System.Nwt.Http
    /// </summary>
    public class ParticleRestApi : ParticleRestApiBase
    {
        #region Public Device Method Overrides

        /// <summary>
        /// Flash a compiled firmware to a device
        /// A return of true only means it was sent to the device, not that flash is successful
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <param name="firmwareStream">Stream of compiled binary</param>
        /// <param name="filename">Filename of compiled binary</param>
        /// <returns>Returns true if binary is sent to device</returns>
        public override async Task<bool> DeviceFlashBinaryAsync(string deviceId, Stream firmwareStream, string filename)
        {
            if (deviceId == null)
                throw new ArgumentNullException(nameof(deviceId));
            if (firmwareStream == null)
                throw new ArgumentNullException(nameof(firmwareStream));

            using (HttpContent file = new StreamContent(firmwareStream))
            {
                file.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

                var content = new MultipartFormDataContent();
                content.Add(new StringContent("binary"), "\"file_type\"");
                content.Add(file, "\"file\"", filename);

                try
                {
                    var responseContent = await PutDataAsync($"{ParticleCloud.ParticleApiVersion}/{ParticleCloud.ParticleApiPathDevices}/{deviceId}", content);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        #endregion

        #region Protected Auth Method Overrides

        /// <summary>
        /// Set the OAuth client members
        /// </summary>
        protected override void SetOAuthClient()
        {
        }

        #endregion

        #region Protected Rest Method Overrides

        /// <summary>
        /// Genericized function to make requests to the Particle Cloud and throw exceptions on HTTP errors
        /// </summary>
        /// <param name="request">HttpRequestMessage with path and method</param>
        /// <returns>Retuns string response from Particle Cloud request</returns>
        protected async Task<string> SendAsync(HttpRequestMessage request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
                var response = await client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return responseContent;

                    case HttpStatusCode.Unauthorized:
                        OnClientUnauthorized();
                        throw new ParticleUnauthorizedException(responseContent);

                    case HttpStatusCode.NotFound:
                        throw new ParticleNotFoundException(responseContent);

                    case HttpStatusCode.BadRequest:
                        throw new ParticleRequestBadRequestException(responseContent);

                    default:
                        throw new Exception();
                }

            }
        }

        #endregion

        #region Protected Event Method Overrides

        /// <summary>
        /// Long running task to listen for events
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud event endpoint</param>
        /// <param name="particleEventGroup">ParticleEventGroup to send new events to</param>
        /// <returns>Returns Task of long running event task</returns>
        protected override async Task ListenForEventAsync(string path, ParticleEventGroup particleEventGroup)
        {
            var httpListener = new HttpListener();
            string eventName = "";

            try
            {
                var url = new Uri($"https://api.particle.io/{path}/?access_token={AccessToken}");
                var request = WebRequest.Create(url);
                request.Method = "GET";

                var task = Task.Factory.StartNew(() => httpListener.Start());

                using (var response = await request.GetResponseAsync())
                {
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        while (!reader.EndOfStream && particleEventGroup.HasHandlers)
                        {
                            var outputString = reader.ReadLine();

                            if (outputString.StartsWith("event:"))
                            {
                                eventName = outputString.Substring(6).Trim();
                            }
                            else if (outputString.StartsWith("data:") && !string.IsNullOrWhiteSpace(eventName))
                            {
                                var jsonSerializerSettings = new JsonSerializerSettings() { DateTimeZoneHandling = DateTimeZoneHandling.Local };
                                var particleEventResponse = JsonConvert.DeserializeObject<ParticleEventResponse>(outputString.Substring(5), jsonSerializerSettings);
                                particleEventResponse.Name = eventName;
                                eventName = "";

                                SynchronizationContextPost(a =>
                                {
                                    particleEventGroup.NewMessage(this, particleEventResponse);
                                }, null);
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            httpListener.Stop();
        }

        #endregion

        #region Internal Rest Method Overrides

        /// <summary>
        /// Make a DELETE requests to the Particle Cloud
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud endpoint</param>
        /// <returns>Retuns string response from Particle Cloud DELETE request</returns>
        internal override async Task<string> DeleteDataAsync(string path)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, CreateUriFromPathString(path));
            return await SendAsync(request);
        }

        /// <summary>
        /// Make a GET requests to the Particle Cloud
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud endpoint</param>
        /// <returns>Retuns string response from Particle Cloud GET request</returns>
        internal override async Task<string> GetDataAsync(string path)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, CreateUriFromPathString(path));
            return await SendAsync(request);
        }

        /// <summary>
        /// Make a POST requests to the Particle Cloud
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud endpoint</param>
        /// <returns>Retuns string response from Particle Cloud POST request</returns>
        internal override async Task<string> PostDataAsync(string path)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, CreateUriFromPathString(path));
            return await SendAsync(request);
        }

        /// <summary>
        /// Make a POST requests to the Particle Cloud
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud endpoint</param>
        /// <param name="data">Dictonary of key/value pairs to convert to form url encoded content</param>
        /// <returns>Retuns string response from Particle Cloud POST request</returns>
        internal override async Task<string> PostDataAsync(string path, Dictionary<string, string> data = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, CreateUriFromPathString(path));
            if (data != null)
                request.Content = new FormUrlEncodedContent(data);

            return await SendAsync(request);
        }

        /// <summary>
        /// Make a POST requests to the Particle Cloud
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud endpoint</param>
        /// <param name="content">IHttpContent to send in request</param>
        /// <returns>Retuns string response from Particle Cloud POST request</returns>
        internal async Task<string> PostDataAsync(string path, HttpContent content = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, CreateUriFromPathString(path));
            request.Content = content;
            return await SendAsync(request);
        }

        /// <summary>
        /// Make a PUT requests to the Particle Cloud
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud endpoint</param>
        /// <returns>Retuns string response from Particle Cloud PUT request</returns>
        internal override async Task<string> PutDataAsync(string path)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, CreateUriFromPathString(path));
            return await SendAsync(request);
        }

        /// <summary>
        /// Make a PUT requests to the Particle Cloud
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud endpoint</param>
        /// <param name="data">Dictonary of key/value pairs to convert to form url encoded content</param>
        /// <returns>Retuns string response from Particle Cloud PUT request</returns>
        internal override async Task<string> PutDataAsync(string path, Dictionary<string, string> data = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, CreateUriFromPathString(path));
            if (data != null)
                request.Content = new FormUrlEncodedContent(data);

            return await SendAsync(request);
        }

        /// <summary>
        /// Make a PUT requests to the Particle Cloud
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud endpoint</param>
        /// <param name="content">IHttpContent to send in request</param>
        /// <returns>Retuns string response from Particle Cloud PUT request</returns>
        internal async Task<string> PutDataAsync(string path, HttpContent content = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, CreateUriFromPathString(path));
            request.Content = content;
            return await SendAsync(request);
        }

        #endregion
    }
}