using Microsoft.Extensions.Caching.Memory;

namespace SecurityLock.KeyPair;

public sealed class MemoryKeyValueAccess<T> : KeyValueAccess<T>
{
    private IMemoryCache _memory;

    public MemoryKeyValueAccess(string context) : base(context)
    {
        _memory = new MemoryCache(new MemoryCacheOptions());
    }

    public override async Task<T?> Get(string key)
    {
        var value = _memory.Get<T?>(AddContext(key));
        await Task.CompletedTask;
        return value;
    }

    public override async Task Set(string key, T value, TimeSpan expiresIn)
    {
        _memory.Set<T>(AddContext(key), value, expiresIn);
        await Task.CompletedTask;
    }
}
