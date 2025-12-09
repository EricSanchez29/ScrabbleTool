public static class Game
{
    // should this be static?
    public static bool StartGame(string dictionaryLocation)
    {
        ScrabbleWordGenerator wordGenerator = new ScrabbleWordGenerator(dictionaryLocation);

        ScrabbleBoard scrabbleBoard = new ScrabbleBoard(wordGenerator);
        scrabbleBoard.DisplayBoard();


        // //testing
        // var bot = new ScrabbleBot(wordGenerator, scrabbleBoard);

        // var testString = "TESTI**";
        // bot.TestMove(testString.ToCharArray(), 'A');


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

        player1.DrawInitialTiles(true);
        player2.DrawInitialTiles(false);
        var context = new GameContext();

        while (context.Continue_questionMark())
        {
            // 1st player makes choice
            player1.MakeMove(context);
            scrabbleBoard.DisplayBoard();
            Console.WriteLine();
            Console.WriteLine("Player 1 score: " + player1.GetPlayerScore());
            Console.WriteLine();

            if (context.GetIsFinalMove())
            {
                return finalMoveSequence(false, player2.GetRemainingTiles());
            }

            // 2nd player makes choice
            player2.MakeMove(context);
            scrabbleBoard.DisplayBoard();
            Console.WriteLine();
            Console.WriteLine("Player 2 score: " + player2.GetPlayerScore());
            Console.WriteLine();

            if (context.GetIsFinalMove())
            {
                return finalMoveSequence(true, player1.GetRemainingTiles());
            }
        }

        return context.RestartGame_questionMark();
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

    private static int calculateFinalScore(IPlayer player1, IPlayer player2)
    {
        return 0;
    }
}