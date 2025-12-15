using System.Text;

public class PlayerBase
{
    public PlayerBase(IBoard board, ScrabbleWordGenerator wordGenerator)
    {
        scrabbleBoard = board;
        generator = wordGenerator;
    }

    protected IBoard scrabbleBoard;
    protected ScrabbleWordGenerator generator;

    // this doesn't really need to be a list
    // its always a 7 element collection of characters, i can use a default char position to indicate an empty position in the end game
    protected List<char> tileRack = new List<char>(7);

    // the human player doesn't have a use for this yet
    // maybe with better UI I will be able to display moves
    // for now these moves are only tracked by the ai player
    //protected List<ScrabbleMove> moves = new List<ScrabbleMove>();

    protected int score = 0;

    protected string drawInitialTiles(bool isPlayer1)
    {
        if (tileRack.FirstOrDefault() != default(char))
        {
            throw new Exception("Rack should be empty");
        }

        bool invalidRack = true;

        string newTiles = string.Empty; // I think that I shouldn't have to assign this value, but the intellisense thinks otherwise

        while (invalidRack)
        {
            newTiles = scrabbleBoard.DrawTiles(7);

            if (isPlayer1)
            {
                if (generator.IsValidTileRack(newTiles))
                {
                    invalidRack = false;
                }
            }
            else
            {
                invalidRack = false;
            }
        }

        for (int i = 0; i < newTiles.Length; i++)
        {
            tileRack.Add(newTiles[i]);
        }

        return newTiles;
    }

    protected void updateTileRack(string wordTiles)
    {
        for (int i = 0; i < wordTiles.Length; i++)
        {
            if (tileRack.Find(x => x == wordTiles[i]) == default(char))
            {
                //throw new Exception("Word cannot be created using");
                continue;
            }

            tileRack.Remove(wordTiles[i]);
        }
    }

    // player must choose which tiles to switch (input string)
    protected void swapTiles(string unwantedTiles)
    {
        // to do
    }

    protected bool isFinalMove()
    {
        if ((scrabbleBoard.GetBagCount() == 0) && (tileRack.Count == 0))
        {
            // player is making the final move
            return true;
        }

        // another way the 

        return false;
    }

    public string GetRemainingTiles()
    {
        StringBuilder sb = new StringBuilder();

        foreach (char tile in tileRack)
        {
            sb.Append(tile);
        }

        return sb.ToString();
    }

    public int GetPlayerScore()
    {
        return score;
    }
}