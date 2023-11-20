namespace SecurityLock.KeyPair;

public sealed class MemoryBlockList : IBlockList
{
    private ISet<string> _memory;

    private MemoryBlockList(ISet<string> blockList)
    {
        _memory = blockList;
    }

    public bool ContainsKey(string key)
        => _memory.Contains(key);

    public static implicit operator MemoryBlockList(List<string> blockList)
        => new MemoryBlockList(blockList.ToHashSet());
}
