using States.Models;

namespace GrainInterfaces.Interfaces
{
    public interface IStockGrain : IGrainWithStringKey
    {
        Task<StockState> Get();
        Task Set(StockState StockState);
        Task<Guid> GetStreamId();
    }
}
