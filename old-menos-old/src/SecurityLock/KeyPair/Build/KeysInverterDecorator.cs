namespace SecurityLock.KeyPair;

public sealed class KeysInverterDecorator : ILock<(string partA, string partB)>
{
    private ILock<(string partA, string partB)> _lock;

    public KeysInverterDecorator(ILock<(string partA, string partB)> l)
    {
        _lock = l;
    }

    public LockResponse TryUnlock((string partA, string partB) key)
    {
        var (partA, partB) = key;
        key = (partB, partA);
        return _lock.TryUnlock(key);
    }
}
