public class ScrabbleMove : ScrabbleBase
{
    public int Points { get; set; }
    public required string Word { get; set; }

    public ScrabbleMove()
    {
        
    }

    public ScrabbleMove(ScrabbleBase baseClass)
    {
        Direction = baseClass.Direction;
        X_coordinate = baseClass.X_coordinate;
        Y_coordinate = baseClass.Y_coordinate;
    }
}