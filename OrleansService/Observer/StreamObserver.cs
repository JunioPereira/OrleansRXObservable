using Orleans.Streams;

namespace OrleansService.Observer
{
    public class StreamObserver<T> : IAsyncObserver<T>, IDisposable
    {
        Func<StreamObserver<T>, Task> FuncReconnect { get; }
        Func<T, Task> FuncNext { get; }
        Func<Exception, Task> FuncOnError { get; }

        public StreamObserver(Func<StreamObserver<T>, Task> funcReconnect, Func<T, Task> funcNext)
            : this(funcReconnect, funcNext, null)
        { }

        public StreamObserver(Func<StreamObserver<T>, Task> funcReconnect, Func<T, Task> funcNext, Func<Exception, Task> funcOnError)
        {
            FuncReconnect = funcReconnect;
            FuncNext = funcNext;
            FuncOnError = funcOnError;
        }

        public void Dispose()
        {
        }

        public Task OnCompletedAsync()
        {
            return FuncReconnect?.Invoke(this);
        }

        public Task OnErrorAsync(Exception ex)
        {
            return FuncOnError?.Invoke(ex);
        }

        public Task OnNextAsync(T item, StreamSequenceToken? token = null)
        {
            return FuncNext?.Invoke(item);
        }
    }
}
