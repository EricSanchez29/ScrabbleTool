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

            if (!generator.CheckDictionary(word))
            {
                Console.WriteLine("Word not found in dictionary.");
                Console.WriteLine("Retry :");
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
                Console.WriteLine("Retry :");
                moveContext.SetRetryMove(true);
                continue;
            }

            Console.WriteLine("");
            Console.WriteLine("Enter direction ('down' - 'across')");
            Console.WriteLine("");

            // read key
            string? directionString = Console.ReadLine();

            if (directionString == "down" || directionString == "d")
            {
                direction = false;
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

            // this line looks weird, change this later by adding more versions of the function
            playerMove.Score = scrabbleBoard.GetMoveScore(playerMove, word!);

            playerLettersUsed = scrabbleBoard.AddWord(playerMove, moveContext);

            if (moveContext.GetRetryMove())
            {
                continue;
            }
        }

        // work on this
        // 1. AddWord should ref out a MetaMove
        // 2. Handle Swap (in playBase)
        // 3. Handle Pass
        // 4. 

        // Console.WriteLine();
        // Console.WriteLine("+" + playerMove.Score + " points");
        // Console.WriteLine();

        // this is ignorant of tiles on the board, should the scrabble board obj tell you which tiles to remove?
        // ex played "TOWER" with T already on board an on rack, removed the rack tile
        updateTileRack(playerLettersUsed);

        drawTiles();

        score += playerMove.Score;

        if(base.isFinalMove())
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