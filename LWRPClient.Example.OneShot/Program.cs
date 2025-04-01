using System;
using System.Net;
using System.Threading.Tasks;

namespace LWRPClient.Example.OneShot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await SetAsync(IPAddress.Parse("192.168.1.212"), TimeSpan.FromSeconds(5));
        }

        static async Task SetAsync(IPAddress addr, TimeSpan timeout)
        {
            using (LWRPConnection conn = new LWRPConnection(addr, LWRPEnabledFeature.SOURCES | LWRPEnabledFeature.DESTINATIONS))
            {
                //Initialize
                conn.Initialize();

                //Wait for ready signal
                await conn.WaitForReadyAsync(timeout);

                //Do changes
                Console.WriteLine($"Old Src 1: {conn.Sources[0].PrimarySourceName}");
                conn.Sources[0].PrimarySourceName = $"TEST-{DateTime.UtcNow.Minute:00}-{DateTime.UtcNow.Second:00}";

                //Apply
                await conn.SendUpdatesAsync(timeout);
            }
        }
    }
}
