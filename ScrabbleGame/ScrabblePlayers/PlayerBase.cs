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
            if (!tileRack.Remove(wordTiles[i]))
            {
                // failed to remove tile, does the user have a blank tile?

                if (!tileRack.Remove('*'))
                {
                    throw new Exception("Couldn't remove " + wordTiles[i] + "but player does not have a * tile");
                }
            }
        }
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