public interface IPlayer
{
    public void DrawInitialTiles(bool isPlayer1);
    public int GetPlayerScore();
    public string GetTilesString();
    public void MakeMove(GameContext context);
}