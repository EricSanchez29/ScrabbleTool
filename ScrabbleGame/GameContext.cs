// would it make sense to make this static
public class GameContext
{
    private bool? gameState { get; set; }
    // true == continue game
    // false == restart game
    // null == shutdown game

    private bool isFinalMove { get; set; }



    public GameContext()
    {
        gameState = true;
        isFinalMove = false;
    }

    // public void EndGame()
    // {
    //     gameOver = true;
    //     gameState = null;
    // }

    // public void RecordException(Exception ex)
    // {
    //     exception = ex;

    //     // either create log here or get this exception in the program to log
    // }
    //private Exception? exception;

    public void SetFinalMove()
    {
        isFinalMove = true;
    }

    public bool GetIsFinalMove()
    {
        return isFinalMove;
    }

    public bool GetContinue()
    {
        if (gameState == true)
        {
            return true;
        }

        return false;
    }

    public void SetRestartGame()
    {
        gameState = false;
    }

    public void SetShutdownGame()
    {
        gameState = null;
    }

    public bool GetRestartGame()
    {
        if (gameState == false)
        {
            return true;
        }

        return false;
    }

    public bool GetShutdownGame()
    {
        if (gameState == null)
        {
            return true;
        }

        return false;
    }
}