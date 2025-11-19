public interface IPLayer
{
    public int GetBagCount();
    public char GetTileChar(string coordinate);
    public char GetTileChar(int x, int y);
    public bool IsOpenSpace(int x, int y);
    public void AddWord(string word, string coordinate, bool direction /*= true*/);
    public (int x, int y) GetBoardPosition(string coordinate);
}