namespace SecurityLock.KeyPair;

public sealed class KeysInverterDecorator
{
    private KeyPairLockEngine.UnLocking _lock;

    public KeysInverterDecorator(KeyPairLockEngine.UnLocking l)
    {
        _lock = l;
    }

    public async Task<LockResponse> TryUnlock((string partA, string partB) key)
    {
        var (partA, partB) = key;
        key = (partB, partA);
        return await _lock(key);
    }
}
