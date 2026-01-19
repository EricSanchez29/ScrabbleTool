using ScrabbleCommon;

public static class Game
{
    // should this be static?
    public static bool StartGame(string dictionaryLocation)
    {
        Console.BackgroundColor = ConsoleColor.Black;

        ScrabbleWordGenerator wordGenerator = new ScrabbleWordGenerator(dictionaryLocation);

        ScrabbleBoard scrabbleBoard = new ScrabbleBoard(wordGenerator);
        scrabbleBoard.DisplayBoard();

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

        bool? finalMoveMaker = null; // false if player 1, true if player 2

        while (context.GetContinue())
        {
            // 1st player makes choice
            player1.MakeMove(context);
            scrabbleBoard.DisplayBoard();
            scrabbleBoard.DisplayScoreBoard(player1.GetPlayerScore(), player2.GetPlayerScore());
            Console.WriteLine();

            if (context.GetIsFinalMove())
            {
                finalMoveMaker = false;
                break;
            }

            // 2nd player makes choice
            player2.MakeMove(context);
            scrabbleBoard.DisplayBoard();
            scrabbleBoard.DisplayScoreBoard(player1.GetPlayerScore(), player2.GetPlayerScore());
            Console.WriteLine();

            if (context.GetIsFinalMove())
            {
                finalMoveMaker = true;
                break;
            }
        }

        // gameover message
        Console.WriteLine();
        Console.WriteLine("Game Over!");

        int player1FinalScore;
        int player2FinalScore;

        if (finalMoveMaker is not null)
        {
            // calculate final scores

            if ((bool)finalMoveMaker) // player 2 made the final move
            {
                var leftOverTiles = player1.GetTilesString();
                var diffScore = 0;

                foreach (var tile in leftOverTiles)
                {
                    diffScore += ScrabbleWordGenerator.GetTilePointValue(tile);
                }

                player1FinalScore = player1.GetPlayerScore() - diffScore;
                player2FinalScore = player2.GetPlayerScore() + diffScore;
            }
            else // player 1 made the final move
            {
                var leftOverTiles = player2.GetTilesString();
                var diffScore = 0;

                foreach (var tile in leftOverTiles)
                {
                    diffScore += ScrabbleWordGenerator.GetTilePointValue(tile);
                }

                player1FinalScore = player1.GetPlayerScore() + diffScore;
                player2FinalScore = player2.GetPlayerScore() - diffScore;
            }
        }
        else // no final score adjustment necessary
        {
            player1FinalScore = player1.GetPlayerScore();
            player2FinalScore = player2.GetPlayerScore();
        }

        // display final score
        Console.WriteLine();
        Console.WriteLine("Final Score:");
        scrabbleBoard.DisplayScoreBoard(player1FinalScore, player2FinalScore);

        // congratulate the winner
        if (player1FinalScore > player2FinalScore)
        {
            Console.WriteLine();
            Console.WriteLine("Player 1 wins!");
        }
        else if (player1FinalScore < player2FinalScore)
        {
            Console.WriteLine();
            Console.WriteLine("Player 2 wins!");
        }
        else // tied game
        {
            Console.WriteLine();
            Console.WriteLine("Tied Game!");
        }

        return context.GetRestartGame();
    }
}