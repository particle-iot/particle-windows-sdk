using Particle.SDK;
using Particle.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParticleConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task task = TestVitals();
            task.Wait();
        }

        static async Task TestVitals()
        {
            const string AuthToken = "AuthTokenHere";
            const string DeviceId = "DeviceIDHere";

            await ParticleCloud.SharedCloud.TokenLoginAsync(AuthToken);

            ParticleDevice particleDevice = await ParticleCloud.SharedCloud.GetDeviceAsync(DeviceId);
            ParticleDeviceVitalsResponse deviceVitals = await particleDevice.GetLastKnownVitals();
        }
    }
}
