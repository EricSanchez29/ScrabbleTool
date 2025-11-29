public interface IPLayer
{
    // is this really necessary?
    // won't I only ever draw tiles after making a move? Could possibly avoid duplicate
    // if I have MakeMove() handle this
    public void DrawTiles();

    public void MakeMove();
    
    //public void SwapTiles?
}