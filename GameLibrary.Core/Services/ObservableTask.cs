namespace GameLibrary.Core.Services;

public abstract class ObservableTask<TStatus>: IObservable<TStatus>
{
    protected abstract TStatus Status { get; }
    public abstract Task Run();
    private readonly HashSet<IObserver<TStatus>> _observers = new();

    public IDisposable Subscribe(IObserver<TStatus> observer)
    {
        if (_observers.Add(observer))
        {
            observer.OnNext(Status);
        }
        return new TaskUnsubscriber<TStatus>(this, observer);
    }

    protected void NotifyStatus()
    {
        foreach (var observer in _observers)
        {
            observer.OnNext(Status);
        }
    }

    protected void NotifyCompleted()
    {
        foreach (var observer in _observers)
        {
            observer.OnCompleted();
        }
    }

    public void Unsubscribe(IObserver<TStatus> observer)
    {
        _observers.Remove(observer);
    }
}

public sealed class TaskUnsubscriber<TStatus>(ObservableTask<TStatus> task, IObserver<TStatus> observer) : IDisposable
{
    public void Dispose()
    {
        task.Unsubscribe(observer);
    }
}
