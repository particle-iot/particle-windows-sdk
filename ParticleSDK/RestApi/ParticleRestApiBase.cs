using Particle.SDK.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Particle.SDK.RestApi
{
    #region Event Handlers

    /// <summary>
    /// Delegete define for callback on unauthorized event
    /// </summary>
    public delegate void ClientUnauthorizedEventHandler();

    #endregion

    /// <summary>
    /// Base class for communicating with Particle Cloud via Windows.Web.Http
    /// </summary>
    public class ParticleRestApiBase
    {
        #region Internal Constants

        internal static readonly string ParticleApiUrl = "https://api.particle.io/";

        #endregion

        #region Protected Members

        protected object clientUnauthorizedLock = new object();
        protected string logedInUsername = "";
        protected ParticleAuthenticationResponse particleAuthentication = null;

        #endregion

        #region Events

        /// <summary>
        /// Global event called by ParticleCloud when any requests returns 401 Unauthorized
        /// </summary>
        public event ClientUnauthorizedEventHandler ClientUnauthorized;

        #endregion

        #region Properties

        /// <summary>
        /// AccessToken from Login/Signup calls
        /// </summary>
        public string AccessToken
        {
            get
            {
                return particleAuthentication?.AccessToken;
            }
        }

        /// <summary>
        /// OAuth Client Id for creating tokens
        /// </summary>
        public string OAuthClientId { get; set; } = "particle";

        /// <summary>
        /// OAuth Client Secret for creating tokens
        /// </summary>
        public string OAuthClientSecret { get; set; } = "particle";

        /// <summary>
        /// SynchronizationContext for dispatching calls
        /// </summary>
        public SynchronizationContext SynchronizationContext { get; set; }

        /// <summary>
        /// Username from Login/Signup call
        /// </summary>
        public string Username
        {
            get
            {
                return logedInUsername;
            }
        }

        #endregion

        #region Public Auth Methods

        /// <summary>
        /// Clear local state.
        /// Does not destroy the token from Particle Cloud
        /// </summary>
        public void Logout()
        {
            particleAuthentication = null;
            logedInUsername = "";
        }

        #endregion

        #region Public Virtual Device Methods

        /// <summary>
        /// Flash a compiled firmware to a device
        /// A return of true only means it was sent to the device, not that flash is successful
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <param name="firmwareStream">Stream of compiled binary</param>
        /// <param name="filename">Filename of compiled binary</param>
        /// <returns>Returns true if binary is sent to device</returns>
        public virtual Task<bool> DeviceFlashBinaryAsync(string deviceId, Stream firmwareStream, string filename)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Protected Auth Methods

        /// <summary>
        /// The client unauthorized event-invoking method
        /// </summary>
        protected void OnClientUnauthorized()
        {
            bool loggedOut = false;

            lock (clientUnauthorizedLock)
            {
                if (particleAuthentication != null)
                {
                    loggedOut = true;
                    Logout();
                }
            }

            if (loggedOut && ClientUnauthorized != null)
                ClientUnauthorized();
        }

        #endregion

        #region Protected Virtual Auth Methods

        /// <summary>
        /// Set the OAuth client members
        /// </summary>
        protected virtual void SetOAuthClient()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Protected Rest Methods

        /// <summary>
        /// Function to return full URI to Particle Cloud endpoint from relative path
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud endpoint</param>
        /// <returns>Returns full Uri given a partial path</returns>
        protected Uri CreateUriFromPathString(string path)
        {
            if (!string.IsNullOrWhiteSpace(AccessToken))
            {
                if (path.Contains("?"))
                    return new Uri($"{ParticleApiUrl}{path}&access_token={AccessToken}");
                return new Uri($"{ParticleApiUrl}{path}?access_token={AccessToken}");
            }
            else
                return new Uri($"{ParticleApiUrl}{path}");
        }

        #endregion

        #region Protected Virtual Rest Methods

        /// <summary>
        /// Genericized function to make requests to the Particle Cloud and throw exceptions on HTTP errors
        /// </summary>
        /// <param name="request">HttpRequestMessage with path and method</param>
        /// <returns>Retuns string response from Particle Cloud request</returns>
        protected virtual Task<string> SendAsync(object request, bool sendAuthHeader = false)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Protected Virtual Event Methods

        /// <summary>
        /// Long running task to listen for events
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud event endpoint</param>
        /// <param name="particleEventGroup">ParticleEventGroup to send new events to</param>
        /// <returns>Returns Task of long running event task</returns>
        protected virtual Task ListenForEventAsync(string path, ParticleEventGroup particleEventGroup)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Internal Threading Methods

        /// <summary>
        /// Dispatches an asynchronous message to a synchronization if set, otherwise calls the callback
        /// </summary>
        /// <param name="d">The System.Threading.SendOrPostCallback delegate to call</param>
        /// <param name="state">The object passed to the delegate</param>
        internal void SynchronizationContextPost(SendOrPostCallback d, object state)
        {
            if (SynchronizationContext != null)
                SynchronizationContext.Post(d, state);
            else
                d(state);
        }

        #endregion

        #region Internal Virtual Rest Methods

        /// <summary>
        /// Make a DELETE requests to the Particle Cloud
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud endpoint</param>
        /// <returns>Retuns string response from Particle Cloud DELETE request</returns>
        internal virtual Task<string> DeleteDataAsync(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Make a GET requests to the Particle Cloud
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud endpoint</param>
        /// <returns>Retuns string response from Particle Cloud GET request</returns>
        internal virtual Task<string> GetDataAsync(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Make a POST requests to the Particle Cloud
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud endpoint</param>
        /// <returns>Retuns string response from Particle Cloud POST request</returns>
        internal virtual Task<string> PostDataAsync(string path, bool sendAuthHeader = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Make a POST requests to the Particle Cloud
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud endpoint</param>
        /// <param name="data">Dictonary of key/value pairs to convert to form url encoded content</param>
        /// <returns>Retuns string response from Particle Cloud POST request</returns>
        internal virtual Task<string> PostDataAsync(string path, Dictionary<string, string> data = null, bool sendAuthHeader = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Make a POST requests to the Particle Cloud
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud endpoint</param>
        /// <param name="content">Content to send in request</param>
        /// <returns>Retuns string response from Particle Cloud POST request</returns>
        internal virtual Task<string> PostDataAsync(string path, object content = null, bool sendAuthHeader = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Make a PUT requests to the Particle Cloud
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud endpoint</param>
        /// <returns>Retuns string response from Particle Cloud PUT request</returns>
        internal virtual Task<string> PutDataAsync(string path, bool sendAuthHeader = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Make a PUT requests to the Particle Cloud
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud endpoint</param>
        /// <param name="data">Dictonary of key/value pairs to convert to form url encoded content</param>
        /// <returns>Retuns string response from Particle Cloud PUT request</returns>
        internal virtual Task<string> PutDataAsync(string path, Dictionary<string, string> data = null, bool sendAuthHeader = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Make a PUT requests to the Particle Cloud
        /// </summary>
        /// <param name="path">Relative path to Particle Cloud endpoint</param>
        /// <param name="content">Content to send in request</param>
        /// <returns>Retuns string response from Particle Cloud PUT request</returns>
        internal virtual Task<string> PutDataAsync(string path, object content = null, bool sendAuthHeader = false)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}