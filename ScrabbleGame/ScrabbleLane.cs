public class ScrabbleLane : ScrabbleBase
{
    public char Tile { get; set; }

    /// <summary>
    /// 0 index position from left to right (up to down)
    /// </summary>
    public int TilePosition { get; set; }
    
    public int Length { get; set; }
}
