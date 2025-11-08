// See https://aka.ms/new-console-template for more information


// file name
string file = @"C:\WorkSpace\Github\ScrabbleTool\FindBingo\data\NWL2023.txt";
//"data\NWL2023.txt";

//
/*Need to fix this path*/
//
string filePath = Path.GetFullPath(file);

var generator = new ScrabbleWordGenerator(filePath);


Console.WriteLine("ScrabbleTool: First atttempt");
Console.WriteLine("");
Console.WriteLine("");

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
        var bingos = generator.GetBingoList(input);

        Console.WriteLine(bingos.Count);

        // order bingo with some lambda function
        var sortedWords = bingos.OrderByDescending(t => t.Item2);//.ToList();

        // maybe limit the number of entries within the generator itself
        int counter = 0;

        foreach (var bingo in sortedWords)
        {
            Console.WriteLine(bingo.Item1 + " = " + bingo.Item2);
            counter++;

            if (counter == 100)
            {
                return;
            }
        } 
    }
}




