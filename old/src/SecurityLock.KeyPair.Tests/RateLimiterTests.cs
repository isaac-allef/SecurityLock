using SecurityLock.KeyPair;

namespace SecurityLock.KeyPair.Tests;

public class RateLimiterTests
{
    private Mock<IKeysBucketLimitRepository> _repo;
    private RateLimiter _sut;

    public RateLimiterTests()
    {
        _repo = new Mock<IKeysBucketLimitRepository>();

        _sut = new RateLimiter(_repo.Object, limit: 1, period: TimeSpan.FromDays(1));
    }

    [Fact(DisplayName = "Should unlock if limit was greater zero")]
    public void Test1()
    {
        _repo.Setup(r => r.ConsumeBucketLimit(It.IsAny<string>())).Returns(2);
        
        var result = _sut.TryUnlock(It.IsAny<(string keyA, string keyB)>());

        var expected = LockResponse.Unlocked();
        Assert.Equal(expected.IsUnlocked, result.IsUnlocked);
    }

    [Fact(DisplayName = "Should unlock and set bucket limit if limit was null")]
    public void Test2()
    {
        _repo.Setup(r => r.ConsumeBucketLimit(It.IsAny<string>())).Returns(() => null);
        
        var result = _sut.TryUnlock(It.IsAny<(string keyA, string keyB)>());

        var expected = LockResponse.Unlocked();
        Assert.Equal(expected.IsUnlocked, result.IsUnlocked);
        _repo.Verify(r => r.SetBucketLimit(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact(DisplayName = "Should lock if limit not null and limit not greater zero")]
    public void Test3()
    {
        _repo.Setup(r => r.ConsumeBucketLimit(It.IsAny<string>())).Returns(0);
        
        var result = _sut.TryUnlock(It.IsAny<(string keyA, string keyB)>());

        var expected = LockResponse.Locked(It.IsAny<string>());
        Assert.Equal(expected.IsUnlocked, result.IsUnlocked);
    }
}
