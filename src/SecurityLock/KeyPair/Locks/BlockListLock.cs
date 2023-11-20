namespace SecurityLock.KeyPair;

public sealed class BlockListLock
{
    private KeyListReader _keyListReader;

    public BlockListLock(KeyListReader blockList)
    {
        _keyListReader = blockList;
    }

    public async Task<LockResponse> TryUnlock((string partA, string partB) key)
    {
        var (firstPart, _) = key;
        var containsKey = await _keyListReader.ListContains(firstPart);

        if (containsKey)
        {
            return LockResponse.Locked($"{firstPart} is blocked");
        }

        return LockResponse.Unlocked();
    }
}
