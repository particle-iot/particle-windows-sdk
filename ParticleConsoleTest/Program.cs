using Particle.SDK;
using Particle.SDK.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            string AuthToken = ConfigurationManager.AppSettings["ParticleAuthToken"];
            string DeviceId = ConfigurationManager.AppSettings["ParticleDeviceId"];
            int ProductId = Convert.ToInt32(ConfigurationManager.AppSettings["ParticleProductId"]);

            await ParticleCloud.SharedCloud.TokenLoginAsync(AuthToken);

            ParticleDevice particleDevice = await ParticleCloud.SharedCloud.GetDeviceAsync(DeviceId);
            ParticleLedgerInstance ledger = await particleDevice.GetLedgerInstance("config", "nimbus");

            //ParticleSimResponse sim =  await particleDevice.GetSimCardAsync();
            //bool active  = await particleDevice.GetSimCardActiveAsync();

            //ParticleDeviceVitalsResponse deviceVitals = await particleDevice.GetLastKnownVitals();
            //List<ParticleDevice> devices = await ParticleCloud.SharedCloud.GetDevicesInProductAsync(ProductId);

            //await ParticleCloud.SharedCloud.RenameDeviceAsync(DeviceId, "I renamed it");
            //await ParticleCloud.SharedCloud.ImportDeviceInProductAsync(ProductId, DeviceId, "mark@lancontrolsystems.com");
        }
    }
}
