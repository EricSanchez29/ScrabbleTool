public interface IPLayer
{
    int GetBagCount();
    char GetTileValue(string coordinate);
    void AddWord(string word, string coordinate, bool direction /*= true*/);
}