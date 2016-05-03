using Particle.SDK.Models;

namespace Particle.SDK
{
    /// <summary>
    /// Class of ParticleEventHandler for a specific relative path for events
    /// </summary>
    public class ParticleEventGroup
    {
        /// <summary>
        /// Global event called by ParticleEventGroup whenever a new message arrives
        /// </summary>
        public event ParticleEventHandler OnMessage;

        /// <summary>
        /// Whether or not this group has any handlers
        /// </summary>
        public bool HasHandlers
        {
            get { return OnMessage != null; }
        }

        /// <summary>
        /// Add new ParticleEventHandler
        /// </summary>
        /// <param name="eventHandler">ParticleEventHandler to add</param>
        public void AddHandler(ParticleEventHandler eventHandler)
        {
            OnMessage += eventHandler;
        }

        /// <summary>
        /// New message for this group of event handlers
        /// </summary>
        /// <param name="sender">Object sending request</param>
        /// <param name="particeEvent">ParticleEventResponse</param>
        public void NewMessage(object sender, ParticleEventResponse particeEvent)
        {
            try {
                if (HasHandlers)
                    OnMessage(sender, particeEvent);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Remove ParticleEventHandler
        /// </summary>
        /// <param name="eventHandler">ParticleEventHandler to remove</param>
        public void RemoveHandler(ParticleEventHandler eventHandler)
        {
            OnMessage -= eventHandler;
        }
    }
}
