using GrainInterfaces.Interfaces;
using Orleans.Runtime;
using Orleans.Streams;
using OrleansService.Observer;
using States.Models;
using System.Collections.Concurrent;

namespace OrleansService.Services
{
    public interface IStockService
    {
        Task Subscribe(string symbol, Func<StockState, Task> action);

        Task Unsubscribe(string symbol, Func<StockState, Task> action);

        Task<StockState> Get(string symbol);
    }

    public class StockService : IStockService
    {
        IClusterClient iClusterClient { get; }
        ConcurrentDictionary<string, StockState> StockDictionary { get; }
        ConcurrentDictionary<string, Func<StockState, Task>> FuncDictionary { get; }
        ConcurrentDictionary<string, StreamSubscriptionHandle<StockState>> SubscriptionDictionary { get; }

        public StockService(IClusterClient clusterClient) 
        {
            iClusterClient = clusterClient;
            StockDictionary = new ConcurrentDictionary<string, StockState>();
            FuncDictionary = new ConcurrentDictionary<string, Func<StockState, Task>>();
            SubscriptionDictionary = new ConcurrentDictionary<string, StreamSubscriptionHandle<StockState>>();
        }

        public Task<StockState> Get(string symbol)
        {
            if (StockDictionary.ContainsKey(symbol))
            {
                return Task.FromResult(StockDictionary[symbol]);
            }
            else 
            {
                var igrain = iClusterClient.GetGrain<IStockGrain>(symbol);
                return igrain.Get();
            }
        }

        public async Task Subscribe(string symbol, Func<StockState, Task> action)
        {
            if (FuncDictionary.ContainsKey(symbol))
            {
                FuncDictionary[symbol] += action;
            }
            else
            {
                FuncDictionary[symbol] = action;

                var igrain = iClusterClient.GetGrain<IStockGrain>(symbol);

                var idStream = await igrain.GetStreamId();

                var streamProvider = iClusterClient.GetStreamProvider("StreamProvider");
                StreamId streamId = StreamId.Create("stock", idStream);
                var iAsyncStream = streamProvider.GetStream<StockState>(streamId);

                StreamObserver<StockState> obeserver = new StreamObserver<StockState>(
                    (obeserver) =>
                    {
                        StockDictionary.TryRemove(symbol, out StockState state);
                        return Task.CompletedTask;
                    },
                    (item) =>
                    {
                        //TODO colocar aqui a chamada para a chegada do stream

                        if (FuncDictionary.ContainsKey(symbol))
                        {
                            StockDictionary[item.Symbol] = item;

                            return FuncDictionary[symbol]?.Invoke(item);
                        }

                        return Task.CompletedTask;
                    },
                    (onError) =>
                    {
                        //TODO colocar aqui a chamada quando der erro no stream
                        StockDictionary.TryRemove(symbol, out StockState state);
                        return Task.CompletedTask;
                    }
                );

                SubscriptionDictionary[symbol] = await iAsyncStream.SubscribeAsync(obeserver);
            }
        }

        public async Task Unsubscribe(string symbol, Func<StockState, Task> action)
        {
            if (FuncDictionary.ContainsKey(symbol))
            {
                FuncDictionary[symbol] -= action;

                var func = FuncDictionary[symbol];

                if (func is null)
                {
                    if (SubscriptionDictionary.TryRemove(symbol, out var _sub))
                    {
                        await _sub.UnsubscribeAsync();
                    }

                    FuncDictionary.TryRemove(symbol, out var _);
                    StockDictionary.TryRemove(symbol, out var _);
                }
            }
        }
    }
}
