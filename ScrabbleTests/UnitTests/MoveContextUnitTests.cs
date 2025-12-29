
namespace ScrabbleTests.UnitTests;

public class MoveContextUnitTests
{
    [Fact]
    public void ToggleRetryState()
    {
        var moveContext = new MoveContext(true);

        Assert.True(moveContext.GetRetryMove());

        moveContext.SetRetryMove(false);

        Assert.False(moveContext.GetRetryMove());
    }

    [Fact]
    public void SanityCheck()
    {
        var moveContext = new MoveContext(false);

        Assert.False(moveContext.GetRetryMove());
        Assert.False(moveContext.GetPassMove());
        Assert.False(moveContext.GetSwapTiles());

        moveContext.SetPassMove();
        Assert.False(moveContext.GetRetryMove());
        Assert.True(moveContext.GetPassMove());
        Assert.False(moveContext.GetSwapTiles());

        moveContext.SetSwapTiles();
        Assert.False(moveContext.GetRetryMove());
        Assert.False(moveContext.GetPassMove());
        Assert.True(moveContext.GetSwapTiles());
    }
}