namespace SecurityLock;

public abstract class LockEngine<T>
{
    public string Context { get; private set; }
    public bool IsDisabled { get; private set; } = false;
    public delegate Task<LockResponse> UnLocking(T key);


    protected IList<UnLocking> _unlocks;

    protected internal LockEngine(string context)
    {
        Context = context;
        _unlocks = new List<UnLocking>();
    }

    public void Disable()
        => IsDisabled = true;
    
    public void Enable()
        => IsDisabled = false;
    

    internal void AddLock(UnLocking unlock)
        => _unlocks.Add(unlock);


    public async Task<LockResponse> TryUnlock(T key)
        => await RunUnlock(_unlocks, key);

    public async void TryUnlockAndThrows(T key)
        => await RunUnlock(_unlocks, key, (response) => throw new LockException(response.Message));

    public async Task<LockNotification> TryUnlockAndNotify(T key)
    {
        var notification = new LockNotification();
        await RunUnlock(_unlocks, key, notification);
        return notification;
    }


    protected private async Task<LockResponse> RunUnlock(IList<UnLocking> unLockingList, T key, LockNotification notification)
    => await RunUnlock(unLockingList, key, (response) => notification.AddError(response.Message));

    protected private async Task<LockResponse> RunUnlock(IList<UnLocking> unLockingList, T key, Action<LockResponse>? execute = null)
    {
        if (unLockingList.Count.Equals(0)) throw new Exception("Not implemented");
        if (IsDisabled) return LockResponse.Unlocked();

        foreach(var unLocking in unLockingList)
        {
            var response = await unLocking(key);
            if (response.IsUnlocked.Equals(false))
            {
                if (execute is not null) execute(response);
                else return response;
            }
        }

        return LockResponse.Unlocked();
    }
}
