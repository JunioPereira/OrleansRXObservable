using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using OrleansService.Services;

namespace SiloHost
{
    public class SiloHostService : IHostedService
    {
        IHost? SiloHost { get; set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return StartSilo(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task StartSilo(CancellationToken cancellationToken)
        {
            SiloHost = new HostBuilder()
                .UseOrleans(o =>
                {
                    o.UseLocalhostClustering();

                    o.AddMemoryGrainStorage("customer");
                    o.AddMemoryGrainStorage("stock");

                    o.AddMemoryStreams("StreamProvider");
                    o.AddMemoryGrainStorage("PubSubStore");

                    //o.AddBroadcastChannel(
                    //            "stock",
                    //            options => options.FireAndForgetDelivery = true);

                    o.UseDashboard(d => {
                        d.Port = 9091;
                        d.Username = "admin";
                        d.Password = "admin";
                    });

                    


                })
                .ConfigureLogging(l => {
                    l.SetMinimumLevel(LogLevel.Warning).AddConsole();
                })
                //.ConfigureServices(service => { service.AddHostedService<StockWorker>(); })
                .ConfigureServices(service => 
                { 
                    //service.AddHostedService<StockWorker>();
                    service.AddSingleton<IStockService, StockService>();
                })
                .Build();

            await SiloHost.StartAsync(cancellationToken);
        }
    }
}
