
using ScrabbleCommon;


// ScrabbleWordGenerator wordGenerator = new ScrabbleWordGenerator(GlobalVariables.DictionaryRelativePath);

// Morphemes.GenerateMorphemeFile(GlobalVariables.RootDictionaryFileName);

// var test = Morphemes.GetRootWordDictionary(GlobalVariables.RootDictionaryFileName);


bool restart = true;

while (restart)
{
    // in a later version of the game I could prompt the user to choose a specific dictionaries
    //
    // currently only use the NASPA Word List 2023 [American standard]
    // other options
    // Collins Scrabble Words CSW [British Standard]
    // Words with friends (WWF)
    // Spanish (probably the only foreign language I ever want to support for fluency reasons)
    restart = Game.StartGame(GlobalVariables.DictionaryRelativePath);
}
