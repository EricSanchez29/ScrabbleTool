using System.Text;
using System.Xml.Schema;

public class ScrabbleBoard : IPLayer, IDisplay
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
        var startingPosition = GetBoardPosition(coordinate);

        ScrabbleMove move = new ScrabbleMove()
            {
                Direction = direction,
                Word = word,
                X_coordinate = startingPosition.x,
                Y_coordinate = startingPosition.y,
            };

        AddWord(move);
    }

    public void AddWord(ScrabbleMove move)
    {
        if (!move.Direction)
        {
            move.Points = GetMoveScore(move, move.Word);

            for (int i = 0; i < move.Word.Length; i++)
            {
                // check if position is already taken
                if (!isSpecialTile(board[move.X_coordinate, move.Y_coordinate]))
                {
                    Console.WriteLine("Invalid play, board position already occupied");
                    return;
                }

                board[move.X_coordinate, move.Y_coordinate] = move.Word[i];

                move.Y_coordinate++;
            }
        }
        else
        {
            move.Points = GetMoveScore(move, move.Word);

            for (int i = 0; i < move.Word.Length; i++)
            {
                // check if position is already taken
                if (!isSpecialTile(board[move.X_coordinate, move.Y_coordinate]))
                {
                    Console.WriteLine("Invalid play, board position already occupied");
                    return;
                }

                board[move.X_coordinate, move.Y_coordinate] = move.Word[i];

                move.X_coordinate++;
            }

        }

        playerMoves.Add(move);
    }
    public void DisplayBoard()
    {
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

    public List<char> DrawTiles(int tileCount)
    {
        var list = new List<char>(tileCount);

        for (int i = tileCount; i > 0; i--)
        {
            // draw random tiles from bag
            var randomNumber = rando.Next(0, bag.Count - 1);
            list.Add(bag[randomNumber]);
            bag.RemoveRange(randomNumber, 1);
        }

        return list;
    }

    public int GetBagCount()
    {
        return bag.Count();
    }

    public char GetTileChar(string coordinate)
    {
        var cartesian = GetBoardPosition(coordinate);
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

    public (int x, int y) GetBoardPosition(string coordinate)
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
                    case ' ':
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

                char character = GetTileChar(move.X_coordinate, move.Y_coordinate + 1);

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
            var coordinates = GetBoardPosition(tile.Key);

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
}

