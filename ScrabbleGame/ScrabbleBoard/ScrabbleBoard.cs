using System.Text;
using System.Xml.Schema;

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
    public string AddWord(ScrabbleMove playerMove, MoveContext moveContext)
    {
        var tilesToRemove = new StringBuilder();

        int totalScore = 0;

        if (!tryIsValidMove(playerMove, out ScrabbleMetaMove? metaMove))
        {
            moveContext.SetRetryMove(true);
        }

        // Calculate move score

        // calculate scores for additional moves if available
        if (metaMove is not null) // this move creates additional words besides Move.Word
        {
            foreach (var additionalMove in metaMove.GetAdditionalMoves())
            {
                var moveScore = GetMoveScore(additionalMove, additionalMove.Word);

                totalScore += moveScore;
            }
        }

        // calculate main move score
        playerMove.Score = GetMoveScore(playerMove, playerMove.Word);

        totalScore += playerMove.Score;


        // only manipulate tiles (output string) here
        if (!playerMove.Direction)
        {
            int y_offset = playerMove.Y_coordinate;

            for (int i = 0; i < playerMove.Word.Length; i++)
            {
                // check if position is already taken
                if (!isSpecialTile(board[playerMove.X_coordinate, y_offset]))
                {
                    // if this position is already occupied by the same letter than I am not overwriting
                    // I'm merely using a board letter in my word
                    if (board[playerMove.X_coordinate, y_offset] != playerMove.Word[i])
                    {
                        // shouldn't reach this if I am correctly checking this in tryIsValidMove()
                        Console.WriteLine("Invalid play, board position already occupied");
                        moveContext.SetRetryMove(true);
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
                if (!isSpecialTile(board[x_offset, playerMove.Y_coordinate]))
                {
                    // if this position is already occupied by the same letter than I am not overwriting
                    // I'm merely using a board letter in my word
                    if (board[x_offset, playerMove.Y_coordinate] != playerMove.Word[i])
                    {
                        // shouldn't reach this if I am correctly checking this in tryIsValidMove()
                        Console.WriteLine("Invalid play, board position already occupied");
                        moveContext.SetRetryMove(true);
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

        // check for bingo (+50 points)
        if (tilesToRemove.Length == 7)
        {
            totalScore += 50;
        }

        if (metaMove is null)
        {
            metaMove = new ScrabbleMetaMove(playerMove);
        }
        else
        {
            metaMove.SetTotalScore(totalScore);
        }

        metaMoves.Add(metaMove);

        // maybe write to console with total score?

        return tilesToRemove.ToString();
    }
    
    public void DisplayBoard()
    {
        Console.WriteLine();
        Console.WriteLine(" ");
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
                else if (tileValue == 91)
                {
                    Console.Write("DL");
                }
                else if (tileValue == 92)
                {
                    Console.Write("DW");
                }
                else if (tileValue == 93)
                {
                    Console.Write("TL");
                }
                else if (tileValue == 94)
                {
                    Console.Write("TW");
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
        return isSpecialTile(board[x, y]);

    }

    public bool isSpecialTile(char tile)
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
        if (tile != 42)
        {
            return false;
        }

        // 'A' to 'Z'
        if ((tile < 65) && (tile > 90))
        {
            return false;
        }

        return true;
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

    public int GetMoveScore(in ScrabbleBase move, string word)
    {
        int score = 0;

        // DoubleWord or TripleWord, need to consider theses values after everything else 

        int doubleWord = 0;
        int tripleWord = 0;

        // should I have less duplicate code, 
        // does it really matter once its converted into CLI?
        if (move.Direction)
        {
            // direction ==  true: across
            for (int i = 0; i < word.Length; i++)
            {
                // DL('[') , DW('\'), TL(']'), TW('^')

                char character = GetTileChar(move.X_coordinate + i, move.Y_coordinate);
                // could probably put this switch in a function
                // if I want to cut down on file size
                switch (character)
                {
                    case '[':
                        int newCharVal = ScrabbleWordGenerator.GetTilePointValue(word[i]);
                        newCharVal = newCharVal * 2;
                        score = score + newCharVal;
                        break;
                    case '\\':
                        doubleWord++;
                        score = score + ScrabbleWordGenerator.GetTilePointValue(word[i]);
                        break;
                    case ']':
                        int newCharVal3 = ScrabbleWordGenerator.GetTilePointValue(word[i]);
                        newCharVal3 = newCharVal3 * 3;
                        score = score + newCharVal3;
                        break;
                    case '^':
                        tripleWord++;
                        score = score + ScrabbleWordGenerator.GetTilePointValue(word[i]);
                        break;
                    default:
                        score = score + ScrabbleWordGenerator.GetTilePointValue(word[i]);
                        break;
                }
            }
        }
        else
        {
            // 
            // direction ==  false: down
            for (int i = 0; i < word.Length; i++)
            {
                // DL('[') , DW('\'), TL(']'), TW('^')

                char character = GetTileChar(move.X_coordinate, move.Y_coordinate + i);

                switch (character)
                {
                    case '[':
                        int newCharVal = ScrabbleWordGenerator.GetTilePointValue(word[i]);
                        newCharVal = newCharVal * 2;
                        score = score + newCharVal;
                        break;
                    case '\\':
                        doubleWord++;
                        score = score + ScrabbleWordGenerator.GetTilePointValue(word[i]);
                        break;
                    case ']':
                        int newCharVal3 = ScrabbleWordGenerator.GetTilePointValue(word[i]);
                        newCharVal3 = newCharVal3 * 3;
                        score = score + newCharVal3;
                        break;
                    case '^':
                        tripleWord++;
                        score = score + ScrabbleWordGenerator.GetTilePointValue(word[i]);
                        break;
                    default:
                        score = score + ScrabbleWordGenerator.GetTilePointValue(word[i]);
                        break;
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
            score = score * 2;
        }

        return score;


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

    // scrabbleMetaMove is only not null if this funcition returns true, is there a better way to do this?
    private bool tryIsValidMove(ScrabbleMove move, out ScrabbleMetaMove? scrabbleMetaMove)
    {
        // is the orignal coordinate out of bounds?
        if (!IsValidCoordinate(move.X_coordinate, move.Y_coordinate))
        {
            scrabbleMetaMove = null;
            return false;
        }

        // Does the word go out of bounds?
        if (move.Direction)
        {
            // across
            if (!IsValidCoordinate(move.X_coordinate + move.Word.Length - 1, move.Y_coordinate))
            {
                scrabbleMetaMove = null;
                return false;
            }
        }
        else
        {
            // down
            if (!IsValidCoordinate(move.X_coordinate, move.Y_coordinate + move.Word.Length - 1))
            {
                Console.WriteLine();
                Console.WriteLine("Word is out of bounds");
                scrabbleMetaMove = null;
                return false;
            }
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

// TO DO



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
            // check if these adjacent words are valid scrabble words
            foreach (ScrabbleMove adjMove in metaMove.GetAdditionalMoves())
            {
                if (!generator.CheckDictionary(adjMove.Word))
                {
                    // If any adjacent word is an invalid dictionary word
                    // then playerMove is an invalid move, return false
                    Console.WriteLine();
                    Console.WriteLine("The adjacent word: " + adjMove.Word + " is not a valid word");
                    scrabbleMetaMove = null;
                    return false;
                }

                // calculate adjacent moves scores, add to metaMove
                adjMove.Score = GetMoveScore(adjMove as ScrabbleBase, adjMove.Word);

                // will calculate the total score outside of this function, probaby in AddWord()
            }

            scrabbleMetaMove = metaMove;
        }
        // else no adjacent words can be formed, move on
        else
        {
            scrabbleMetaMove = null;
        }

        return true;
    }

    // need to thoroughly test both across and down since its confusing to read over
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
                var potentialWordLoop = new StringBuilder(playerMove.Word[i]);
                int potentialWordLoopYCoord = playerMove.Y_coordinate;

                // stop either at the first blank tile or the end of the board
                for (int j = playerMove.Y_coordinate - 1; j >= 0; j--)
                {
                    char thisChar = board[playerMove.X_coordinate + i, j];

                    if (isSpecialTile(thisChar))
                    {
                        break;
                    }

                    potentialWordLoop.Insert(0, thisChar);
                    potentialWordLoopYCoord--;
                }

                // look down the lane
                for (int j = playerMove.Y_coordinate + 1; j < 15; j++)
                {
                    char thisChar = board[playerMove.X_coordinate + i, j];

                    if (isSpecialTile(thisChar))
                    {
                        break;
                    }

                    potentialWordLoop.Append(thisChar);
                }

                // if there are empty space above and below, do not add to potentialWordsList
                if (potentialWordLoop.Length == 1)
                {
                    continue;
                }

                metaMove.AddAddtionalMove(new ScrabbleMove
                {
                    Word = potentialWordLoop.ToString(),
                    Direction = false,
                    X_coordinate = playerMove.X_coordinate + i,
                    Y_coordinate = potentialWordLoopYCoord,
                });
            }

            var potentialWord = new StringBuilder();
            int potentialWordXCoord = playerMove.X_coordinate;

            // peak the left side of the move
            if (!IsOpenSpace(playerMove.X_coordinate - 1, playerMove.Y_coordinate))
            {
                // if not empty keep looking until end of board
                for (int i = playerMove.X_coordinate - 1; i >= 0; i--)
                {
                    char currentChar = board[i, playerMove.Y_coordinate];

                    if (isSpecialTile(currentChar))
                    {
                        // no more tiles on the board for potentialWord, 
                        // don't reach end of the board
                        break;
                    }

                    potentialWord.Insert(currentChar, 0);
                    potentialWordXCoord--;
                }
            }

            // word should be in the middle when there are tiles to the left and right
            // word should be on the right (end) if there are only tiles on the left
            // word should be on the left (begining) if there are only tiles on the right
            potentialWord.Append(playerMove.Word);

            // question: is it word inserting a character into stringbuilder, compared to building a char array backwards and reversing it

            // peak to the right
            if (!IsOpenSpace(playerMove.X_coordinate + 1, playerMove.Y_coordinate))
            {
                // if not empty keep looking until end of board or until empty
                for (int i = playerMove.X_coordinate + 1; i < 15; i++)
                {
                    char currentChar = board[i, playerMove.Y_coordinate];

                    if (isSpecialTile(currentChar))
                    {
                        break;
                    }

                    potentialWord.Append(currentChar);
                }
            }

            if (potentialWord.ToString() != playerMove.Word)
            {
                metaMove.AddAddtionalMove(new ScrabbleMove
                {
                    Word = potentialWord.ToString(),
                    Direction = true,
                    Y_coordinate = playerMove.Y_coordinate,
                    X_coordinate = potentialWordXCoord,
                });
            }
        }
        else // down
        {
            // copied and pasted from above and changed to the perpendicular situation
            // need to verify both are functioning as expected

            // loop to the left and right of the word
            // shouldn't reach out of bounds on the scrabble board here because I do a check in tryIsValidMove
            for (int i = 0; i < playerMove.Word.Length; i++)
            {
                // "walk" down the lane and get every tile already on the board in that lane   

                var potentialWordLoop = new StringBuilder(playerMove.Word[i]);
                int potentialWordLoopXCoord = playerMove.X_coordinate;

                // stop either at the first blank tile or the end of the board
                for (int j = playerMove.X_coordinate - 1; j >= 0; j--)
                {
                    char thisChar = board[j, playerMove.Y_coordinate + i];

                    if (isSpecialTile(thisChar))
                    {
                        break;
                    }

                    potentialWordLoop.Insert(0, thisChar);
                    potentialWordLoopXCoord--;
                }

                // look down the lane
                for (int j = playerMove.X_coordinate + 1; j < 15; j++)
                {
                    char thisChar = board[j, playerMove.Y_coordinate + i];

                    if (isSpecialTile(thisChar))
                    {
                        break;
                    }

                    potentialWordLoop.Append(thisChar);
                }

                // if there are empty spaces to the left and to the right, do not add this loop to potentialWordsList
                if (potentialWordLoop.Length == 1)
                {
                    continue;
                }

                metaMove.AddAddtionalMove(new ScrabbleMove
                {
                    Word = potentialWordLoop.ToString(),
                    Direction = true,
                    X_coordinate = potentialWordLoopXCoord,
                    Y_coordinate = playerMove.Y_coordinate + i,
                });
            }

            var potentialWord = new StringBuilder();
            int potentialWordYCoord = playerMove.Y_coordinate;

            // peak above the move
            if (!IsOpenSpace(playerMove.X_coordinate, playerMove.Y_coordinate - 1))
            {
                // if not empty keep looking until end of board
                for (int i = playerMove.X_coordinate - 1; i >= 0; i--)
                {
                    char currentChar = board[i, playerMove.Y_coordinate];

                    if (isSpecialTile(currentChar))
                    {
                        // no more tiles on the board for potentialWord, 
                        // don't reach end of the board
                        break;
                    }

                    potentialWord.Insert(currentChar, 0);
                    potentialWordYCoord--;
                }
            }

            // word should be in the middle when there are tiles to the left and right
            // word should be on the right (end) if there are only tiles on the left
            // word should be on the left (begining) if there are only tiles on the right
            potentialWord.Append(playerMove.Word);

            // peak below
            if (!IsOpenSpace(playerMove.X_coordinate + 1, playerMove.Y_coordinate))
            {
                // if not empty keep looking until end of board or until empty
                for (int i = playerMove.X_coordinate + 1; i < 15; i++)
                {
                    char currentChar = board[i, playerMove.Y_coordinate];

                    if (isSpecialTile(currentChar))
                    {
                        break;
                    }

                    potentialWord.Append(currentChar);
                }
            }

            if (potentialWord.ToString() != playerMove.Word)
            {
                metaMove.AddAddtionalMove(new ScrabbleMove
                {
                    Word = potentialWord.ToString(),
                    Direction = false,
                    X_coordinate = playerMove.X_coordinate,
                    Y_coordinate = potentialWordYCoord,
                });
            }
        }

        return metaMove.HasAdditionalMoves();
    }

    //private bool is

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

        StringBuilder sb = new StringBuilder(coordinate).Remove(0, 1);

        y = Convert.ToInt32(sb.ToString()) - 1;

        if (y == 0)
        {
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

