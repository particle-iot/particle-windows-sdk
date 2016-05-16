using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Particle.SDK.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Particle.SDK
{
    #region Enums

    /// <summary>
    /// Enumeration for Particle Device types
    /// </summary>
    public enum ParticleDeviceType
    {
        Unknown = -1,
        ParticleCore = 0,
        ParticlePhoton = 6,
        ParticleP1 = 8,
        ParticleElectron = 10,

        // Non Particle Devices
        DigistumpOak = 82,
        RedBearDuo = 88,
        BluzDK = 103,
    }

    /// <summary>
    /// Enumeration for internally used Particle Device States
    /// </summary>
    public enum ParticleDeviceState
    {
        Unknown,
        Offline,
        Flashing,
        Online,
        Tinker
    }

    #endregion

    /// <summary>
    /// Class for using a device with Particle Cloud
    /// implements the IPropertyChange interface.
    /// </summary>
    public class ParticleDevice : INotifyPropertyChanged
    {
        #region Private Members

        private ParticleDeviceResponse deviceState = null;
        private ParticleCloud particleCloud = null;
        private ParticleDeviceState state = ParticleDeviceState.Unknown;
        private bool isFlashing = false;
        private double mbsUsed = 0;
        private Guid? onlineEventListenerID = null;

        #endregion

        #region Events

        /// <summary>
        /// Global event called by ParticleDevice when any property is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Readonly Id from ParticleDeviceResponse
        /// </summary>
        public string Id
        {
            get { return deviceState.Id; }
        }

        /// <summary>
        /// Readonly Name from ParticleDeviceResponse
        /// </summary>
        public string Name
        {
            get { return deviceState.Name; }
        }

        /// <summary>
        /// Readonly LastApp from ParticleDeviceResponse
        /// </summary>
        public string LastApp
        {
            get { return deviceState.LastApp; }
        }

        /// <summary>
        /// Readonly LastIPAddress from ParticleDeviceResponse
        /// </summary>
        public string LastIPAddress
        {
            get { return deviceState.LastIPAddress; }
        }

        /// <summary>
        /// Readonly LastHeard from ParticleDeviceResponse
        /// </summary>
        public DateTime LastHeard
        {
            get { return deviceState.LastHeard; }
        }

        /// <summary>
        /// Readonly RequiresDeepUpdate from ParticleDeviceResponse
        /// </summary>
        public bool RequiresDeepUpdate
        {
            get { return deviceState.RequiresDeepUpdate; }
        }

        /// <summary>
        /// Readonly ProductId from ParticleDeviceResponse
        /// </summary>
        public int ProductId
        {
            get
            {
                return deviceState.ProductId;
            }
        }

        /// <summary>
        /// Readonly Known ProductId from ParticleDeviceResponse
        /// </summary>
        public ParticleDeviceType KnownProductId
        {
            get
            {
                return IntToParticleDeviceType(deviceState.ProductId);
            }
        }

        /// <summary>
        /// Readonly Connected from ParticleDeviceResponse
        /// </summary>
        public bool Connected
        {
            get { return deviceState.Connected; }
        }

        /// <summary>
        /// Readonly PlatformId from ParticleDeviceResponse
        /// </summary>
        public int PlatformId
        {
            get
            {
                return deviceState.PlatformId;
            }
        }

        /// <summary>
        /// Readonly Known PlatformId from ParticleDeviceResponse
        /// </summary>
        public ParticleDeviceType KnownPlatformId
        {
            get
            {
                return IntToParticleDeviceType(deviceState.PlatformId);
            }
        }

        /// <summary>
        /// Readonly Cellular from ParticleDeviceResponse
        /// </summary>
        public bool Cellular
        {
            get { return deviceState.Cellular; }
        }

        /// <summary>
        /// Readonly Status from ParticleDeviceResponse
        /// </summary>
        public string Status
        {
            get { return deviceState.Status; }
        }

        /// <summary>
        /// Readonly LastICCID from ParticleDeviceResponse
        /// </summary>
        public string LastICCID
        {
            get { return deviceState.LastICCID; }
        }

        /// <summary>
        /// Readonly IMEI from ParticleDeviceResponse
        /// </summary>
        public string IMEI
        {
            get { return deviceState.IMEI; }
        }

        /// <summary>
        /// Readonly CurrentBuildTarget from ParticleDeviceResponse
        /// </summary>
        public string CurrentBuildTarget
        {
            get { return deviceState.CurrentBuildTarget; }
        }

        /// <summary>
        /// Readonly Variables from ParticleDeviceResponse
        /// </summary>
        public Dictionary<string, string> Variables
        {
            get { return deviceState.Variables; }
        }

        /// <summary>
        /// Readonly Functions from ParticleDeviceResponse
        /// </summary>
        public string[] Functions
        {
            get { return deviceState.Functions; }
        }

        /// <summary>
        /// Readonly CC3000PatchVersion from ParticleDeviceResponse
        /// </summary>
        public string CC3000PatchVersion
        {
            get { return deviceState.CC3000PatchVersion; }
        }

        /// <summary>
        /// Readonly internally created State
        /// </summary>
        public ParticleDeviceState State
        {
            get { return state; }
            internal set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }

        /// <summary>
        /// Readonly value of flashing state
        /// </summary>
        public bool IsFlashing
        {
            get { return isFlashing; }
            internal set
            {
                isFlashing = value;
                OnPropertyChanged("IsFlashing");
            }
        }

        /// <summary>
        /// Readonly amount of megabytes used in billing period for an Electron
        /// </summary>
        public double MbsUsed
        {
            get { return mbsUsed; }
            internal set
            {
                mbsUsed = value;
                OnPropertyChanged("MbsUsed");
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of a ParticleDevice from a ParticleDeviceResponse
        /// </summary>
        /// <param name="deviceState">ParticleDeviceResponse from Particle Cloud</param>
        /// <param name="particleCloud">Authorized ParticleCloud</param>
        public ParticleDevice(ParticleDeviceResponse deviceState, ParticleCloud particleCloud)
        {
            this.deviceState = deviceState;
            this.particleCloud = particleCloud;

            UpdateState();
        }

        /// <summary>
        /// Create a new instance of a ParticleDevice with only an ID
        /// </summary>
        /// <param name="id">Device ID</param>
        /// <param name="particleCloud">Authorized ParticleCloud</param>
        public ParticleDevice(string deviceId, ParticleCloud particleCloud)
        {
            this.deviceState = new ParticleDeviceResponse();
            this.deviceState.Id = deviceId;
            this.particleCloud = particleCloud;

            UpdateState();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Used when you detect the device is flashing by external means and wish to update the model
        /// </summary>
        /// <param name="completed">True if the flash is completed</param>
        public void FlagFlashStatusChange(bool completed = false)
        {
            IsFlashing = !completed;
            if (!completed)
                State = ParticleDeviceState.Flashing;
        }

        /// <summary>
        /// Flash a compiled firmware to a device
        /// A return of true only means it was sent to the device, not that flash is successful
        /// </summary>
        /// <param name="firmwareStream">Stream of compiled binary</param>
        /// <param name="filename">Filename of compiled binary</param>
        /// <param name="monitorStatus">Whether or not to monitor status</param>
        /// <returns>Returns true if binary is sent to device</returns>
        public async Task<bool> FlashBinaryAsync(Stream firmwareStream, string filename, bool monitorStatus = false)
        {
            if (firmwareStream == null)
                throw new ArgumentNullException(nameof(firmwareStream));

            try
            {
                IsFlashing = true;
                State = ParticleDeviceState.Flashing;
                if (monitorStatus)
                    MonitorForOnlineEvent();
                await particleCloud.DeviceFlashBinaryAsync(this, firmwareStream, filename);
                return true;
            }
            catch
            {
                IsFlashing = false;
                State = ParticleDeviceState.Unknown;
                return false;
            }
        }

        /// <summary>
        /// Flash a known app to a device
        /// A return of true only means it was sent to the device, not that flash is successful
        /// </summary>
        /// <param name="app">Known app name by Particle Cloud</param>
        /// <param name="monitorStatus">Whether or not to monitor status</param>
        /// <returns>Returns true if known app is sent to device</returns>
        public async Task<bool> FlashKnownAppAsync(string app, bool monitorStatus = false)
        {
            if (string.IsNullOrWhiteSpace(app))
                throw new ArgumentNullException(nameof(app));

            var data = new Dictionary<string, string>
            {
                {"app", app}
            };

            try
            {
                IsFlashing = true;
                State = ParticleDeviceState.Flashing;
                if (monitorStatus)
                    MonitorForOnlineEvent();
                var responseContent = await particleCloud.PutDataAsync($"{ParticleCloud.ParticleApiVersion}/{ParticleCloud.ParticleApiPathDevices}/{Id}", data);
                return true;
            }
            catch
            {
                IsFlashing = false;
                State = ParticleDeviceState.Unknown;
                return false;
            }
        }

        /// <summary>
        /// Retrieve a variable from a device
        /// </summary>
        /// <param name="variable">Variable name</param>
        /// <returns>Returns a ParticleVariableResponse</returns>
        public async Task<ParticleVariableResponse> GetVariableAsync(string variable)
        {
            if (string.IsNullOrWhiteSpace(variable))
                throw new ArgumentNullException(nameof(variable));

            try
            {
                var jsonSerializerSettings = new JsonSerializerSettings() { DateTimeZoneHandling = DateTimeZoneHandling.Local };

                var responseContent = await particleCloud.GetDataAsync($"{ParticleCloud.ParticleApiVersion}/{ParticleCloud.ParticleApiPathDevices}/{Id}/{variable}");
                return JsonConvert.DeserializeObject<ParticleVariableResponse>(responseContent, jsonSerializerSettings);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Check device functions to see if it's compatible with tinker
        /// </summary>
        /// <returns>Returns true if device is compatible with tinker</returns>
        public bool IsRunningTinker()
        {
            var lowercaseFunctions = new List<string>();
            foreach (string function in Functions)
            {
                lowercaseFunctions.Add(function.ToLower());
            }
            string[] tinkerFunctions = { "digitalread", "digitalwrite", "analogread", "analogwrite" };

            return (Connected && !tinkerFunctions.Except(lowercaseFunctions).Any());
        }

        /// <summary>
        /// Refreshes a devices properties with current information
        /// </summary>
        /// <returns>Returns true if the device is updated</returns>
        public async Task<bool> RefreshAsync()
        {
            try
            {
                var jsonSerializerSettings = new JsonSerializerSettings() { DateTimeZoneHandling = DateTimeZoneHandling.Local };

                var responseContent = await particleCloud.GetDataAsync($"{ParticleCloud.ParticleApiVersion}/{ParticleCloud.ParticleApiPathDevices}/{Id}");
                ParticleDeviceResponse deviceState = JsonConvert.DeserializeObject<ParticleDeviceResponse>(responseContent, jsonSerializerSettings);
                SetDeviceState(deviceState);

                if (Cellular)
                    await UpdateMonthlyUssageAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Rename a device
        /// </summary>
        /// <param name="name">New neame for device</param>
        /// <returns>Returns true if device is renamed</returns>
        public async Task<bool> RenameAsync(string name)
        {
            var data = new Dictionary<string, string>
            {
                {"name", name}
            };

            try
            {
                var responseContent = await particleCloud.PutDataAsync($"{ParticleCloud.ParticleApiVersion}/{ParticleCloud.ParticleApiPathDevices}/{Id}", data);
                return await RefreshAsync();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Call a function on a device
        /// </summary>
        /// <param name="function">Function to call</param>
        /// <param name="arg">Arguments to send to function</param>
        /// <returns>Returns a ParticleFunctionResponse</returns>
        public async Task<ParticleFunctionResponse> RunFunctionAsync(string function, string arg)
        {
            if (string.IsNullOrWhiteSpace(function))
                throw new ArgumentNullException(nameof(function));

            var data = new Dictionary<string, string>
            {
                {"arg", arg}
            };

            try
            {
                var responseContent = await particleCloud.PostDataAsync($"{ParticleCloud.ParticleApiVersion}/{ParticleCloud.ParticleApiPathDevices}/{Id}/{function}", data);
                var results = JsonConvert.DeserializeObject<ParticleFunctionResponse>(responseContent);
                return results;
            }
            catch (ParticleUnauthorizedException)
            {
                throw;
            }
            catch (ParticleRequestBadRequestException)
            {
                throw;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Send a signal to the device to shout rainbows
        /// </summary>
        /// <param name="turnSignalOn">Turn signal on</param>
        /// <returns>True on success</returns>
        public async Task<bool> SignalAsync(bool turnSignalOn)
        {
            var data = new Dictionary<string, string>
            {
                {"signal", turnSignalOn ? "1" : "0"}
            };

            try
            {
                var responseContent = await particleCloud.PutDataAsync($"{ParticleCloud.ParticleApiVersion}/{ParticleCloud.ParticleApiPathDevices}/{Id}", data);
                var result = JToken.Parse(responseContent);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Unclaim a device from users account
        /// </summary>
        /// <returns>True if the device is successfully unbclaimed</returns>
        public async Task<bool> UnclaimAsync()
        {
            try
            {
                var responseContent = await particleCloud.DeleteDataAsync($"{ParticleCloud.ParticleApiVersion}/{ParticleCloud.ParticleApiPathDevices}/{Id}");
                var result = JToken.Parse(responseContent);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get the amount of megabytes used in billing period for an Electron
        /// </summary>
        /// <returns>Double value of amount of megabytes used in billing period</returns>
        public async Task<double> UpdateMonthlyUssageAsync()
        {
            if (!Cellular)
                return 0;

            double mbsUsed = 0;

            try
            {
                var particleApiPathSimDataUsage = string.Format(ParticleCloud.ParticleApiPathSimDataUsage, LastICCID);
                var responseContent = await particleCloud.GetDataAsync($"{ParticleCloud.ParticleApiVersion}/{particleApiPathSimDataUsage}");
                var result = JToken.Parse(responseContent);

                foreach (JObject entry in (JArray)result)
                    mbsUsed = Math.Max(mbsUsed, (double)entry["mbs_used_cumulative"]);
            }
            catch
            {
                mbsUsed = -1;
            }

            MbsUsed = mbsUsed;
            return mbsUsed;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Convert Value into known Particle Device Type
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>ParticleDeviceTypealid ParticleDeviceType or Unknown</returns>
        public static ParticleDeviceType IntToParticleDeviceType(int value)
        {
            if (Enum.IsDefined(typeof(ParticleDeviceType), value))
                return (ParticleDeviceType)value;
            else
                return ParticleDeviceType.Unknown;
        }

        public static string ParticleDeviceTypeToString(int value)
        {
            return ParticleDeviceTypeToString((ParticleDeviceType)value);
        }

        public static string ParticleDeviceTypeToString(ParticleDeviceType value)
        {
            switch (value)
            {
                case ParticleDeviceType.ParticleCore:
                    return "Core";
                case ParticleDeviceType.ParticlePhoton:
                    return "Photon";
                case ParticleDeviceType.ParticleP1:
                    return "P1";
                case ParticleDeviceType.ParticleElectron:
                    return "Electron";
                case ParticleDeviceType.DigistumpOak:
                    return "Digistump Oak";
                case ParticleDeviceType.RedBearDuo:
                    return "Red Bear Duo";
                case ParticleDeviceType.BluzDK:
                    return "Bluz DK";
                default:
                    return "Unknown";
            }
        }

        #endregion

        #region Public Event Methods

        /// <summary>
        /// Creates a new long running task to listen for events on this specific device
        /// </summary>
        /// <param name="eventHandler">ParticleEventHandler to call when new event arrives</param>
        /// <param name="eventNamePrefix">Prefix to monitor on event stream</param>
        /// <returns>Returns GUID reference to long running event task</returns>
        public async Task<Guid> SubscribeToDeviceEventsWithPrefixAsync(ParticleEventHandler eventHandler, string eventNamePrefix = "")
        {
            return await particleCloud.SubscribeToDeviceEventsWithPrefixAsync(eventHandler, this, eventNamePrefix);
        }

        /// <summary>
        /// Removes ParticleEventHandler linked to a specified GUID, stoping the handler from receiving events
        /// and if it's the last one shutting down the long running event 
        /// </summary>
        /// <param name="eventListenerID">GUID from a previous call to subscribe to an event</param>
        public void UnsubscribeFromEvent(Guid eventListenerID)
        {
            particleCloud.UnsubscribeFromEvent(eventListenerID);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Start monitoring for an Online event
        /// </summary>
        public async void MonitorForOnlineEvent()
        {
            if (onlineEventListenerID == null)
                onlineEventListenerID = await SubscribeToDeviceEventsWithPrefixAsync(CheckForOnlineEvent, "spark");
        }

        /// <summary>
        /// Update this device with new values from a ParticleDeviceResponse
        /// </summary>
        /// <param name="deviceState">Updated ParticleDeviceResponse</param>
        private void SetDeviceState(ParticleDeviceResponse deviceState)
        {
            bool updateState = false;
            ParticleDeviceResponse oldDeviceState = this.deviceState;
            this.deviceState = deviceState;

            var properties = deviceState.GetType().GetRuntimeProperties();
            foreach (PropertyInfo property in properties)
            {
                bool updateStateField = property.Name == "Connected" || property.Name == "Functions";
                var valueA = property.GetValue(deviceState);
                var valueB = property.GetValue(oldDeviceState);

                if (valueA == null || valueB == null)
                {
                    if (valueA != valueB)
                    {
                        OnPropertyChanged(property.Name);
                        if (updateStateField)
                            updateState = true;
                    }
                }
                else if (!valueA.Equals(valueB))
                {
                    OnPropertyChanged(property.Name);
                    if (updateStateField)
                        updateState = true;
                }
            }

            if (updateState)
                UpdateState();
        }

        /// <summary>
        /// A ParticleEventHandler to look for the specific event that this device is online
        /// </summary>
        /// <param name="sender">Object sending request</param>
        /// <param name="particeEvent">ParticleEventResponse</param>
        private void CheckForOnlineEvent(object sender, ParticleEventResponse particeEvent)
        {
            if (particeEvent.Name.Equals("spark/status") && particeEvent.Data.Equals("online"))
            {
                Guid tempOnlineEventListenerID = onlineEventListenerID.Value;
                onlineEventListenerID = null;

                UnsubscribeFromEvent(tempOnlineEventListenerID);

                particleCloud.SynchronizationContextPost(async a =>
                {
                    IsFlashing = false;
                    UpdateState();
                    await RefreshAsync();
                }, null);
            }
        }

        /// <summary>
        /// Notifies clients that a property value has changed
        /// </summary>
        /// <param name="propertyName">Property name that has changed</param>
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Update the devices state based on flashing, connected and functions
        /// </summary>
        private void UpdateState()
        {
            if (IsFlashing)
            {
                State = ParticleDeviceState.Flashing;
                return;
            }

            if (!Connected)
            {
                State = ParticleDeviceState.Offline;
            }
            else if (Functions == null || Functions.Length < 4)
            {
                State = ParticleDeviceState.Online;
            }
            else
            {
                if (IsRunningTinker())
                    State = ParticleDeviceState.Tinker;
                else
                    State = ParticleDeviceState.Online;
            }
        }

        #endregion
    }

    /// <summary>
    /// Collection class of ParticleDevics
    /// </summary>
    public class ParticleDeviceCollection : List<ParticleDevice>
    {
        public ParticleDeviceCollection()
        {
        }
    }
}
