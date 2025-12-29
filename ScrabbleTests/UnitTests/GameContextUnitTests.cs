using System;

namespace ScrabbleTests.UnitTests;

public class GameContextUnitTests
{
    [Fact]
    public void CheckIsFinalMove()
    {
        var gameContext = new GameContext();
        Assert.False(gameContext.GetIsFinalMove());
        gameContext.SetFinalMove();
        Assert.True(gameContext.GetIsFinalMove());
    }

    [Fact]
    public void CheckGameState()
    {
        var gameContext = new GameContext();
        Assert.False(gameContext.GetRestartGame());
        Assert.False(gameContext.GetShutdownGame());
        Assert.True(gameContext.GetContinue());

        gameContext.SetRestartGame();
        Assert.True(gameContext.GetRestartGame());
        Assert.False(gameContext.GetShutdownGame());
        Assert.False(gameContext.GetContinue());

        gameContext.SetShutdownGame();
        Assert.False(gameContext.GetRestartGame());
        Assert.True(gameContext.GetShutdownGame());
        Assert.False(gameContext.GetContinue());
    }
}
