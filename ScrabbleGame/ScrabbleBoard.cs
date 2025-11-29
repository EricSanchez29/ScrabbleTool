using System.Text;
using System.Xml.Schema;

public class ScrabbleBoard : IBoard, IDisplay
{
    public ScrabbleBoard()
    {
        board = createBoard();
        bag = createScrabbleBag();
        rando = new Random();
        playerMoves = new List<ScrabbleMove>();
    }

    // Scrabble board
    private char[,] board;

    // Scrabble bag
    private List<char> bag;

    // Persistent instance of Random
    private Random rando;

    // Player Moves
    private List<ScrabbleMove> playerMoves;

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

        AddWord(move);
    }

    // should another param with player letters, or maybe could track players letters?
    // also need to add better checks for occupied board space, need to take into account using letter already on board
    // and also add a bingo check +50 points
    public void AddWord(ScrabbleMove move)
    {
        // isValidMove()

        if (!move.Direction)
        {
            move.Score = GetMoveScore(move, move.Word);

            int y_offset = move.Y_coordinate;

            for (int i = 0; i < move.Word.Length; i++)
            {
                // check if position is already taken
                if (!isSpecialTile(board[move.X_coordinate, y_offset]))
                {
                    //Console.WriteLine("Invalid play, board position already occupied");
                    y_offset++;
                    continue;
                }

                board[move.X_coordinate, y_offset] = move.Word[i];

                y_offset++;
            }
        }
        else
        {
            move.Score = GetMoveScore(move, move.Word);

            int x_offset = move.X_coordinate;

            for (int i = 0; i < move.Word.Length; i++)
            {
                // check if position is already taken
                if (!isSpecialTile(board[x_offset, move.Y_coordinate]))
                {
                    Console.WriteLine("Invalid play, board position already occupied");
                    x_offset++;
                    continue;
                }

                board[x_offset, move.Y_coordinate] = move.Word[i];

                x_offset++;
            }

        }

        playerMoves.Add(move);
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

        for (int j = 0; j < 15; j++)
        {

            Console.WriteLine("");
            Console.Write(" ");
            Console.Write(j + 1);

            // whats this for?
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

    private bool isSpecialTile(char tile)
    {
        if ((tile == ' ') || (tile == '[') || (tile == '\\') || (tile == ']') || (tile == '^'))
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

    public string GetBoardPositionString(int x, int y)
    {
        string alphaNumeric = string.Empty;



        return alphaNumeric;
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


    //private void recordScrabbleMove(ScrabbleMove )

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


    public ScrabbleMove GetLastMove()
    {
        return playerMoves.Last();
    }

    private bool isValidMove(ScrabbleMove move)
    {
        // is the orignal coordinate out of bounds?

        // Does the word go out of bounds?

        // Is there an unhandled character? (I may already handle this elsewhere but maybe move here)

        // Is this a legal scrabble move?
        // - Input word is a valid word in my dictionary
        // - Check adjacent tiles for additional words (are they valid?)
        //
        return false;
    }

    public bool IsValidCoordinate(int x, int y)
    {
        if ((x < 0) || (x > 14) || (y < 0) || (y > 14))
        {
            return false;
        }

        return true;
    }

    // should I do a TryGetXYCoordinate(string, ref int x, ref int y)

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

}

