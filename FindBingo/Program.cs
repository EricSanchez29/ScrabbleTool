// See https://aka.ms/new-console-template for more information
Console.WriteLine("ScrabbleTool: First atttempt");
Console.WriteLine("");
//Console.WriteLine("Enter the word");


//open file
// 

var bingos = Bingo.GetBingoList("strange");
//var bingos = Bingo.GetBingoList("abcd");

Console.WriteLine(bingos.Count);

int counter = 0;
foreach (var bingo in bingos)
{
    Console.WriteLine(bingo.Item1 + " = " + bingo.Item2);
    counter++;

    // if (counter == 100)
    // {
    //     return;
    // }
} 
