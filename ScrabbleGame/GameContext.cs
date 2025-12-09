public class GameContext
{
    private bool? gameState { get; set; }
    // true == continue game
    // false == restart game
    // null == shutdown game

    private bool isFinalMove { get; set;}


    // should I use the shutdown state to signal that the game is over?
    // or 
    private bool gameOver { get; set; }

    private Exception? exception;

    public GameContext()
    {
        gameState = true;
        isFinalMove = false;
    }

    public void EndGame()
    {
        gameOver = true;
        gameState = null;
    }

    public void RecordException(Exception ex)
    {
        exception = ex;

        // either create log here or get this exception in the program to log
    }

    public void SetFinalMove()
    {
        isFinalMove = true;
    }

    public bool GetIsFinalMove()
    {
        return isFinalMove;
    }

    public bool Continue_questionMark()
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

    public bool RestartGame_questionMark()
    {
        if (gameState == false)
        {
            return true;
        }

        return false;
    }

    public bool ShutdownGame_questionMark()
    {
        if (gameState == null)
        {
            return true;
        }

        return false;
    }
}