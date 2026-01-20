using System.Text;
using System.Xml.Schema;
using ScrabbleCommon;
public class ScrabbleBoard : IBoard, IDisplay
{
    public ScrabbleBoard(ScrabbleWordGenerator gen)
    {
        board = createBoard();
        bag = createScrabbleBag();
        rando = new Random();
        generator = gen;
    }

    ScrabbleWordGenerator generator;

    // Scrabble board
    private char[,] board;

    // Scrabble bag
    private List<char> bag;

    // Persistent instance of Random
    private Random rando;

    // Player Moves
    private List<ScrabbleMove> playerMoves = [];
    // get rid of one of these later
    private List<ScrabbleMetaMove> metaMoves = [];

    private HashSet<string> boardWords = new HashSet<string>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="word"> valid scrabble word </param>
    /// <param name="coordinate"> A1 - O15 </param>
    /// <param name="direction"> across (true) or down (false) </param>
    /// <returns></returns>
    public void AddWord(string word, string coordinate, bool direction = true)
    {
        // convert from Scrabble coordinate system to array coordinates
        var startingPosition = GetXYCoordinate(coordinate);

        ScrabbleMove move = new ScrabbleMove()
        {
            Direction = direction,
            Word = word,
            X_coordinate = startingPosition.x,
            Y_coordinate = startingPosition.y,
        };

        //AddWord(move);
    }

    // should another param with player letters, or maybe could track players letters?
    // also need to add better checks for occupied board space, need to take into account using letter already on board
    // and also add a bingo check +50 points

    // return how many letters were not already on the board
    public string AddWord(ScrabbleMove playerMove, MoveContext moveContext, string playerTiles, out ScrabbleMetaMove scrabbleMetaMove)
    {
        var tilesToRemove = new StringBuilder();

        int totalScore = 0;

        if (!tryIsValidMove(playerMove, playerTiles, out ScrabbleMetaMove? metaMove))
        {
            moveContext.SetRetryMove(true);
            scrabbleMetaMove = new ScrabbleMetaMove(playerMove);
            return string.Empty;
        }

        moveContext.SetRetryMove(false);

        // calculate main move score
        playerMove.Score = GetMoveScore(metaMove ?? new ScrabbleMetaMove(playerMove), playerMove.Word);
        totalScore += playerMove.Score;

        Console.WriteLine();
        Console.WriteLine("Main move: " + playerMove.Word + " +" + playerMove.Score + " points");

        if (metaMove is not null) // this move creates additional words besides Move.Word
        {
            foreach (var additionalMove in metaMove.GetAdditionalMoves())
            {
                totalScore += additionalMove.Score;

                addWordToHashSet(additionalMove);

                Console.WriteLine();
                Console.WriteLine("Adjacent move: " + additionalMove.Word + " +" + additionalMove.Score + " points");
            }
        }

        addWordToHashSet(playerMove);

        // only manipulate tiles (output string) here
        if (!playerMove.Direction)
        {
            int y_offset = playerMove.Y_coordinate;

            for (int i = 0; i < playerMove.Word.Length; i++)
            {
                // check if position is already taken
                if (!IsSpecialTile(board[playerMove.X_coordinate, y_offset]))
                {
                    // if this position is already occupied by the same letter than I am not overwriting
                    // I'm merely using a board letter in my word
                    if (!IsSameLetter(board[playerMove.X_coordinate, y_offset], playerMove.Word[i]) )
                    {
                        // shouldn't reach this if I am correctly checking this in tryIsValidMove()
                        // this is a game breaking error
                        throw new Exception("Invalid play, board position already occupied. Error occured in tryIsValidMove()");
                    }
                }
                else
                {
                    // will only write to the scrabbleboard with the tiles in my hand
                    board[playerMove.X_coordinate, y_offset] = playerMove.Word[i];

                    tilesToRemove.Append(playerMove.Word[i]);
                }

                y_offset++;
            }
        }
        else
        {
            int x_offset = playerMove.X_coordinate;

            for (int i = 0; i < playerMove.Word.Length; i++)
            {
                // check if position is already taken
                if (!IsSpecialTile(board[x_offset, playerMove.Y_coordinate]))
                {
                    // if this position is already occupied by the same letter than I am not overwriting
                    // I'm merely using a board letter in my word
                    if (!IsSameLetter(board[x_offset, playerMove.Y_coordinate], playerMove.Word[i]))
                    {
                        // shouldn't reach this if I am correctly checking this in tryIsValidMove()
                        // this is a game breaking error
                        throw new Exception("Invalid play, board position already occupied. Error occured in tryIsValidMove()");
                    }

                }
                else
                {
                    // will only write to the scrabbleboard with the tiles in my hand
                    board[x_offset, playerMove.Y_coordinate] = playerMove.Word[i];

                    tilesToRemove.Append(playerMove.Word[i]);
                }

                x_offset++;
            }

        }

        // only do this when no adjacent words were found
        if (metaMove is null)
        {
            metaMove = new ScrabbleMetaMove(playerMove);
        }

        metaMove.SetTotalScore(totalScore);

        metaMoves.Add(metaMove);

        scrabbleMetaMove = metaMove;

        Console.WriteLine();
        Console.WriteLine("+" + totalScore + " points");
        Console.WriteLine();

        return tilesToRemove.ToString();
    }
    
    public void DisplayBoard()
    {
        Console.WriteLine();
        Console.WriteLine(" ");
        Console.Write(" ");
        Console.Write(" ");
        Console.Write(" ");
        Console.Write(" ");

        // A - O
        for (int k = 0; k < 15; k++)
        {
            Console.Write(" ");
            int letter = k + 65;
            Console.Write((char)letter);
            Console.Write(" ");
            Console.Write(" ");
            //Console.Write(" ");
        }

        // 1 - 15
        for (int j = 0; j < 15; j++)
        {

            Console.WriteLine("");
            Console.Write(" ");
            Console.Write(j + 1);

            // So single and double digit numbers take up the same space
            if (j < 9)
            {
                Console.Write(" ");
            }

            for (int i = 0; i < 15; i++)
            {
                Console.Write(" ");
                //Console.Write(" ");

                var tileValue = (int)board[i, j];

                /*
                    A-Z and DL('[') , DW('\'), TL(']'), TW('^')
                */
                if ((tileValue >= 65) && (tileValue <= 90))
                {
                    Console.Write(" ");
                    Console.Write((char)tileValue);

                }
                else if ((tileValue >= 97) && (tileValue <= 122))
                {
                    // this is a blank tile used as another letter
                    Console.Write(" ");
                    Console.Write((char)(tileValue - 32));
                }
                else if (tileValue == 91)
                {
                    Console.Write("dl");
                }
                else if (tileValue == 92)
                {
                    Console.Write("dw");
                }
                else if (tileValue == 93)
                {
                    Console.Write("tl");
                }
                else if (tileValue == 94)
                {
                    Console.Write("tw");
                }
                else
                {
                    Console.Write(" ");
                    Console.Write(" ");
                }

                Console.Write(" ");
            }
        }
    }

    public void DisplayScoreBoard(int player1Score, int player2Score)
    {
        Console.WriteLine();
        Console.WriteLine(" ");
        Console.Write(" ");

        Console.Write("Player 1:");
        displayNumberString(player1Score);

        for (int i = 0; i < 33; i++)
        {
            Console.Write(" ");
        }
        

        Console.Write("Player 2:");
        displayNumberString(player2Score);
        Console.Write(" ");
        Console.Write(" ");
        Console.Write(" ");

    }
    
    
    private void displayNumberString(int playerScore)
    {
        StringBuilder sb;

        if (playerScore < 0)
        {
            throw new Exception("Something went wrong, cannot have a negative score.");
        }
        else if ((playerScore < 10) && (playerScore >= 0))
        {
            // add 3 spaces before number
            sb = new StringBuilder(Convert.ToString(playerScore));

            sb.Insert(0, "   ");
        }
        else if ((playerScore < 100) && (playerScore >= 10))
        {
            // add 2 space before number
            sb = new StringBuilder(Convert.ToString(playerScore));

            sb.Insert(0, "  ");
        }
        else if ((playerScore < 1000) && (playerScore >= 100))
        {
            // add 1 space before number
            sb = new StringBuilder(Convert.ToString(playerScore));

            sb.Insert(0, ' ');
        }
        else
        {
            // add no space before number
            sb = new StringBuilder(Convert.ToString(playerScore));

            if (sb.Length > 4)
            {
                throw new Exception("Something went wrong, cannot handle a score higher than 9999.");
            }
        }

        var debug = sb.ToString();

        Console.Write(sb.ToString());
    }

    public string DrawTiles(int tileCount)
    {
        if ((tileCount >= 8) || (tileCount <= 0))
        {
            // handle this better
            throw new Exception("Invalid command, can only request between 1 and 7 tiles");
        }

        var list = new StringBuilder(tileCount);

        for (int i = tileCount; i > 0; i--)
        {
            if (bag.Count == 0)
            {
                // if the tileCount requested is larger than the
                // number of tiles in the bag
                // return the number of tiles available
                break;
            }

            // draw random tiles from bag
            var randomNumber = rando.Next(0, bag.Count - 1);
            list.Append(bag[randomNumber]);
            bag.RemoveRange(randomNumber, 1);
        }

        return list.ToString();
    }

    public int GetBagCount()
    {
        return bag.Count();
    }

    public char GetTileChar(string coordinate)
    {
        var cartesian = GetXYCoordinate(coordinate);
        return board[cartesian.x, cartesian.y];
    }

    public char GetTileChar(int x, int y)
    {
        return board[x, y];
    }
    
    public bool IsOpenSpace(int x, int y)
    {
        return IsSpecialTile(board[x, y]);

    }

    public bool IsSpecialTile(char tile)
    {
        if ((tile == ' ') || (tile == '[') || (tile == '\\') || (tile == ']') || (tile == '^'))
        {
            return true;
        }

        return false;
    }

    public bool isValidTile(char tile)
    {
        // *
        if (tile == 42)
        {
            return true;
        }

        // 'A' to 'Z'
        if ((tile >= 65) && (tile <= 90))
        {
            return true;
        }

        return false;
    }

    public (int x, int y) GetXYCoordinate(string coordinate)
    {
        // check this and possibly swap string around to find letter
        int character = coordinate[0];

        int x_coordinate = 0;

        if ((character >= 65) && (character <= 79))
        {
            // A -> O, 0 -> 14
            x_coordinate = character - 65;
        }
        else
        {
            // what should I do instead of throwing?
            throw new Exception("Invalid board position");
        }

        StringBuilder sb = new StringBuilder(coordinate).Remove(0, 1);

        int y_coordinate = Convert.ToInt32(sb.ToString()) - 1;

        return new(x_coordinate, y_coordinate);
    }

    private bool tryGetAdditionalMoveScore(ScrabbleMove adjMove, out int score)
    {
        score = 0;
        int doubleWord = 0;
        int tripleWord = 0;

        // how should I validate this move?
        // if (!tryIsValidMove(scrabbleMove, playerTiles, out ScrabbleMetaMove? metaMove))
        // {
        //     return false;
        // }

        if (adjMove.Direction)
        {
            // direction ==  true: across
            for (int i = 0; i < adjMove.Word.Length; i++)
            {
                if (!tryGetMoveScoreHelper(GetTileChar(adjMove.X_coordinate + i, adjMove.Y_coordinate), adjMove.Word[i], out int dub, out int tri, out bool isLetterTile, out int subScore))
                {
                    score = 0;
                    return false;
                }

                score += subScore;
                doubleWord += dub;
                tripleWord += tri;
            }
        }
        else
        {
            // direction ==  false: down
            for (int i = 0; i < adjMove.Word.Length; i++)
            {
                if(!tryGetMoveScoreHelper(GetTileChar(adjMove.X_coordinate, adjMove.Y_coordinate + i), adjMove.Word[i], out int dub, out int tri, out bool isLetterTile, out int subScore))
                {
                    score = 0;
                    return false;
                }

                score += subScore;
                doubleWord += dub;
                tripleWord += tri;
            }
        }

        // multiply (double/triple)
        for (int i = doubleWord; i > 0; i--)
        {
            score = score * 2;
        }

        for (int i = tripleWord; i > 0; i--)
        {
            score = score * 3;
        }

        return true;
    }

    // will do some limited validation of move here
    // return false if move is invalid
    private bool tryGetMoveScoreHelper(char tileOnBoard, char wordCharacter, out int doubleWord, out int tripleWord, out bool boardSpaceOccupied, out int score)
    {
        score = 0;
        doubleWord = 0;
        tripleWord = 0;
        boardSpaceOccupied = false;

        // DL('[') , DW('\'), TL(']'), TW('^')
        switch (tileOnBoard)
        {
            case '[':
                int newCharVal = getTilePointValue(wordCharacter);
                newCharVal *= 2;
                score += newCharVal;
                break;
            case '\\':
                doubleWord++;
                score += getTilePointValue(wordCharacter);
                break;
            case ']':
                int newCharVal3 = getTilePointValue(wordCharacter);
                newCharVal3 *= 3;
                score += newCharVal3;
                break;
            case '^':
                tripleWord++;
                score += getTilePointValue(wordCharacter);
                break;
            case ' ': // tile on board is blank
                score += getTilePointValue(wordCharacter);
                break;
            default: // if valid move, should be occupied by the same tile as word character
                if (!IsSameLetter(tileOnBoard, wordCharacter)) { return false; }

                score += getTilePointValue(wordCharacter);
                boardSpaceOccupied = true;
                break;
        }

        return true;
    }

    public bool TryGetMoveScore(ScrabbleBase mainMove, string word, string playerTiles, out int score)
    {
        score = 0;
        int doubleWord = 0;
        int tripleWord = 0;
        var sb = new StringBuilder(7);

        if (!tryIsValidMove(new ScrabbleMove(mainMove) { Word = word }, playerTiles, out ScrabbleMetaMove? metaMove))
        {
            return false;
        }

        if (mainMove.Direction)
        {
            // direction ==  true: across
            for (int i = 0; i < word.Length; i++)
            {
                var boardChar = GetTileChar(mainMove.X_coordinate + i, mainMove.Y_coordinate);

                if (!tryGetMoveScoreHelper(boardChar, word[i], out int dub, out int tri, out bool occupied, out int subScore))
                {
                    score = 0;
                    return false;
                }

                score += subScore;
                doubleWord += dub;
                tripleWord += tri;

                // the boardChar is a special or blank open position, so using tile from player rack
                if (!occupied)
                {
                    sb.Append(word[i]);
                }
            }
        }
        else
        {
            // direction ==  false: down
            for (int i = 0; i < word.Length; i++)
            {
                char boardChar = GetTileChar(mainMove.X_coordinate, mainMove.Y_coordinate + i);

                if (!tryGetMoveScoreHelper(boardChar, word[i], out int dub, out int tri, out bool occupied, out int subScore))
                {
                    score = 0;
                    return false;
                }

                score += subScore;
                doubleWord += dub;
                tripleWord += tri;

                // the boardChar is a special or blank open position, so using tile from player rack
                if (!occupied)
                {
                    sb.Append(word[i]);
                }
            }
        }

        // multiply (double/triple)
        for (int i = doubleWord; i > 0; i--)
        {
            score = score * 2;
        }

        for (int i = tripleWord; i > 0; i--)
        {
            score = score * 3;
        }

        // does this main move have additional moves?
        if (metaMove is not null)
        {
            foreach (var addMove in metaMove.GetAdditionalMoves())
            {
                if (!tryGetAdditionalMoveScore(addMove, out int subScore)) { return false; }

                score += subScore;
            }
        }

        // check for 50 point at the very end
        if (sb.Length == 7)
        {
            score += 50;
        }

        return true;
    }

    // assume that this meta move is already valid
    // already has possible additional moves as part of metaMove
    public int GetMoveScore(ScrabbleMetaMove metaMove, string playerTiles)
    {
        int score = 0;
        int doubleWord = 0;
        int tripleWord = 0;
        var playerTilesUsed = new StringBuilder(7);

        var mainMove = metaMove.GetMainMove();

        if (mainMove.Direction)
        {
            // direction ==  true: across
            for (int i = 0; i < mainMove.Word.Length; i++)
            {
                var boardChar = GetTileChar(mainMove.X_coordinate + i, mainMove.Y_coordinate);

                score += getMoveScoreHelper(boardChar, mainMove.Word[i], out int dub, out int tri, out bool occupied);

                doubleWord += dub;
                tripleWord += tri;

                // the boardChar is a special or blank open position, so using tile from player rack
                if (!occupied)
                {
                    playerTilesUsed.Append(boardChar);
                }
            }
        }
        else
        {
            // direction ==  false: down
            for (int i = 0; i < mainMove.Word.Length; i++)
            {
                char boardTile = GetTileChar(mainMove.X_coordinate, mainMove.Y_coordinate + i);

                score += getMoveScoreHelper(boardTile, mainMove.Word[i], out int dub, out int tri, out bool occupied);

                doubleWord += dub;
                tripleWord += tri;

                // the boardChar is a special or blank open position, so using tile from player rack
                if (!occupied)
                {
                    playerTilesUsed.Append(mainMove.Word[i]);
                }
            }
        }

        // multiply (double/triple)
        for (int i = doubleWord; i > 0; i--)
        {
            score = score * 2;
        }

        for (int i = tripleWord; i > 0; i--)
        {
            score = score * 3;
        }

        // does this move have additional moves?
        var additionalMove = metaMove.GetAdditionalMoves();

        foreach (var move in additionalMove)
        {
            score += getAdditionalMoveScore(move);
        }

        // check for 50 point at the very end
        if (playerTilesUsed.Length == 7)
        {
            score += 50;
        }

        return score;
    }

    private int getMoveScoreHelper(char tileOnBoard, char wordCharacter, out int doubleWord, out int tripleWord, out bool isBoardSpaceOccupied)
    {
        int score = 0;
        doubleWord = 0;
        tripleWord = 0;
        isBoardSpaceOccupied = false;

        // DL('[') , DW('\'), TL(']'), TW('^')
        switch (tileOnBoard)
        {
            case '[':
                int newCharVal = getTilePointValue(wordCharacter);
                newCharVal *= 2;
                score += newCharVal;
                break;
            case '\\':
                doubleWord++;
                score += getTilePointValue(wordCharacter);
                break;
            case ']':
                int newCharVal3 = getTilePointValue(wordCharacter);
                newCharVal3 *= 3;
                score += newCharVal3;
                break;
            case '^':
                tripleWord++;
                score += getTilePointValue(wordCharacter);
                break;
            case ' ': // tile on board is blank
                score += getTilePointValue(wordCharacter);
                break;
            default: // occupied by the same tile as word character
                score += getTilePointValue(wordCharacter);
                isBoardSpaceOccupied = true;
                break;
        }

        return score;
    }

    private int getAdditionalMoveScore(ScrabbleMove adjMove)
    {
        int score = 0;
        int doubleWord = 0;
        int tripleWord = 0;
        var playerTilesUsed = new StringBuilder(7);

        if (adjMove.Direction)
        {
            // direction ==  true: across
            for (int i = 0; i < adjMove.Word.Length; i++)
            {
                var boardChar = GetTileChar(adjMove.X_coordinate + i, adjMove.Y_coordinate);

                score += getMoveScoreHelper(boardChar, adjMove.Word[i], out int dub, out int tri, out bool occupied);

                doubleWord += dub;
                tripleWord += tri;

                // the boardChar is a special or blank open position, so using tile from player rack
                if (!occupied)
                {
                    playerTilesUsed.Append(boardChar);
                }
            }
        }
        else
        {
            // direction ==  false: down
            for (int i = 0; i < adjMove.Word.Length; i++)
            {
                char boardTile = GetTileChar(adjMove.X_coordinate, adjMove.Y_coordinate + i);

                score += getMoveScoreHelper(boardTile, adjMove.Word[i], out int dub, out int tri, out bool occupied);

                doubleWord += dub;
                tripleWord += tri;

                // the boardChar is a special or blank open position, so using tile from player rack
                if (!occupied)
                {
                    playerTilesUsed.Append(adjMove.Word[i]);
                }
            }
        }

        // multiply (double/triple)
        for (int i = doubleWord; i > 0; i--)
        {
            score = score * 2;
        }

        for (int i = tripleWord; i > 0; i--)
        {
            score = score * 3;
        }

        return 0;
    }

    private int getTilePointValue(char tile)
    {
        if ((tile >= 65) && (tile <= 90))
        {
            return ScrabbleWordGenerator.GetTilePointValue(tile);
        }
        else // should I check that it is indeed a lower case char?
        {
            return 0;
        }
    }

    //A-Z and DL('[') , DW('\'), TL(']'), TW('^')
    private Dictionary<string, char> bonusTiles = new Dictionary<string, char>
    {
        {"A1", '^' },
        {"A8", '^' },
        {"A15", '^' },
        {"H1", '^' },
        {"H15", '^' },
        {"O1", '^' },
        {"O8", '^' },
        {"O15", '^' },
        {"B6", ']'},
        {"B10", ']'},
        {"F2", ']'},
        {"F6", ']'},
        {"F10", ']'},
        {"F14", ']'},
        {"J2", ']'},
        {"J6", ']'},
        {"J10", ']'},
        {"J14", ']'},
        {"N6", ']'},
        {"N10", ']' },
        {"A4", '['},
        {"A12", '['},
        {"C7", '['},
        {"C9", '['},
        {"D1", '['},
        {"D8", '['},
        {"D15", '['},
        {"G3", '['},
        {"G7", '['},
        {"G9", '['},
        {"G13", '['},
        {"H4", '['},
        {"H12", '['},
        {"I3", '['},
        {"I7", '['},
        {"I9", '['},
        {"I13", '['},
        {"L1", '['},
        {"L8", '['},
        {"L15", '['},
        {"M7", '['},
        {"M9", '['},
        {"O4", '['},
        {"O12", '['},
    };

    private char[,] createBoard()
    {
        int i = 0;
        var tiles = new char[15, 15];

        for (int j = 0; j < 15; j++)
        {
            for (int k = 0; k < 15; k++)
            {
                tiles[j, k] = ' ';
            }
        }

        // Fill DW
        for (; i < 15; i++)
        {
            tiles[i, i] = '\\';
        }

        for (; i > 0; i--)
        {
            tiles[i - 1, 15 - i] = '\\';
        }

        // center tile
        //tiles[7, 7] = (char)9733;

        // go through hashmap
        // fill TW and TL
        foreach (var tile in bonusTiles)
        {
            var coordinates = GetXYCoordinate(tile.Key);

            tiles[coordinates.Item1, coordinates.Item2] = tile.Value;
        }

        return tiles;
    }

    private List<char> createScrabbleBag()
    {
        var scrabbleBag = new List<char>();
        var dist = ScrabbleWordGenerator.LetterDistList;

        //using [0] for blank space tile,convert to *
        scrabbleBag.Add('*');
        scrabbleBag.Add('*');

        for (int i = 1; i < dist.Count; i++)
        {
            for (int j = dist[i]; j > 0; j--)
            {
                scrabbleBag.Add((char)(64 + i));
            }
        }

        return scrabbleBag;
    }

    public ScrabbleMetaMove GetLastMove()
    {
        return metaMoves.Last();
    }

    public List<ScrabbleMetaMove> GetAllMoves()
    {
        return metaMoves;
    }

    // compare lowercase to uppercase
    public bool IsSameLetter(char boardTile, char rackTile)
    {
        if ((boardTile > 96) && (boardTile < 123)) // boardTile is lowercase
        {
            if ((rackTile > 96) && (rackTile < 123)) // rackTile is lowercase
            {
                if (boardTile == rackTile)
                {
                    return true;
                }

                return false;
            }
            else if ((rackTile > 64) && (rackTile < 91)) // rackTile is uppercase
            {
                if (boardTile - 32 == rackTile)
                {
                    return true;
                }

                return false;
            }
        }
        else if ((boardTile > 64) && (boardTile < 91)) // boardTile is uppercase
        {
            if ((rackTile > 96) && (rackTile < 123)) // rackTile is lowercase
            {
                if (rackTile - 32 == boardTile)
                {
                    return true;
                }

                return false;
            }
            else if ((rackTile > 64) && (rackTile < 91)) // rackTile is uppercase
            {
                if (boardTile == rackTile)
                {
                    return true;
                }

                return false;
            }
        }

        return false;
    }

    public bool IsWordInBounds(int wordLength, bool direction, int x, int y)
    {
        // Does the word go out of bounds?
        if (direction)
        {
            // across
            if (!IsValidCoordinate(x + wordLength - 1, y))
            {
                return false;
            }
        }
        else
        {
            // down
            if (!IsValidCoordinate(x, y + wordLength - 1))
            {
                return false;
            }
        }
        return true;
    }

    private bool tryIsValidFirstMove(ScrabbleMove move)
    {
        if (move.Direction)
        {
            // if word is written in across direction, 
            // coordinate must be (x,7) aka must occupy center row
            if (move.Y_coordinate != 7)
            {
                return false;
            }

            // check if the move contains the center square H8 (aka (7,7))
            if ((move.X_coordinate - 7 > 0) || (move.Word.Length + move.X_coordinate < 7))
            {
                return false;
            }

        }
        else
        {
            // if word is written in down direction, 
            // coordinate must be (7,y) aka must occupy center column
            if (move.X_coordinate != 7)
            {
                return false;
            }

            // check if the move contains the center square H8 (aka (7,7))
            if ((move.Y_coordinate - 7 > 0) || (move.Word.Length + move.Y_coordinate < 7))
            {
                return false;
            }
        }

        return true;
    }

    private bool tryIsValidMove(ScrabbleMove move, string playerTiles, out ScrabbleMetaMove? scrabbleMetaMove)
    {
        // Is the starting coordinate out of bounds?
        if (!IsValidCoordinate(move.X_coordinate, move.Y_coordinate))
        {
            scrabbleMetaMove = null;
            return false;
        }

        // Does the word go out of bounds?
        if (!IsWordInBounds(move.Word.Length, move.Direction, move.X_coordinate, move.Y_coordinate))
        {
            scrabbleMetaMove = null;
            return false;
        }

        // The first move must contain center square H8
        if ((metaMoves.Count == 0) && !tryIsValidFirstMove(move))
        {
            Console.WriteLine();
            Console.WriteLine("First move does not contain center square");
            scrabbleMetaMove = null;
            return false;
        }

        // Is there an unhandled character? (I may already handle this elsewhere but maybe move here)
        for (int i = 0; i < move.Word.Length; i++)
        {
            if (!isValidTile(move.Word[i]))
            {
                Console.WriteLine();
                Console.WriteLine("Invalid char: " + move.Word[i]);
                scrabbleMetaMove = null;
                return false;
            }
        }

        // Is this move going to attempt to overwrite tiles already on the board
        var boardTilesSb = new StringBuilder();

        if (!move.Direction)
        {
            int y_offset = move.Y_coordinate;

            for (int i = 0; i < move.Word.Length; i++)
            {
                // check if position is already taken
                if (!IsSpecialTile(board[move.X_coordinate, y_offset]))
                {
                    // if this position is already occupied by the same letter than I am not overwriting
                    // I'm merely using a board letter in my word
                    if (!IsSameLetter(board[move.X_coordinate, y_offset], move.Word[i]))
                    {
                        // can't overwrite a letter
                        scrabbleMetaMove = null;
                        return false;
                    }
                    else
                    {
                        boardTilesSb.Append(move.Word[i]);
                    }
                }

                y_offset++;
            }
        }
        else
        {
            int x_offset = move.X_coordinate;

            for (int i = 0; i < move.Word.Length; i++)
            {
                // check if position is already taken
                if (!IsSpecialTile(board[x_offset, move.Y_coordinate]))
                {
                    // if this position is already occupied by the same letter than I am not overwriting
                    // I'm merely using a board letter in my word
                    if (!IsSameLetter(board[x_offset, move.Y_coordinate], move.Word[i]))
                    {
                        // can't overwrite a letter
                        scrabbleMetaMove = null;
                        return false;
                    }
                    else
                    {
                        boardTilesSb.Append(move.Word[i]);
                    }

                }

                x_offset++;
            }
        }

        // Is this move attempting to use tiles that are not in their rack nor on the board
        var playerTilesList = playerTiles.ToList();
        var boardTilesList = boardTilesSb.ToString().ToList();

        var blankIndexes = new List<int>(2);

        for (int i = 0; i < move.Word.Length; i++)
        {
            if (playerTilesList.Contains(move.Word[i]))
            {
                playerTilesList.Remove(move.Word[i]);
            }
            else if (boardTilesList.Contains(move.Word[i]))
            {
                boardTilesList.Remove(move.Word[i]);
            }
            else if (playerTilesList.Contains('*'))
            {
                blankIndexes.Add(i);
                playerTilesList.Remove('*');
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Move is not valid. Could not find the tile " + move.Word[i] + " on player rack or on the board");
                scrabbleMetaMove = null;
                return false;
            }
        }

        if (blankIndexes.Count > 2)
        {
            throw new Exception("Something went wrong, there cannot be more than 2 blank/wildcard tiles in a game");
        }

        if (blankIndexes.Count > 0)
        {
            var sb = new StringBuilder(move.Word);

            foreach (var blankIndex in blankIndexes)
            {
                sb[blankIndex] = ScrabbleWordGenerator.ConvertUpperToLowerCase(move.Word[blankIndex]);
            }

            move.Word = sb.ToString();
        }

        // Input word is a valid word in my dictionary (is this necessary?)
        // I should do this outside     
        if (!generator.CheckDictionary(move.Word))
        {
            Console.WriteLine();
            Console.WriteLine(move.Word + " is not a valid word");
            scrabbleMetaMove = null;
            return false;
        }

        // Check adjacent tiles for additional words (are they valid?)
        if (tryGetNewAdjacentWords(move, out ScrabbleMetaMove metaMove))
        {
            // // all additionalMove words should already be validated
            // // if no addtionalMoves found exits normally with true
            // foreach (ScrabbleMove adjMove in metaMove.GetAdditionalMoves())
            // {
            //     // calculate adjacent moves scores, add to metaMove
            //     adjMove.Score = GetMoveScore(adjMove, adjMove.Word, playerTiles);

            //     // will calculate the total score outside of this function, probably in AddWord()
            // }

            scrabbleMetaMove = metaMove;
        }
        // at least one invalid adjacent word was found
        else
        {
            scrabbleMetaMove = null;
            return false;
        }

        return true;
    }

    // return true if new adjacent words are valid or if no adjacent words were found
    // return false if at least one new adjacent word is not found in the dictionary
    private bool tryGetNewAdjacentWords(ScrabbleMove playerMove, out ScrabbleMetaMove metaMove)
    {
        metaMove = new ScrabbleMetaMove(playerMove);

        // look for additional words coming from playerMove
        // - only create a lane if adjacent square is not a blank space

        // across
        if (playerMove.Direction)
        {
            // loop above and below the word
            // shouldn't reach out of bounds on the scrabble board here because I do a check in tryIsValidMove
            for (int i = 0; i < playerMove.Word.Length; i++)
            {
                // "walk" down the lane and get every tile already on the board in that lane   
                var potentialWordLoop = new StringBuilder();
                int potentialWordLoopYCoord = playerMove.Y_coordinate;

                // stop either at the first blank tile or the end of the board
                for (int j = playerMove.Y_coordinate - 1; j >= 0; j--)
                {
                    char thisChar = board[playerMove.X_coordinate + i, j];

                    if (IsSpecialTile(thisChar))
                    {
                        break;
                    }

                    potentialWordLoop.Insert(0, thisChar);
                    potentialWordLoopYCoord--;
                }

                potentialWordLoop.Append(playerMove.Word[i]);

                // look down the lane
                for (int j = playerMove.Y_coordinate + 1; j < 15; j++)
                {
                    char thisChar = board[playerMove.X_coordinate + i, j];

                    if (IsSpecialTile(thisChar))
                    {
                        break;
                    }

                    potentialWordLoop.Append(thisChar);
                }

                // if there are empty space above and below, do not add to potentialWordsList
                if (potentialWordLoop.Length <= 1)
                {
                    continue;
                }

                string potentialWordLoopString = potentialWordLoop.ToString();

                if (!generator.CheckDictionary(potentialWordLoopString))
                {
                    return false;
                }

                var potentialAdditionalMove = new ScrabbleMove
                {
                    Word = potentialWordLoopString,
                    Direction = false,
                    X_coordinate = playerMove.X_coordinate + i,
                    Y_coordinate = potentialWordLoopYCoord,
                };

                if (isWordOnBoard(potentialAdditionalMove))
                {
                    // adjacent word is not a new word, its already on the board
                    continue;
                }

                metaMove.AddAddtionalMove(potentialAdditionalMove);
            }

            var potentialWord = new StringBuilder();
            int potentialWordXCoord = playerMove.X_coordinate;

            if (playerMove.X_coordinate == 0)
            {
                // can't look left, already reached end of board
            }
            else
            {
                // peak the left side of the move
                if (!IsOpenSpace(playerMove.X_coordinate - 1, playerMove.Y_coordinate))
                {
                    // if not empty keep looking until end of board
                    for (int i = playerMove.X_coordinate - 1; i >= 0; i--)
                    {
                        char currentChar = board[i, playerMove.Y_coordinate];

                        if (IsSpecialTile(currentChar))
                        {
                            // no more tiles on the board for potentialWord, 
                            // don't reach end of the board
                            break;
                        }

                        potentialWord.Insert(0, currentChar);
                        potentialWordXCoord--;
                    }
                }
            }


            // word should be in the middle when there are tiles to the left and right
            // word should be on the right (end) if there are only tiles on the left
            // word should be on the left (begining) if there are only tiles on the right
            potentialWord.Append(playerMove.Word);

            // question: is it worth inserting a character into stringbuilder, compared to building a char array backwards and reversing it

            // peak to the right
            int endOfPlayerMoveX = playerMove.X_coordinate + playerMove.Word.Length - 1;

            if (endOfPlayerMoveX == 14)
            {
                // can't look right, alread reached the end of the board
            }
            else
            {
                endOfPlayerMoveX++;

                if (!IsOpenSpace(endOfPlayerMoveX, playerMove.Y_coordinate))
                {
                    // if not empty keep looking until end of board or until empty
                    for (int i = endOfPlayerMoveX; i < 15; i++)
                    {
                        char currentChar = board[i, playerMove.Y_coordinate];

                        if (IsSpecialTile(currentChar))
                        {
                            break;
                        }

                        potentialWord.Append(currentChar);
                    }
                }
            }

            string potentialWordString = potentialWord.ToString();

            if (!generator.CheckDictionary(potentialWordString))
            {
                return false;
            }

            if (potentialWordString != playerMove.Word)
            {
                var potentialMove = new ScrabbleMove
                {
                    Word = potentialWordString,
                    Direction = true,
                    Y_coordinate = playerMove.Y_coordinate,
                    X_coordinate = potentialWordXCoord,
                };

                if (!isWordOnBoard(potentialMove))
                {
                    metaMove.AddAddtionalMove(potentialMove);
                }
            }
        }
        else // playerMove direction is down
        {
            // copied and pasted from above and changed to the perpendicular situation
            // need to verify both are functioning as expected

            // loop to the left and right of the word
            // shouldn't reach out of bounds on the scrabble board here because I do a check in tryIsValidMove
            for (int i = 0; i < playerMove.Word.Length; i++)
            {
                // "walk" down the lane and get every tile already on the board in that lane   
                var potentialWordLoop = new StringBuilder();
                int potentialWordLoopXCoord = playerMove.X_coordinate;

                // stop either at the first blank tile or the end of the board
                for (int j = playerMove.X_coordinate - 1; j >= 0; j--)
                {
                    char thisChar = board[j, playerMove.Y_coordinate + i];

                    if (IsSpecialTile(thisChar))
                    {
                        break;
                    }

                    potentialWordLoop.Insert(0, thisChar);
                    potentialWordLoopXCoord--;
                }

                potentialWordLoop.Append(playerMove.Word[i]);

                // look down the lane
                for (int j = playerMove.X_coordinate + 1; j < 15; j++)
                {
                    char thisChar = board[j, playerMove.Y_coordinate + i];

                    if (IsSpecialTile(thisChar))
                    {
                        break;
                    }

                    potentialWordLoop.Append(thisChar);
                }

                // if there are empty spaces to the left and to the right, do not add this loop to potentialWordsList
                if (potentialWordLoop.Length <= 1)
                {
                    continue;
                }

                string potentialWordLoopString = potentialWordLoop.ToString();

                if (!generator.CheckDictionary(potentialWordLoopString))
                {
                    return false;
                }

                var potentialAdditionalMove = new ScrabbleMove
                {
                    Word = potentialWordLoopString,
                    Direction = true,
                    X_coordinate = potentialWordLoopXCoord,
                    Y_coordinate = playerMove.Y_coordinate + i,
                };

                if (isWordOnBoard(potentialAdditionalMove))
                {
                    // adjacent word is not a new word, its already on the board
                    continue;
                }

                metaMove.AddAddtionalMove(potentialAdditionalMove);
            }

            var potentialWord = new StringBuilder();
            int potentialWordYCoord = playerMove.Y_coordinate;

            if (potentialWordYCoord > 0)
            {
                // peak above the move
                if (!IsOpenSpace(playerMove.X_coordinate, playerMove.Y_coordinate - 1))
                {
                    // if not empty keep looking until end of board
                    for (int i = playerMove.Y_coordinate - 1; i >= 0; i--)
                    {
                        char currentChar = board[playerMove.X_coordinate, i];

                        if (IsSpecialTile(currentChar))
                        {
                            // no more tiles on the board for potentialWord, 
                            // don't reach end of the board
                            break;
                        }

                        potentialWord.Insert(0, currentChar);
                        potentialWordYCoord--;
                    }
                }
            }

            // word should be in the middle when there are tiles to the left and right
            // word should be on the right (end) if there are only tiles on the left
            // word should be on the left (begining) if there are only tiles on the right
            potentialWord.Append(playerMove.Word);

            int endOfPlayerMoveY = playerMove.Y_coordinate + playerMove.Word.Length - 1; // does this math make sense?

            if (endOfPlayerMoveY < 14)
            {
                endOfPlayerMoveY++;

                if (!IsOpenSpace(playerMove.X_coordinate, endOfPlayerMoveY))
                {
                    // I think I could make this more efficient, aka not accessing the first char twice

                    // if not empty keep looking until end of board or until empty
                    for (int i = endOfPlayerMoveY; i < 15; i++)
                    {
                        char currentChar = board[playerMove.X_coordinate, i];

                        if (IsSpecialTile(currentChar))
                        {
                            break;
                        }

                        potentialWord.Append(currentChar);
                    }
                }
            }

            string potentialWordString = potentialWord.ToString();

            if (!generator.CheckDictionary(potentialWordString))
            {
                return false;
            }

            if (potentialWordString != playerMove.Word)
            {
                var potentialMove = new ScrabbleMove
                {
                    Word = potentialWord.ToString(),
                    Direction = false,
                    X_coordinate = playerMove.X_coordinate,
                    Y_coordinate = potentialWordYCoord,
                };

                if (!isWordOnBoard(potentialMove))
                {
                    metaMove.AddAddtionalMove(potentialMove);
                }
            }
        }

        return true;
    }

    private void addWordToHashSet(ScrabbleMove scrabbleMove)
    {
        var key = new StringBuilder(scrabbleMove.Word);
        key.Append('_');
        key.Append(scrabbleMove.X_coordinate);
        key.Append('_');
        key.Append(scrabbleMove.Y_coordinate);
        key.Append('_');
        key.Append(scrabbleMove.Direction);

        boardWords.Add(key.ToString());
    }

    private bool isWordOnBoard(ScrabbleMove scrabbleMove)
    {
        var key = new StringBuilder(scrabbleMove.Word);
        key.Append('_');
        key.Append(scrabbleMove.X_coordinate);
        key.Append('_');
        key.Append(scrabbleMove.Y_coordinate);
        key.Append('_');
        key.Append(scrabbleMove.Direction);

        return boardWords.Contains(key.ToString());
    }

    public bool IsValidCoordinate(int x, int y)
    {
        if ((x < 0) || (x > 14) || (y < 0) || (y > 14))
        {
            return false;
        }

        return true;
    }

    public bool TryGetXYCoordinate(string coordinate, out int x, out int y)
    {
        x = int.MinValue;
        y = int.MinValue;

        // either expecting a 2 or 3 length string
        // examples: H8 or C13 
        if ((coordinate.Length > 3) || (coordinate.Length < 2))
        {
            return false;
        }

        char xCHar = coordinate[0];

        // capitalize letter
        // will accept h8 or c13 for example
        char capitalX = ScrabbleWordGenerator.ConverLowerToUpper(xCHar);

        if ((capitalX < 65) || (capitalX > 79))
        {
            return false;
        }

        x = capitalX - 65;

        var numberString = new StringBuilder(coordinate).Remove(0, 1).ToString();

        // ASCII 0-9 is decimal 48-57
        if (numberString.Length == 2)
        {
            // if there are 2 digits the first should be 1

            if (numberString[0] != '1')
            {
                return false;
            }

            y = 10;

            char numberChar = numberString[1];

            // // max number is 15, min 2 digit number is 10
            if ((numberChar < 48) || (numberChar > 53))
            {
                return false;
            }

            y = y + (int)(numberChar - 48) - 1;
        }
        else if (numberString.Length == 1)
        {
            char numberChar = numberString[0];

            // // max number is 15, min 2 digit number is 10
            if ((numberChar < 48) || (numberChar > 57))
            {
                return false;
            }

            y = numberChar - 48 - 1;
        }
        else
        {
            // string length is not correct
            // since I've checked length before i shouldn't ever reach this
            return false;
        }

        return true;
    }

    // I could determine the tiles that each player has but why I waste time calculating that?
    public bool AreThereAnyPossibleMovesLeft(IPlayer player1, IPlayer player2)
    {


        return true;
    }
}

