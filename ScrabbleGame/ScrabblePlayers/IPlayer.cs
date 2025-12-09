public interface IPlayer
{
    public void MakeMove(GameContext context);

    public void DrawInitialTiles(bool isPlayer1);

    public string GetRemainingTiles();

    public int GetPlayerScore();
}