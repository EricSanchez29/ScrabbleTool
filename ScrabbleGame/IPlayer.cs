public interface IPLayer
{
    public int GetBagCount();
    public char GetTileChar(string coordinate);
    public char GetTileChar(int x, int y);
    public bool IsOpenSpace(int x, int y);
    public void AddWord(string word, string coordinate, bool direction);
    public void AddWord(ScrabbleMove move);
    public (int x, int y) GetBoardPosition(string coordinate);
    public int GetMoveScore(in ScrabbleBase move, string word);
    public ScrabbleMove GetLastMove();
    // or should this function be part of addword (return string instead of void)
    public string DrawTiles(int numberOfTiles);
}