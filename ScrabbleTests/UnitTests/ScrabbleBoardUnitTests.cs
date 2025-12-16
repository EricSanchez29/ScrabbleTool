namespace ScrabbleTests;

public class ScrabbleBoardUnitTests
{
    [Fact]
    public void Test()
    {

    }

   
    [Fact]
    public void TestDrawTiles()
    {


    }

    [Fact]
    public void TestGetXYCoordinate()
    {
        // setup
        var gen = new ScrabbleWordGenerator(GlobalTestVariables.DictionaryLocation);
        var board = new ScrabbleBoard(gen);

        // A1
        var result = board.TryGetXYCoordinate("A1", out int x, out int y);
        Assert.True(result);
        Assert.True(x == 0);
        Assert.True(y == 0);

        // A15
        var result1 = board.TryGetXYCoordinate("A15", out int x1, out int y1);
        Assert.True(result1);
        Assert.True(x1 == 0);
        Assert.True(y1 == 14);

        // O1
        var result2 = board.TryGetXYCoordinate("O1", out int x2, out int y2);
        Assert.True(result2);
        Assert.True(x2 == 14);
        Assert.True(y2 == 0);

        // O15
        var result3 = board.TryGetXYCoordinate("O15", out int x3, out int y3);
        Assert.True(result3);
        Assert.True(x3 == 14);
        Assert.True(y3 == 14);

        // H*
        var result4 = board.TryGetXYCoordinate("H*", out int x4, out int y4);
        Assert.False(result4);

        // H19
        var result5 = board.TryGetXYCoordinate("H19", out int x5, out int y5);
        Assert.False(result5);

        // Y10
        var result6 = board.TryGetXYCoordinate("H*", out int x6, out int y6);
        Assert.False(result6);
    }

    [Fact]
    public void TestIsSpecialTile()
    {

    }

    [Fact]
    public void TestisValidTile()
    {
        

    }

    

    [Fact]
    public void TestAddWord()
    {
    // - will have to prepopulate board 
    }


}
