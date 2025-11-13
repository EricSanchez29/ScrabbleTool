using System.Text;
using System.Xml.Schema;

public class ScrabbleBoard
{
    public ScrabbleBoard()
    {
        board = getBoard();
        bag = getScrabbleBag();
        rando = new Random();
    }

    // Scrabble board
    private char[,] board;

    // Scrabble bag
    private List<char> bag;

    // Persistent instance of Random
    private Random rando;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="word"> "" </param>
    /// <param name="coordinate"> A1 - O15 </param>
    /// <param name="direction"> across or down</param>
    /// <returns></returns>
    public void AddWord(string word, string coordinate, string direction = "across")
    {
        // convert from Scrabble coordinate system to array coordinates
        var startingPosition = getBoardPosition(coordinate);

        if (direction == "down")
        {
            for (int i = 0; i < word.Length; i++)
            {


                // check if position is already taken
                if (' ' != board[startingPosition.x, startingPosition.y])
                {
                    Console.WriteLine("Invalid play, board position already occupied");
                    return;
                }


                board[startingPosition.x, startingPosition.y] = word[i];

                startingPosition.y++;
            }
        }
        else if (direction == "across")
        {
            for (int i = 0; i < word.Length; i++)
            {
                // check if position is already taken
                if (' ' != board[startingPosition.x, startingPosition.y])
                {
                    Console.WriteLine("Invalid play, board position already occupied");
                    return;
                }


                board[startingPosition.x, startingPosition.y] = word[i];

                startingPosition.x++;
            }
        }
        else
        {
            Console.WriteLine();
        }



       

        // // check if any potential position already has a tile
        // if (direction == "down")
        // {
        //     for (int i = 0; i < word.Length; i++)
        //     {
        //         // check if position is already taken
        //         if (' ' != board[startingPosition.Item1, startingPosition.Item2 - i])
        //         {
        //             Console.WriteLine("Invalid play, board position already occupied");
        //             return;
        //         }

        //         board
        //     }
        // }
        // else if (direction == "across")
        // {

        // }
        // else
        // {
        //     Console.WriteLine("Invalid direction");
        // }


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

    private (int x, int y) getBoardPosition(string coordinate)
    {
        // check this and possibly swap string around to find letter
        int character = coordinate[0];

        int x_coordinate = 0;

        if ((character >= 65) && (character <= 79))
        {
            // A -> 0, O-> 14
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

    private char[,] getBoard()
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
            if (i == 7)
            {
                continue;
            }

            tiles[i, i] = '\\';
        }

        for (; i > 0; i--)
        {
            if (i == 8)
            {
                continue;
            }

            tiles[i - 1, 15 - i] = '\\';
        }

        // center tile
        //tiles[7, 7] = (char)9733;

        // go through hashmap
        // fill TW and TL
        foreach (var tile in bonusTiles)
        {
            var coordinates = getBoardPosition(tile.Key);

            tiles[coordinates.Item1, coordinates.Item2] = tile.Value;
        }

        return tiles;
    }

    private List<char> getScrabbleBag()
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

}

