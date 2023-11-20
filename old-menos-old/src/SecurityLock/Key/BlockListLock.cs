namespace SecurityLock.Key;

public sealed class BlockListLock : ILock<string>
{
    private BlockList _blockList;

    public BlockListLock(BlockList blockList)
    {
        _blockList = blockList;
    }

    public LockResponse TryUnlock(string key)
    {
        var containsKey = _blockList.ContainsKey(key);

        if (containsKey)
        {
            return LockResponse.Locked($"{key} is blocked");
        }

        return LockResponse.Unlocked();
    }
}
