namespace SecurityLock.KeyPair;

public sealed class MemoryListReader : KeyListReader
{
    private ISet<string> _list;

    private MemoryListReader(ISet<string> list)
    {
        _list = list;
    }

    public override async Task<bool> ListContains(string key)
    {
        await Task.CompletedTask;
        return _list.Contains(key);
    }

    public static implicit operator MemoryListReader(List<string> list)
        => new MemoryListReader(list.ToHashSet());
}
