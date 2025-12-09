using System;

namespace ScrabbleTests.UnitTests;

public class HumanPlayerUnitTests
{
    [Theory]
    [InlineData('*')]
    public void TestMakeMove(char laneChar)
    {
        char[] playerTil = new char[7];

        //var move = generator.GetPossibleScrabbleWords(playerTile, laneChar);

    }

    [Fact]
    public void DrawInitialTiles()
    {

    }

    [Theory]
    [InlineData("")]
    public void MakeMove_GameContext(string gameContext)
    {

    }

    [Fact]
    public void makeStartingMove()
    {

    }
}
