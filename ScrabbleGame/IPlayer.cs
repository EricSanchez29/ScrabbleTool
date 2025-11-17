public interface IPLayer
{
    public int GetBagCount();
    public char GetTileValue(string coordinate);
    public char GetTileValue(int x, int y);
    public bool IsOpenSpace(int x, int y);
    public void AddWord(string word, string coordinate, bool direction /*= true*/);
    public (int x, int y) GetBoardPosition(string coordinate);
}