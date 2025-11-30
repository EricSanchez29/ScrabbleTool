public interface IPLayer
{
    // bool is signalling the final move of the game
    public bool MakeMove(GameContext context);

    public void DrawInitialTiles();

    public string GetRemainingTiles();
}