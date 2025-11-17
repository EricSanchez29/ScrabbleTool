using System;

//namespace ScrabbleGame;

public class ScrabbleLane
{
    public int Points { get; set; }
    public required string Word { get; set; }
    public int X_coordinate { get; set; }
    public int Y_coordinate { get; set; }
    public bool Direction { get; set; }
}
