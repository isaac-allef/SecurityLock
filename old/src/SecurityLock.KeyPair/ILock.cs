namespace SecurityLock.KeyPair;

public interface ILock
{
    public LockResponse TryUnlock((string keyA, string keyB) keys);
}
