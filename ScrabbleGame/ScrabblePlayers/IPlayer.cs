public interface IPlayer
{
    // bool is signalling the final move of the game
    public bool MakeMove(GameContext context);

    public void DrawInitialTiles(bool isPlayer1);

    public string GetRemainingTiles();

    public int GetPlayerScore();
}