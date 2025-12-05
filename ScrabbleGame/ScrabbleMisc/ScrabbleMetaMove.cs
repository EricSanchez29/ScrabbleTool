public class ScrabbleMetaMove
{
    private ScrabbleMove _mainMove;

    private List<ScrabbleMove> additionalMoves = new List<ScrabbleMove>();

    public ScrabbleMetaMove(ScrabbleMove mainMove)
    {
        _mainMove = mainMove;
    }

    public void AddAddtionalMove(ScrabbleMove move)
    {
        additionalMoves.Add(move);
    }

    public List<ScrabbleMove> GetAdditionalMoves()
    {
        return additionalMoves;
    }
    
    public bool HasAdditionalMoves()
    {
        if (additionalMoves.Count > 0)
        {
            return true;
        }

        return false;
    }
}