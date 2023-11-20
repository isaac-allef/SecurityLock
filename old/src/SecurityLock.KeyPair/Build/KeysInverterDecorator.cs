namespace SecurityLock.KeyPair;

public sealed class KeysInverterDecorator : ILock
{
    private ILock _lock;

    public KeysInverterDecorator(ILock l)
    {
        _lock = l;
    }

    public LockResponse TryUnlock((string keyA, string keyB) keys)
    {
        var (keyA, keyB) = keys;
        keys = (keyB, keyA);
        return _lock.TryUnlock(keys);
    }
}
