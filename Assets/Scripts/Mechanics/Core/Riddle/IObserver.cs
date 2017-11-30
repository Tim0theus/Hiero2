public interface IObserver {
    void UpdateStatus(IObservable observable);
    void Add(IObservable observable);
    void Remove(IObservable observable);
}
