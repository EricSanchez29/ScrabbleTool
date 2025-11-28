
// See https://aka.ms/new-console-template for more information

string dictionaryLocation = @"C:\WorkSpace\Github\ScrabbleTool\ScrabbleCommon\data\NWL2023.txt";

//ScrabbleWordGenerator scrabbleWordGenerator = new ScrabbleWordGenerator(dictionaryLocation);

ScrabbleBoard scrabbleBoard = new ScrabbleBoard();
scrabbleBoard.DisplayBoard();

ScrabbleWordGenerator wordGenerator = new ScrabbleWordGenerator(dictionaryLocation);

//var scrabbleBot = new ScrabbleBot(wordGenerator, scrabbleBoard);

// board.AddWord("CLANKER", "H8");
// board.DisplayBoard();

// var botsMove = scrabbleBot.MakeMove("gtestin");
// //var botsMove = scrabbleBot.MakeMove("utanhre");
// board.AddWord(botsMove); 
// board.DisplayBoard();

bool shutdown = false;
IPLayer pLayer1;
IPLayer pLayer2;

// choose 1st player
Console.WriteLine();
Console.WriteLine("Would you like to go first? (Y/N)");
Console.WriteLine();

var readkey = Console.ReadLine();

if ((readkey == "Y") || (readkey == "y"))
{
    pLayer1 = new HumanPlayer(wordGenerator, scrabbleBoard);
    pLayer2 = new ScrabbleBot(wordGenerator, scrabbleBoard);
}
else
{
    pLayer1 = new ScrabbleBot(wordGenerator, scrabbleBoard);
    pLayer2 = new HumanPlayer(wordGenerator, scrabbleBoard);
}

pLayer1.DrawTiles();
pLayer2.DrawTiles();

while (!shutdown)
{
    // 1st player makes choice
    pLayer1.MakeMove();
    scrabbleBoard.DisplayBoard();
    pLayer1.DrawTiles();


    // 2nd player makes choice
    pLayer2.MakeMove();
    scrabbleBoard.DisplayBoard();
    pLayer2.DrawTiles();
    
}
