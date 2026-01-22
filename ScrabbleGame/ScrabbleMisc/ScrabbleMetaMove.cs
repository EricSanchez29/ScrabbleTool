public class ScrabbleMetaMove
{
    private ScrabbleMove _mainMove;

    private List<ScrabbleMove> additionalMoves = new List<ScrabbleMove>();

    private int _totalScore = 0;

    public ScrabbleMetaMove(ScrabbleMove mainMove)
    {
        _mainMove = mainMove;
    }

    public void SetMainMoveScore(int subScore)
    {
        _mainMove.Score = subScore;
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

    // I'm to lazy to do this the "correct way"
    // this works but they could be public properties of metaMove
    // change when I have time
    public void SetTotalScore(int totalScore)
    {
        _totalScore = totalScore;
    }

    public int GetTotalScore()
    {
        return _totalScore;
    }
    
    public ScrabbleMove GetMainMove()
    {
        return _mainMove;
    }
}