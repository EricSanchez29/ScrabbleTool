public class HumanPlayer : PlayerBase, IPLayer
{
    public HumanPlayer(ScrabbleWordGenerator gen, IBoard board) : base(board)
    {
        generator = gen;
    }

    private ScrabbleWordGenerator generator;

    // need to 
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

    public bool MakeMove(GameContext context)
    {
        bool retry = true;
        string word = string.Empty;
        string coordinate = string.Empty;
        int x_coordinate = int.MaxValue;
        int y_coordinate = int.MaxValue;
        bool direction = true;

        while (retry)
        {
            retry = false;

            Console.WriteLine();
            Console.WriteLine("Your turn:");
            displayPlayerTiles();

            Console.WriteLine();
            Console.WriteLine("Enter your next word: ");

            word = Console.ReadLine();

            if ((word is null) || (word is default(string)))
            {
                retry = true;
                continue;
            }

            if (!generator.CheckDictionary(word))
            {
                Console.WriteLine("Word not found in dictionary.");
                Console.WriteLine("Retry :");
                retry = true;
                continue;
            }

            Console.WriteLine("");
            Console.WriteLine("Enter coordinate ('A1' - 'O15')");
            Console.WriteLine("");


            coordinate = Console.ReadLine();

            if ((coordinate is null) || (coordinate is default(string)))
            {
                retry = true;
                continue;
            }

            bool valid = scrabbleBoard.TryGetXYCoordinate(coordinate, out x_coordinate, out y_coordinate);

            if (!valid)
            {
                Console.WriteLine("Word not found in dictionary.");
                Console.WriteLine("Retry :");
                retry = true;
                continue;
            }

            Console.WriteLine("");
            Console.WriteLine("Enter direction ('down' - 'across')");
            Console.WriteLine("");

            // read key
            string directionString = Console.ReadLine();

            if (directionString == "down" || directionString == "d")
            {
                direction = false;
            }
        }

        // change this later maybe?
        // let program.cs handle this?
        if ((x_coordinate == int.MaxValue) || (y_coordinate == int.MaxValue) || (word == string.Empty))
        {
            throw new Exception("Invalid input");
        }

        scrabbleBoard.AddWord(new ScrabbleMove()
        {
            Word = word,
            X_coordinate = x_coordinate,
            Y_coordinate = y_coordinate,
            Direction = direction,

        });

        updateTileRack(word);

        drawTiles();

        return isFinalMove();
    }

    public void DrawInitialTiles()
    {
        if (tileRack.FirstOrDefault() == default(char))
        {
            throw new Exception("Rack should not be empty");
        }
        
        string newTiles = scrabbleBoard.DrawTiles(7);
        Console.WriteLine();
        Console.WriteLine("Player Tiles: " + newTiles);

        for (int i = 0; i < newTiles.Length; i++)
        {
            tileRack.Add(newTiles[i]);
        }
    }
}