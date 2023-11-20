namespace SecurityLock.KeyPair;

public sealed class KeysConcatenatorDecorator : ILock
{
    private ILock _lock;

    public KeysConcatenatorDecorator(ILock l)
    {
        _lock = l;
    }

    public LockResponse TryUnlock((string keyA, string keyB) keys)
    {
        var (keyA, keyB) = keys;
        keys = (keyA + keyB, keyB + keyA);
        return _lock.TryUnlock(keys);
    }
}
