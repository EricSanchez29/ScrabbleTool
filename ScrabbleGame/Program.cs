
string dictionaryLocation = @"C:\WorkSpace\Github\ScrabbleTool\ScrabbleCommon\data\NWL2023.txt";

bool restart = true;

while (restart)
{
    restart = Game.StartGame(dictionaryLocation);
}
