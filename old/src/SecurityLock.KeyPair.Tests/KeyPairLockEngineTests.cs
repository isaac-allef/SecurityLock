using SecurityLock.KeyPair;
using static SecurityLock.KeyPair.KeyPairLockEngine;

namespace SecurityLock.KeyPair.Tests;

public class KeyPairLockEngineTests
{
    private Mock<ILock> _l1;
    private Mock<ILock> _l2;
    private Mock<ILock> _l3;
    private KeyPairLockEngine _sut;

    public KeyPairLockEngineTests()
    {
        _l1 = new Mock<ILock>();
        _l2 = new Mock<ILock>();
        _l3 = new Mock<ILock>();

        _sut = new KeyPairLockEngineBuilder("testContext")
            .AddLock(_l1.Object)
            .AddLock(_l2.Object)
            .AddLock(_l3.Object)
            .Build();
    }

    [Fact(DisplayName = "Should run all locks adds if all unlock")]
    public void Test1()
    {
        _l1.Setup(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()))
            .Returns(LockResponse.Unlocked());
            
        _l2.Setup(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()))
            .Returns(LockResponse.Unlocked());

        _l3.Setup(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()))
            .Returns(LockResponse.Unlocked());
        
        var result = _sut.TryUnlock(("A", "B"));

        _l1.Verify(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()), Times.Once);
        _l2.Verify(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()), Times.Once);
        _l3.Verify(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()), Times.Once);
    }

    [Fact(DisplayName = "Should break locks runing if 1 lock")]
    public void Test2()
    {
        _l1.Setup(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()))
            .Returns(LockResponse.Unlocked());

        _l2.Setup(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()))
            .Returns(LockResponse.Locked(It.IsAny<string>()));

        _l3.Setup(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()))
            .Returns(LockResponse.Unlocked());
        
        var result = _sut.TryUnlock(("A", "B"));

        _l1.Verify(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()), Times.Once);
        _l2.Verify(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()), Times.Once);
        _l3.Verify(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()), Times.Never);
    }

    [Fact(DisplayName = "Should throws locks runing if 1 lock")]
    public void Test3()
    {
        _l1.Setup(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()))
            .Returns(LockResponse.Unlocked());

        _l2.Setup(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()))
            .Returns(LockResponse.Locked(It.IsAny<string>()));

        _l3.Setup(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()))
            .Returns(LockResponse.Unlocked());
        
        Assert.Throws<LockException>(() => _sut.TryUnlockAndThrows(("A", "B")));

        _l1.Verify(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()), Times.Once);
        _l2.Verify(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()), Times.Once);
        _l3.Verify(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()), Times.Never);
    }

    [Fact(DisplayName = "Should notify locks runing if 1 lock")]
    public void Test4()
    {
        _l1.Setup(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()))
            .Returns(LockResponse.Unlocked());

        _l2.Setup(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()))
            .Returns(LockResponse.Locked(It.IsAny<string>()));

        _l3.Setup(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()))
            .Returns(LockResponse.Unlocked());

        var result = _sut.TryUnlockAndNotify(("A", "B"));

        _l1.Verify(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()), Times.Once);
        _l2.Verify(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()), Times.Once);
        _l3.Verify(l => l.TryUnlock(It.IsAny<(string keyA, string keyB)>()), Times.Once);

        var expected = new LockNotification();
        expected.AddError(It.IsAny<string>());
        
        Assert.Equal(expected.Errors, result.Errors);
    }
}
