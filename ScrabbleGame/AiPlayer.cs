using System.Text;

public class ArtificiallyUnintelligentPlayer
{
    public ArtificiallyUnintelligentPlayer(ScrabbleWordGenerator wordGenerator, IPLayer player)
    {
        generator = wordGenerator;
        scrabblePlayer = player;
    }

    private ScrabbleWordGenerator generator;

    private IPLayer scrabblePlayer;

    public (string word_coordinate, bool direction) MakeMove(string playerTiles)
    {
        // if center is empty, no one has made a legal scrabble move yet
        if (scrabblePlayer.GetTileValue("G8") == ' ')
        {
            return makeStartingMove(playerTiles);
        }

        // TO DO

        return (string.Empty, false);
    }

    private (string word_coordinate, bool direction) makeStartingMove(string playerTiles)
    {
        // get potential bingos
        var bingoWords = generator.GetBingoList(playerTiles).OrderBy(x => x.Score);
    
        // choose word with highest bonus points
        string chosenWord = string.Empty;
        string startingPosition = string.Empty;

        // only do this if have potential bingos of length 5, 6, or 7
        //
        // (tuple can't be null)
        var longWord = bingoWords.First(x => x.Word.Length > 4).Word;
        if ( longWord == null || longWord == string.Empty)
        {
            // check this
            bingoWords.OrderBy(x => x.Word.Length);
            chosenWord = bingoWords.First().Word;

            // for not default choice is the center square
            startingPosition = "G8";
        }
        else
        {
            
        }


        // choose direction
        // this doesn't really matter as much for first move

        return (chosenWord + "_" + startingPosition, false);
    }


    // prime objectives

    // 1. pick words with the most points

    // 2. pick words that utilize bonus tiles

    // 3. pick words that block your opponent from utilizing

    // 4. (end game) when bag is empty prioritize words that use as many of your letters as possible

}
