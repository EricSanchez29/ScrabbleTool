public static class Game
{
    public static bool StartGame(string dictionaryLocation)
    {
        ScrabbleBoard scrabbleBoard = new ScrabbleBoard();
        scrabbleBoard.DisplayBoard();

        ScrabbleWordGenerator wordGenerator = new ScrabbleWordGenerator(dictionaryLocation);

        IPLayer player1;
        IPLayer player2;

        // choose 1st player
        Console.WriteLine();
        Console.WriteLine("Would you like to go first? (Y/N)");
        Console.WriteLine();

        var readkey = Console.ReadLine();

        if ((readkey == "Y") || (readkey == "y"))
        {
            player1 = new HumanPlayer(wordGenerator, scrabbleBoard);
            player2 = new ScrabbleBot(wordGenerator, scrabbleBoard);
        }
        else
        {
            player1 = new ScrabbleBot(wordGenerator, scrabbleBoard);
            player2 = new HumanPlayer(wordGenerator, scrabbleBoard);
        }

        player1.DrawInitialTiles();
        player2.DrawInitialTiles();
        var context = new GameContext();

        while (context.Continue())
        {
            // 1st player makes choice
            player1.MakeMove(context);
            scrabbleBoard.DisplayBoard();

            // 2nd player makes choice
            player2.MakeMove(context);
            scrabbleBoard.DisplayBoard();
        }

        return context.ShouldWeRestartGame();
    }
}