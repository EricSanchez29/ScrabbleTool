public interface IBoard
{
    public int GetBagCount();
    public char GetTileChar(string coordinate);
    public char GetTileChar(int x, int y);
    public bool IsOpenSpace(int x, int y);
    public bool IsValidCoordinate(int x, int y);
    public bool IsWordOutOfBounds(int wordLength, bool direction, int x, int y);
    public bool TryGetXYCoordinate(string coordinate, out int x, out int y);
    public string AddWord(ScrabbleMove move, MoveContext moveContext, string plyaerTiles, out ScrabbleMetaMove metaMove); 
    public (int x, int y) GetXYCoordinate(string coordinate);
    public int GetPotentialMoveScore(in ScrabbleBase move, string word);
    public ScrabbleMetaMove GetLastMove();
    // or should this function be part of addword (return string instead of void)
    public string DrawTiles(int numberOfTiles);
}