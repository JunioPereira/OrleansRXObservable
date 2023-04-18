using GrainInterfaces.Interfaces;
using Orleans.Providers;
using OrleansService.Services;
using States.Models;
using System.Collections.Generic;

namespace Grains
{
    [StorageProvider(ProviderName = "customer")]
    public class CustomerGrain : Grain<CustomerState>, ICustomerGrain
    {
        IStockService iStockService { get; }

        Dictionary<string, Func<StockState, Task>> SubscribeList { get; }

        public CustomerGrain(IStockService _iStockService) 
        {
            iStockService = _iStockService;
            SubscribeList = new Dictionary<string, Func<StockState, Task>>();
        }

        public Task<CustomerState> Get()
        {
            return Task.FromResult(State);
        }

        public async Task Set(CustomerState customer)
        {
            State = customer;

            foreach (var item in State.Stocks)
            {
                if (!SubscribeList.ContainsKey(item.Key))
                {
                    SubscribeList[item.Key] = (item) =>
                    {
                        ArrivedStock(item);
                        return Task.CompletedTask;
                    };

                    await iStockService.Subscribe(item.Key, SubscribeList[item.Key]);
                }
            }

            var list = new  List<string>();

            foreach (var item in SubscribeList)
            {
                if (!State.Stocks.ContainsKey(item.Key))
                {
                    await iStockService.Unsubscribe(item.Key, SubscribeList[item.Key]);
                    list.Add(item.Key);
                }
            }

            foreach (var item in list)
            {
                SubscribeList.Remove(item);
            }
        }

        private void ArrivedStock(StockState item) 
        {
            State.Stocks[item.Symbol] = item;
        }
    }
}
