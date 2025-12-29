using System;


// maybe this could be a part of game context but idk its worth passing that object around more places
public class MoveContext
{
    // private bool retryMove;
    // private bool passMove;
    // private bool swapTiles;

    //b1b0
    // 0 0 pass
    // 0 1 swap
    // 1 0 retry
    // 1 1 continue (no retry)
    private bool bit1 = true;
    private bool bit0 = true;

    public MoveContext(bool retryState)
    {
        SetRetryMove(retryState);
    }

    public void SetRetryMove(bool retryState)
    {
        if (retryState)
        {
            bit1 = true;
            bit0 = false;
        }
        else
        {
            bit1 = true;
            bit0 = true;
        }
    }

    public bool GetRetryMove()
    {
        if (bit1 && !bit0)
        {
            return true;
        }

        return false;
    }

    public void SetPassMove()
    {
        bit1 = false;
        bit0 = false;
    }

    public bool GetPassMove()
    {
        if (!bit1 && !bit0)
        {
            return true;
        }

        return false;
    }

    public void SetSwapTiles()
    {
        bit1 = false;
        bit0 = true;
    }

    public bool GetSwapTiles()
    {
        if (!bit1 && bit0)
        {
            return true;
        }

        return false;
    }
}
