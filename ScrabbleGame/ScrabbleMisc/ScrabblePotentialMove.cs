public class ScrabblePotentialMove
{
    // Associating a specific lane with each potential move makes it easier to remove lane from list
    // once the move has offically been decided

    public ScrabblePotentialMove(ScrabbleMove move, ScrabbleLane lane)
    {
        Move = move;
        Lane = lane;
    }


    public ScrabbleMove Move { get; }

    public ScrabbleLane Lane { get; }
}