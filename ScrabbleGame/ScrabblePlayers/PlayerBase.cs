public class PlayerBase
{
    public PlayerBase(IBoard board)
    {
        scrabbleBoard = board;
    }

    protected IBoard scrabbleBoard;

    protected List<char> tileRack = new List<char>(7);

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

    // to do
    protected void swapTiles(string unwantedTiles)
    {

    }
    
    public void DrawInitialTiles()
    {
        
    }
}