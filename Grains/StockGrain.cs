using GrainInterfaces.Interfaces;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Streams;
using States.Models;

namespace Grains
{
    [StorageProvider(ProviderName = "stock")]
    public class StockGrain : Grain<StockState>, IStockGrain
    {
        private IAsyncStream<StockState> iAsyncStream { get; set; }
        Guid streamIdentification { get; }

        public StockGrain() 
        {
            streamIdentification = Guid.NewGuid();
        }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            State.Symbol = this.GetPrimaryKeyString();

            IStreamProvider streamProvider = this.GetStreamProvider("StreamProvider");
            StreamId streamId = StreamId.Create("stock", streamIdentification);
            iAsyncStream = streamProvider.GetStream<StockState>(streamId);

            return base.OnActivateAsync(cancellationToken);
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            return base.OnDeactivateAsync(reason, cancellationToken);
        }

        public Task<StockState> Get()
        {
            return Task.FromResult(State);
        }

        public Task<Guid> GetStreamId()
        {
            return Task.FromResult(streamIdentification);
        }

        public Task Set(StockState StockState)
        {
            State = StockState;

            iAsyncStream.OnNextAsync(StockState);

            return Task.CompletedTask;
        }
    }
}
