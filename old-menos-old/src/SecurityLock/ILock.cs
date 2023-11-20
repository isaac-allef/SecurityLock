namespace SecurityLock;

public interface ILock<T>
{
    public LockResponse TryUnlock(T key);
}
