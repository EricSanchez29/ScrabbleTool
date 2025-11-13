
// See https://aka.ms/new-console-template for more information

//string dictionaryLocation = @"C:\WorkSpace\Github\ScrabbleTool\ScrabbleGame\data";

//ScrabbleWordGenerator scrabbleWordGenerator = new ScrabbleWordGenerator(dictionaryLocation);

ScrabbleBoard board = new ScrabbleBoard();

board.DisplayBoard();

Console.WriteLine();
Console.WriteLine("Play first? (Y/N)");

//char playFirst = Console.ReadKey().KeyChar;

bool shutdown = false;


while (!shutdown)
{
    Console.WriteLine("");
    Console.WriteLine("Enter your Scrabble tiles");
    Console.WriteLine("");

    string input = Console.ReadLine();


    if ((input == null) || (input == ".."))
    {
        shutdown = true;
    }
    else
    {
        
    }
}