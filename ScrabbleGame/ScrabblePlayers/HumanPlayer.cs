public class HumanPlayer : PlayerBase, IPlayer
{
    public HumanPlayer(ScrabbleWordGenerator gen, IBoard board) : base(board, gen)
    {

    }


    // need to handle empty bag as an indicator of end of game
    private void drawTiles()
    {
        if (scrabbleBoard.GetBagCount() == 0)
        {
            return;
        }

        string newTiles = scrabbleBoard.DrawTiles(7 - tileRack.Count);
        Console.WriteLine();
        Console.WriteLine("New Tiles: " + newTiles);

        for (int i = 0; i < newTiles.Length; i++)
        {
            tileRack.Add(newTiles[i]);
        }
    }

    private void displayPlayerTiles()
    {
        Console.WriteLine();

        for (int i = 0; i < tileRack.Count; i++)
        {
            Console.Write(tileRack[i]);
        }
    }

    public void MakeMove(GameContext gameContext)
    {
        MoveContext moveContext = new MoveContext(true);

        string? word = string.Empty;
        string? coordinate = string.Empty;
        int x_coordinate = int.MaxValue;
        int y_coordinate = int.MaxValue;
        bool direction = true;
        string playerLettersUsed = string.Empty;

        var playerMove = new ScrabbleMove()
        {
            Word = string.Empty,
        };

        ScrabbleMetaMove metaMove = new ScrabbleMetaMove(playerMove);

        while (moveContext.GetRetryMove())
        {
            moveContext.SetRetryMove(false);

            Console.WriteLine();
            Console.WriteLine("Your turn:");
            displayPlayerTiles();

            Console.WriteLine();
            Console.WriteLine("Enter your next word: ");

            word = Console.ReadLine();

            if ((word is null) || (word is default(string)))
            {
                moveContext.SetRetryMove(true);
                continue;
            }

            if (word == "(swap)")
            {
                if (scrabbleBoard.GetBagCount() == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Cannot swap tiles, tile bag is empty");
                    Console.WriteLine();
                    moveContext.SetRetryMove(true);
                    continue;
                }

                moveContext.SetSwapTiles();
                break;
            }

            if (word == "(pass)")
            {
                moveContext.SetPassMove();
                break;
            }

            if (!generator.CheckDictionary(word))
            {
                Console.WriteLine("Word not found in dictionary.");
                Console.WriteLine("Retry:");
                moveContext.SetRetryMove(true);
                continue;
            }

            Console.WriteLine("");
            Console.WriteLine("Enter coordinate ('A1' - 'O15')");
            Console.WriteLine("");


            coordinate = Console.ReadLine();

            if ((coordinate is null) || (coordinate is default(string)))
            {
                moveContext.SetRetryMove(true);
                continue;
            }

            bool valid = scrabbleBoard.TryGetXYCoordinate(coordinate, out x_coordinate, out y_coordinate);

            if (!valid)
            {
                Console.WriteLine("Invalid coordinate");
                Console.WriteLine("Retry:");
                moveContext.SetRetryMove(true);
                continue;
            }

            Console.WriteLine("");
            Console.WriteLine("Enter direction ('down' or 'across')");
            Console.WriteLine("");

            // read key
            string? directionString = Console.ReadLine();

            if (directionString == "down" || directionString == "d" || directionString == "DOWN" || directionString == "D")
            {
                direction = false;
            }
            else if (directionString == "across" || directionString == "a" || directionString == "ACROSS" || directionString == "A")
            {
                direction = true;
            }
            else
            {
                Console.WriteLine("Invalid direction");
                Console.WriteLine("Retry:");
                moveContext.SetRetryMove(true);
                continue;
            }

            // change this later maybe?
            // let program.cs handle this?
            if ((x_coordinate == int.MaxValue) || (y_coordinate == int.MaxValue) || (word == string.Empty))
            {
                throw new Exception("Invalid input");
            }

            playerMove = new ScrabbleMove()
            {
                Word = ScrabbleWordGenerator.ConvertLowerWordToUpperWord(word!),
                X_coordinate = x_coordinate,
                Y_coordinate = y_coordinate,
                Direction = direction,
            };

            playerLettersUsed = scrabbleBoard.AddWord(playerMove, moveContext, GetRemainingTiles(), out metaMove);
        }

        if (moveContext.GetPassMove())
        {
            Console.WriteLine();
            Console.WriteLine("Passing Move");
            Console.WriteLine();
        }
        else if (moveContext.GetSwapTiles())
        {
            Console.WriteLine();
            Console.WriteLine("Swapping tiles");
            Console.WriteLine("Type letters you wish to swap: ");

            string? unwantedTiles = Console.ReadLine();

            if (unwantedTiles is null)
            {
                throw new Exception("FATAL ERROR");
            }

            updateTileRack(unwantedTiles);

            drawTiles();

            displayPlayerTiles();
        }
        else
        {
            updateTileRack(playerLettersUsed);

            drawTiles();

            displayPlayerTiles();

            score += metaMove.GetTotalScore(); // total score not set
        }

        if (base.isFinalMove())
        {
            gameContext.SetFinalMove();
        }
    }

    public void DrawInitialTiles(bool isPlayer1)
    {
        var newTiles = drawInitialTiles(isPlayer1);

        Console.WriteLine();
        Console.WriteLine("Player Tiles: " + newTiles);
    }
}