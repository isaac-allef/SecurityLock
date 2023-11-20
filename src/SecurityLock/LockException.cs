namespace SecurityLock;

public class LockException : ApplicationException
{
    public LockException(string message) : base(message)
    {
    }
}
