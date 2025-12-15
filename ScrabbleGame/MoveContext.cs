using System;


// maybe this could be a part of game context but idk its worth passing that object around more places
public class MoveContext
{
    private bool retryMove;
    private bool passMove;
    private bool swapTiles;

    // need to have some restrictions that prevent any of these from being simultaneously true
    // pass
    // swap
    // retry
    
    

    public MoveContext(bool retryState)
    {
        SetRetryMove(retryState);
        passMove = false;
    }

    public void SetRetryMove(bool retryState)
    {
        retryMove = retryState;
    }

    public bool GetRetryMove()
    {
        return retryMove;
    }

    public void SetPassMove(bool passState)
    {
        passMove = passState;
    }

    public bool GetPassMove()
    {
        return passMove;
    }

    public void SetSwapTiles(bool swapState)
    {
        swapTiles = true;
    }

    public bool GetSwapTiles()
    {
        return swapTiles;
    }
}
