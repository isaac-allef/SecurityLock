namespace SecurityLock.KeyPair;

public sealed class CombinationLimiter : ILock
{
    private ILinkedKeysRepository _repo;
    private int _limit;
    private TimeSpan _expiresIn;

    public CombinationLimiter(ILinkedKeysRepository repo, int limit, TimeSpan expiresIn)
    {
        _repo = repo;
        _limit = limit;
        _expiresIn = expiresIn;
    }

    public LockResponse TryUnlock((string keyA, string keyB) keys)
    {
        var (keyA, keyB) = keys;
        var linkedKeys = _repo.GetLinkedKeys(keyA);

        if (linkedKeys.Contains(keyB))
        {
            return LockResponse.Unlocked();
        }
        
        else if (linkedKeys.Count < _limit)
        {
            _repo.LinkKeys(keyA, keyB, _expiresIn);
            return LockResponse.Unlocked();
        }

        else
        {
            return LockResponse.Locked($"{keyA} has already reached his combination limit");
        }
    }
}
