namespace Particle.SDK.Models
{
    public delegate void ParticleEventHandler(object sender, ParticleEventResponse particeEvent);

    /// <summary>
    /// Class of relative path and ParticleEventHandler for tracking handlers for events
    /// </summary>
    public class ParticleEventHandlerData
    {
        public string Path { get; internal set; }
        public ParticleEventHandler EventHandler { get; internal set; }

        public ParticleEventHandlerData(string path, ParticleEventHandler eventHandler)
        {
            Path = path;
            EventHandler = eventHandler;
        }
    }
}
