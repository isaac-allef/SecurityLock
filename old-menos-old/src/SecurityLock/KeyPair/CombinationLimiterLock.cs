namespace SecurityLock.KeyPair;

public sealed class CombinationLimiterLock : ILock<(string partA, string partB)>
{
    private CombinationLimiter _combinationLimiter;
    private int _limit;
    private TimeSpan _expiresIn;

    public CombinationLimiterLock(CombinationLimiter combinationLimiter, int limit, TimeSpan expiresIn)
    {
        _combinationLimiter = combinationLimiter;
        _limit = limit;
        _expiresIn = expiresIn;
    }

    public LockResponse TryUnlock((string partA, string partB) key)
    {
        var (partA, partB) = key;
        var linkedKyes = _combinationLimiter.GetLinkedKeys(partA);

        if (linkedKyes.Contains(partB))
        {
            return LockResponse.Unlocked();
        }
        
        else if (linkedKyes.Count < _limit)
        {
            _combinationLimiter.LinkKeys(partA, partB, _expiresIn);
            return LockResponse.Unlocked();
        }

        else
        {
            return LockResponse.Locked($"{partA} has already reached his combination limit");
        }
    }
}
