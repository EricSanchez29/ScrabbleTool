
// this interface doesn't really serve a purpose anymore, 
public interface IBoard
{
    public int GetBagCount();
    public char GetTileChar(string coordinate);
    public char GetTileChar(int x, int y);
    public bool IsOpenSpace(int x, int y);
    public bool IsSpecialTile(char tile);
    public bool IsValidCoordinate(int x, int y);
    public bool IsWordInBounds(int wordLength, bool direction, int x, int y);
    public bool IsSameLetter(char boardTile, char rackTile);
    public bool TryGetXYCoordinate(string coordinate, out int x, out int y);
    public string AddWord(ScrabbleMove move, MoveContext moveContext, string plyaerTiles, out ScrabbleMetaMove metaMove);
    public (int x, int y) GetXYCoordinate(string coordinate);
    public bool TryGetMoveScore(ScrabbleBase mainMove, string word, string playerTiles, out int score);
    public int GetMoveScore(ScrabbleMetaMove metaMove, string playerTiles);
    public ScrabbleMetaMove GetLastMove();
    // or should this function be part of addword (return string instead of void)
    public string DrawTiles(int numberOfTiles);
    public List<ScrabbleMetaMove> GetAllMoves();
    public void DisplayBoard();
}