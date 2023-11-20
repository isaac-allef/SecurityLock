using SecurityLock.KeyPair;

namespace SecurityLock.KeyPair.Tests;

public class BlockerTests
{
    private Mock<IBlockList> _blockList;
    private Blocker _sut;

    public BlockerTests()
    {
        _blockList = new Mock<IBlockList>();

        _sut = new Blocker(_blockList.Object);
    }

    [Fact(DisplayName = "Should lock if keyA is into block list")]
    public void Test1()
    {
        _blockList.Setup(r => r.ContainsKey(It.IsAny<string>())).Returns(true);
        
        var result = _sut.TryUnlock(It.IsAny<(string keyA, string keyB)>());

        var expected = LockResponse.Locked(It.IsAny<string>());
        Assert.Equal(expected.IsUnlocked, result.IsUnlocked);
    }

    [Fact(DisplayName = "Should unlock if keyA is not into block list")]
    public void Test2()
    {
        _blockList.Setup(r => r.ContainsKey(It.IsAny<string>())).Returns(false);
        
        var result = _sut.TryUnlock(It.IsAny<(string keyA, string keyB)>());

        var expected = LockResponse.Unlocked();
        Assert.Equal(expected.IsUnlocked, result.IsUnlocked);
    }
}
