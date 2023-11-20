using SecurityLock.KeyPair;

namespace SecurityLock.KeyPair.Tests;

public class CombinationLimiterTests
{
    private Mock<ILinkedKeysRepository> _repo;
    private CombinationLimiter _sut;

    public CombinationLimiterTests()
    {
        _repo = new Mock<ILinkedKeysRepository>();

        _sut = new CombinationLimiter(_repo.Object, limit: 1, expiresIn: TimeSpan.FromDays(1));
    }

    [Theory(DisplayName = "Should unlock if keyB it's already linked keyA")]
    [InlineData("keyA", "keyB")]
    public void Test1(string keyA, string keyB)
    {
        _repo.Setup(r => r.GetLinkedKeys(It.IsAny<string>())).Returns(new HashSet<string>() { keyB });
        
        var result = _sut.TryUnlock((keyA, keyB));

        var expected = LockResponse.Unlocked();
        Assert.Equal(expected.IsUnlocked, result.IsUnlocked);
    }

    [Theory(DisplayName = "Should unlock and link keyB to keyA if limit not exceeded")]
    [InlineData("keyA", "keyB")]
    public void Test2(string keyA, string keyB)
    {
        _repo.Setup(r => r.GetLinkedKeys(It.IsAny<string>())).Returns(new HashSet<string>());
        
        var result = _sut.TryUnlock((keyA, keyB));

        var expected = LockResponse.Unlocked();
        Assert.Equal(expected.IsUnlocked, result.IsUnlocked);
        _repo.Verify(r => r.LinkKeys(keyA, keyB, It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact(DisplayName = "Should lock if keyB it's not linked keyA and linked limit was exceeded")]
    public void Test3()
    {
        _repo.Setup(r => r.GetLinkedKeys(It.IsAny<string>())).Returns(new HashSet<string>() { "a", "b" });
        
        var result = _sut.TryUnlock((It.IsAny<(string keyA, string keyB)>()));

        var expected = LockResponse.Locked(It.IsAny<string>());
        Assert.Equal(expected.IsUnlocked, result.IsUnlocked);
    }
}
