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
            const string AuthToken = "AuthToken";
            const string DeviceId = "DeviceID";
            int productId = 0;

            await ParticleCloud.SharedCloud.TokenLoginAsync(AuthToken);

            //ParticleDevice particleDevice = await ParticleCloud.SharedCloud.GetDeviceAsync(DeviceId);
            //ParticleDeviceVitalsResponse deviceVitals = await particleDevice.GetLastKnownVitals();
            //List<ParticleDevice> devices = await ParticleCloud.SharedCloud.GetDevicesInProductAsync(productId);
            await ParticleCloud.SharedCloud.RenameDeviceAsync(DeviceId, "I renamed it");
            //await ParticleCloud.SharedCloud.ImportDeviceInProductAsync(productId, DeviceId, "mark@lancontrolsystems.com");
        }
    }
}
