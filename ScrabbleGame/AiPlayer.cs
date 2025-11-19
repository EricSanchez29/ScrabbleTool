using System.Text;

public class ArtificiallyUnintelligentPlayer
{
    public ArtificiallyUnintelligentPlayer(ScrabbleWordGenerator wordGenerator, IPLayer player)
    {
        generator = wordGenerator;
        scrabbleBoard = player;
    }

    private ScrabbleWordGenerator generator;

    private IPLayer scrabbleBoard;

    private List<ScrabbleLane> opponentMoves = new List<ScrabbleLane>();

    private List<ScrabbleLane> aiMoves = new List<ScrabbleLane>();

    private List<ScrabbleLane> openLanes = new List<ScrabbleLane>();

    private string centerSquare = "G8";
    public (string word_coordinate, bool direction) MakeMove(string playerTiles)
    {
        // Case 1: AI making the first move
        // if center is empty, no one has made a legal scrabble move yet
        if (scrabbleBoard.GetTileChar(centerSquare) == ' ')
        {
            // need to update lanes, and moves list
            return makeStartingMove(playerTiles);
        }

        // Case 2: Opponent made the first move, AI goes 2nd
        // find opponent first move
        findPlayersFirstMove(out string word, out int x, out int y, out bool direction);

        var oppFirstMove = new ScrabbleLane
        {
            Word = word,
            X_coordinate = x,
            Y_coordinate = y,
            Direction = direction
        };

        // calculate word score
        oppFirstMove.Points = calculateWordScore(oppFirstMove);

        // add to opp moves list
        opponentMoves.Add(oppFirstMove);

        // create lanes for empty list
        var newLanes = getLanesFromWord(oppFirstMove);

        // pick words for each lane (or just top lanes if too big)


        // choose word/lane with highest bonus points/overall score



        // update lanes list (remove at least one lane also might block other lanes with new word)



        // Case 3: this is the AI's 2nd move or later

        // to do

        // Look for previously open lanes that are now blocked
        // look through existing lanes
        // pick words for each lane (or just top lanes if too big)
        // choose word/lane with highest bonus points/overall score
        // update lanes list (remove at least one lane also might block other lanes with new word)

        

        

        /*
        // define quadrants and sample board
        // List<(int, string)> quadrants =
        // [
        //     (countQuadrant(quadrantMap["topLeft"]), "topLeft"),
        //     (countQuadrant(quadrantMap["topRight"]), "topRight"),
        //     (countQuadrant(quadrantMap["bottomLeft"]), "bottomLeft"),
        //     (countQuadrant(quadrantMap["bottomRight"]), "bottomRight"),
        // ];

        // // pick quadrant
        // var topPick = quadrants.OrderByDescending(x => x.Item1).First();

        // if (quadrants.FindAll(x => x.Item1 == topPick.Item1).Count > 1)
        // {
        //     // check more than one quadrant?
        // }
        */


        /*
            Will use one or a combination of methods 

            "Computer vision" method
            - Define 8x8 segments for each "quadrant" (board is 15x15 so im double counting the center row/column, am i biasing towards the middle) 
            - Take sample of board (don't query evey coordinate)
            - Take 2x2 squares and asign total number tiles found
            so end up with 4 by 4 grid of tile distribution per quadrant
            - 


            "Path finder" method

            1. Identify quadrant (or half?) of board that is most empty

            2. Look for "hooks" (tiles from previous words) that will open up lanes in quadrant

            3. Use hook letter and player tiles (1 + 7) and find bingos (and also non bingo potential words)

            4. Apply bonus tile scores to potential words 

            5. Select best option ()
        */

        return (string.Empty, false);
    }

    // [true = across, false = down]
    private void findPlayersFirstMove(out string word, out int x_coordinate, out int y_coordinate, out bool direction)
    {
        var coord = scrabbleBoard.GetBoardPosition(centerSquare);

        int coordinateOffset = 0;

        // up/down
        if (scrabbleBoard.IsOpenSpace(coord.x - 1, coord.y) && scrabbleBoard.IsOpenSpace(coord.x + 1, coord.y))
        {
            char[] charArray = new char[7];

            // search up until empty
            for (int i = 0; i < 7; i++)
            {
                if (scrabbleBoard.IsOpenSpace(coord.x, coord.y - i))
                {
                    coordinateOffset = coord.y - i + 1;
                    break;
                }

                charArray[i] = scrabbleBoard.GetTileChar(coord.x, coord.y - i);
            }

            Array.Reverse(charArray);
            var sb = new StringBuilder(new string(charArray));

            // search down until empty
            for (int i = 0; i < 7; i++)
            {
                if (scrabbleBoard.IsOpenSpace(coord.x, coord.y + i))
                {
                    break;
                }

                sb.Append(scrabbleBoard.GetTileChar(coord.x, coord.y + i));
            }

            word = sb.ToString();

            x_coordinate = coord.x;

            y_coordinate = coord.y - coordinateOffset;

            direction = false;

        }
        // left/right
        else
        {
            char[] charArray = new char[7];

            // search left until empty
            for (int i = 0; i < 7; i++)
            {
                if (scrabbleBoard.IsOpenSpace(coord.x - i, coord.y))
                {
                    coordinateOffset = coord.x - i + 1;
                    break;
                }

                charArray[i] = scrabbleBoard.GetTileChar(coord.x, coord.y - i);
            }

            Array.Reverse(charArray);
            var sb = new StringBuilder(new string(charArray));

            // search right until empty
            for (int i = 0; i < 7; i++)
            {
                if (scrabbleBoard.IsOpenSpace(coord.x + i, coord.y))
                {
                    break;
                }

                sb.Append(scrabbleBoard.GetTileChar(coord.x + i, coord.y));
            }

            word = sb.ToString();

            x_coordinate = coord.x - coordinateOffset;

            y_coordinate = coord.y;
          
            direction = true;
        }
    }

    private int calculateWordScore(in ScrabbleLane move)
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
            for (int i = 0; i < move.Word.Length; i++)
            {
                // DL('[') , DW('\'), TL(']'), TW('^')

                char character = scrabbleBoard.GetTileChar(move.X_coordinate + i, move.Y_coordinate);
                // could probably put this switch in a function
                // if I want to cut down on file size
                switch (character)
                {
                    case '[':
                        int newCharVal = generator.GetTilePointValue(move.Word[i]);
                        newCharVal = newCharVal * 2;
                        score = score + newCharVal;
                        break;
                    case '\\':
                        doubleWord++;
                        score = score + generator.GetTilePointValue(move.Word[i]);
                        break;
                    case ']':
                        int newCharVal3 = generator.GetTilePointValue(move.Word[i]);
                        newCharVal3 = newCharVal3 * 3;
                        score = score + newCharVal3;
                        break;
                    case '^':
                        tripleWord++;
                        score = score + generator.GetTilePointValue(move.Word[i]);
                        break;
                }
            }
        }
        else
        {
            // 
            // direction ==  false: down
            for (int i = 0; i < move.Word.Length; i++)
            {
                // DL('[') , DW('\'), TL(']'), TW('^')

                char character = scrabbleBoard.GetTileChar(move.X_coordinate, move.Y_coordinate + 1);

                switch (character)
                {
                    case '[':
                        int newCharVal = generator.GetTilePointValue(move.Word[i]);
                        newCharVal = newCharVal * 2;
                        score = score + newCharVal;
                        break;
                    case '\\':
                        doubleWord++;
                        score = score + generator.GetTilePointValue(move.Word[i]);
                        break;
                    case ']':
                        int newCharVal3 = generator.GetTilePointValue(move.Word[i]);
                        newCharVal3 = newCharVal3 * 3;
                        score = score + newCharVal3;
                        break;
                    case '^':
                        tripleWord++;
                        score = score + generator.GetTilePointValue(move.Word[i]);
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

    private int countQuadrant((int x, int y) coordinate)
    {
        int count = 0;

        for (int x = coordinate.x; x < 8; x++)
        {
            for (int y = coordinate.y; y < 8; y++)
            {
                if (!scrabbleBoard.IsOpenSpace(x, y))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private List<ScrabbleLane> getLanesFromWord(in ScrabbleLane word)
    {
        var lanes = new List<ScrabbleLane>();

        for (int i = 0; i < word.Word.Length; i++)
        {
            int j;

            // search in the right direction
            for (j = 0; j < 15; j++)
            {
                // if there is space on some side of the letter
                //if ()
            }

            for (j = 15 - j; j < 0; j++)
            {
                
            }
        }
        
        return lanes;
    }

    private Dictionary<string, (int x, int y)> quadrantMap = new Dictionary<string, (int x, int y)>
    {
        {"topLeft", (0, 0) },
        {"topRight", (7, 0) },
        {"bottomLeft", (0, 7) },
        {"bottomRight", (7, 7) },
    };

    private (string word_coordinate, bool direction) makeStartingMove(string playerTiles)
    {
        // get potential bingos
        var bingoWords = generator.GetBingoList(playerTiles).OrderBy(x => x.Score);

        // choose word with highest bonus points
        string chosenWord = string.Empty;
        string startingPosition = string.Empty;

        // only do this if have potential bingos of length 5, 6, or 7
        //
        // (tuple can't be null)
        var longWord = bingoWords.First(x => x.Word.Length > 4).Word;
        if (longWord == null || longWord == string.Empty)
        {
            // check this
            bingoWords.OrderBy(x => x.Word.Length);
            chosenWord = bingoWords.First().Word;

            // for not default choice is the center square
            startingPosition = centerSquare;
        }
        else
        {

        }


        // choose direction
        // this doesn't really matter as much for first move

        return (chosenWord + "_" + startingPosition, false);
    }


    // prime objectives

    // 1. pick words with the most points

    // 2. pick words that utilize bonus tiles

    // 3. pick words that block your opponent from utilizing

    // 4. (end game) when bag is empty prioritize words that use as many of your letters as possible

}
