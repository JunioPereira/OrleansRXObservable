using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClientOrleans
{
    public static class OrleansConnection
    {
        public static async Task<IHost> StartOrleansClient()
        {
            var client = new HostBuilder()
                .UseOrleansClient(c =>
                {
                    c.UseLocalhostClustering();
                })
                .ConfigureLogging(l =>
                {
                    l.SetMinimumLevel(LogLevel.Warning).AddConsole();
                });

            var host = client.Build();
            await host.StartAsync();
            return host;
        }
    }
}
