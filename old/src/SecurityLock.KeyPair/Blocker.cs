namespace SecurityLock.KeyPair;

public sealed class Blocker : ILock
{
    private IBlockList _blockList;

    public Blocker(IBlockList blockList)
    {
        _blockList = blockList;
    }

    public LockResponse TryUnlock((string keyA, string keyB) keys)
    {
        var (keyA, _) = keys;

        var containsKey = _blockList.ContainsKey(keyA);

        if (containsKey)
        {
            return LockResponse.Locked($"{keyA} is blocked");
        }

        return LockResponse.Unlocked();
    }
}
