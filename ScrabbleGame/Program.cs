
// See https://aka.ms/new-console-template for more information

string dictionaryLocation = @"C:\WorkSpace\Github\ScrabbleTool\ScrabbleCommon\data\NWL2023.txt";

//ScrabbleWordGenerator scrabbleWordGenerator = new ScrabbleWordGenerator(dictionaryLocation);

ScrabbleBoard board = new ScrabbleBoard();
board.DisplayBoard();

ScrabbleWordGenerator wordGenerator = new ScrabbleWordGenerator(dictionaryLocation);

Console.WriteLine();
Console.WriteLine("Play first? (Y/N)");

//char playFirst = Console.ReadKey().KeyChar;

bool shutdown = false;


while (!shutdown)
{
    Console.WriteLine("");
    Console.WriteLine("Enter your Scrabble tiles");
    Console.WriteLine("");

    string word = Console.ReadLine();

    // check input string?

    if ((word == null) || (word == ".."))
    {
        shutdown = true;
    }
    else
    {
        // check dictionary
        if (!wordGenerator.CheckDictionary(word))
        {
            Console.WriteLine("");
            Console.WriteLine("Word not found in dictionary");
            Console.WriteLine("");
            continue;
        }

        Console.WriteLine("");
        Console.WriteLine("Enter coordinate ('A1' - 'O15')");
        Console.WriteLine("");

        
        string coordinate = Console.ReadLine();

        if ((coordinate == null) || (coordinate == ".."))
        {
            shutdown = true;
        }

        Console.WriteLine("");
        Console.WriteLine("Enter direction ('down' - 'across')");
        Console.WriteLine("");

        // read key
        string directionString = Console.ReadLine();
        bool direction = true; // default is across

        if (directionString == "down")
        {
            direction = false;
        }

        // place tiles on board
        board.AddWord(word, coordinate, direction);

        // draw new tiles
        board.DrawTiles(word.Length);

        // display board
        board.DisplayBoard();


        // autoplayer turn
        // - check dictionary, place tiles, draw new tiles

    }
}