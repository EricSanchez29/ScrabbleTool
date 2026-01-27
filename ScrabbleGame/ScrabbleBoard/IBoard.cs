
public interface IBoard
{
    public string AddWord(ScrabbleMove move, MoveContext moveContext, string plyaerTiles, out ScrabbleMetaMove metaMove);
    public string DrawTiles(int numberOfTiles);
    public int GetBagCount();
    public ScrabbleMetaMove GetLastMove();
    public List<ScrabbleMetaMove> GetAllMoves();
    public int GetMoveScore(ScrabbleMetaMove metaMove);
    public char GetTileChar(string coordinate);
    public char GetTileChar(int x, int y);
    public (int x, int y) GetXYCoordinate(string coordinate);
    public bool IsOpenSpace(int x, int y);
    public bool IsSameLetter(char boardTile, char rackTile);
    public bool IsSpecialTile(char tile);
    public bool IsValidCoordinate(int x, int y);
    public bool IsWordInBounds(int wordLength, bool direction, int x, int y);
    public bool TryGetMoveScore(ScrabbleBase mainMove, string word, string playerTiles, out int score);
    public bool TryGetXYCoordinate(string coordinate, out int x, out int y);
}