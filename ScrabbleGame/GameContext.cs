public class GameContext
{
    private bool? gameState { get; set; }
    // true == continue game
    // false == restart game
    // null == shutdown game

    // should I use the shutdown state to signal that the game is over?
    // or 
    private bool gameOver { get; set; }

    private Exception? exception;

    public GameContext()
    {
        gameState = true;
    }



    public void EndGame()
    {
        
    }

    public void RecordException(Exception ex)
    {
        exception = ex;

        // either create log here or get this exception in the program to log
    }

    public bool Continue()
    {
        if (gameState == true)
        {
            return true;
        }

        return false;
    }

    public void RestartGame()
    {
        gameState = false;
    }

    public void ShutdownGame()
    {
        gameState = null;
    }

    public bool ShouldWeRestartGame()
    {
        if (gameState == false)
        {
            return true;
        }

        return false;
    }

    public bool ShouldWeShutdownGame()
    {
        if (gameState == null)
        {
            return true;
        }

        return false;
    }
}