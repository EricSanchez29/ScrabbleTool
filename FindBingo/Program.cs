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
Console.WriteLine("Enter the word");
Console.WriteLine("");
string input = Console.ReadLine();


var bingos = generator.GetBingoList("input");

// need to add check for impossible letter combos
// add different functionality for very long words, greater than 12?



Console.WriteLine(bingos.Count);

// maybe limit the number of entries within the generator itself
//int counter = 0;



//bingos is getting many duplicates
// should I change my algo?
// should I check a hashset and then convert to list at the end?



// order bingo with some lambda function
var sortedWords = bingos.OrderByDescending(t => t.Item2);//.ToList();

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
