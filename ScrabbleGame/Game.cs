public static class Game
{
    public static bool StartGame(string dictionaryLocation)
    {
        ScrabbleBoard scrabbleBoard = new ScrabbleBoard();
        scrabbleBoard.DisplayBoard();

        ScrabbleWordGenerator wordGenerator = new ScrabbleWordGenerator(dictionaryLocation);

        IPlayer player1;
        IPlayer player2;

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

        bool finalMove = false;

        while (context.Continue())
        {
            // 1st player makes choice
            finalMove = player1.MakeMove(context);
            scrabbleBoard.DisplayBoard();

            if (finalMove)
            {
                return finalMoveSequence(false, player2.GetRemainingTiles());
            }



            // 2nd player makes choice
            player2.MakeMove(context);
            scrabbleBoard.DisplayBoard();

            if (finalMove)
            {
                return finalMoveSequence(true, player1.GetRemainingTiles());
            }

            
        }

        return context.ShouldWeRestartGame();
    }

    // bool is shutdown bit? is this weird?
    //
    // who made the final move?
    // player 1 = false, player 2 = true
    private static bool finalMoveSequence(bool finalMovePlayer, string otherPlayerTiles)
    {
        // display final game screen

        // calculate final scores


        return false;
    }
}