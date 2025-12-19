

using ScrabbleCommon;

bool restart = true;

while (restart)
{
    restart = Game.StartGame(GlobalVariables.DictionaryRelativePath);
}
