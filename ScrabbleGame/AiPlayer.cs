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

    private List<ScrabbleMove> opponentMoves = new List<ScrabbleMove>();

    private List<ScrabbleMove> aiMoves = new List<ScrabbleMove>();

    private List<ScrabbleLane> openLanes = new List<ScrabbleLane>();

    private string centerSquare = "G8";

    // need to check that the bot has a potential bingo (+50 pts)
    public ScrabbleMove MakeMove(string playerTiles)
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

        var oppFirstMove = new ScrabbleMove
        {
            Word = word,
            X_coordinate = x,
            Y_coordinate = y,
            Direction = direction
        };

        oppFirstMove.Points = calculateWordScore(oppFirstMove, oppFirstMove.Word);

        opponentMoves.Add(oppFirstMove);

        // create lanes for empty list
        openLanes = getLanesFromWord(oppFirstMove);

        var topScoringMoves = new List<ScrabbleMove>();

        // pick words for each lane (or just top lanes if too big)
        foreach (var lane in openLanes)
        {
            var possibleWords = new List<ScrabbleMove>();

            // get words from lane letter/space and playertiles
            var bingoList = generator.GetBingoList(playerTiles + lane.Tile.ToString());

            // remove words that are too long for this lane
            // maybe i could get fancier with bigger words that include more tiles already on the board
            // for example (lane  with a/lenght=5 could not fit word angle) but if there is a d at the end could have "angled"
            bingoList.RemoveAll(x => x.Word.Length >= lane.Length);

            // get the score for each word
            foreach (var bingo in bingoList)
            {
                possibleWords.Add(new ScrabbleMove(lane)
                {
                    Points = calculateWordScore(lane, bingo.Word),
                    Word = bingo.Word,
                });
            }

            // pick the top 5 (maybe make this smaller) scoring words
            topScoringMoves.AddRange((List<ScrabbleMove>)possibleWords.OrderByDescending(x => x.Points).Take(5));
        }

        // choose word/lane with highest bonus points/overall score
        var topScoringMove = topScoringMoves.OrderByDescending(x => x.Points).First();

        aiMoves.Add(topScoringMove);

        // update lanes list (remove at least one lane also might block other lanes with new word)


        return topScoringMove;



        //TO DO 

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

    private int calculateWordScore(in ScrabbleBase move, string word)
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

                char character = scrabbleBoard.GetTileChar(move.X_coordinate + i, move.Y_coordinate);
                // could probably put this switch in a function
                // if I want to cut down on file size
                switch (character)
                {
                    case '[':
                        int newCharVal = generator.GetTilePointValue(word[i]);
                        newCharVal = newCharVal * 2;
                        score = score + newCharVal;
                        break;
                    case '\\':
                        doubleWord++;
                        score = score + generator.GetTilePointValue(word[i]);
                        break;
                    case ']':
                        int newCharVal3 = generator.GetTilePointValue(word[i]);
                        newCharVal3 = newCharVal3 * 3;
                        score = score + newCharVal3;
                        break;
                    case '^':
                        tripleWord++;
                        score = score + generator.GetTilePointValue(word[i]);
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

                char character = scrabbleBoard.GetTileChar(move.X_coordinate, move.Y_coordinate + 1);

                switch (character)
                {
                    case '[':
                        int newCharVal = generator.GetTilePointValue(word[i]);
                        newCharVal = newCharVal * 2;
                        score = score + newCharVal;
                        break;
                    case '\\':
                        doubleWord++;
                        score = score + generator.GetTilePointValue(word[i]);
                        break;
                    case ']':
                        int newCharVal3 = generator.GetTilePointValue(word[i]);
                        newCharVal3 = newCharVal3 * 3;
                        score = score + newCharVal3;
                        break;
                    case '^':
                        tripleWord++;
                        score = score + generator.GetTilePointValue(word[i]);
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

    // spaces for words, includes letter of a word already on the board
    private List<ScrabbleLane> getLanesFromWord(in ScrabbleMove move)
    {
        var lanes = new List<ScrabbleLane>();

        for (int i = 0; i < move.Word.Length; i++)
        {
            // word direction is across
            if (move.Direction)
            {
                int j;

                // search in the right direction
                for (j = move.X_coordinate + 1; j < 15; j++)
                {
                    if (!scrabbleBoard.IsOpenSpace(j, move.Y_coordinate))
                    {
                        j--;
                        break;
                    }
                }

                var leftLane = new ScrabbleLane
                {
                    Direction = true,
                    Length = j - move.X_coordinate,
                    X_coordinate = j,
                    Y_coordinate = move.Y_coordinate,
                };

                lanes.Add(leftLane);

                // search in the left direction
                for (j = move.X_coordinate - 1; j > 0; j--)
                {
                    if (!scrabbleBoard.IsOpenSpace(j, move.Y_coordinate))
                    {
                        j++;
                        break;
                    }
                }

                var rightLane = new ScrabbleLane
                {
                    Direction = true,
                    Length = j - move.X_coordinate,
                    X_coordinate = j,
                    Y_coordinate = move.Y_coordinate,
                };

                lanes.Add(rightLane);
            }
            // word direction is down
            else
            {
                int j;

                // search in the up direction
                for (j = move.Y_coordinate + 1; j < 15; j++)
                {
                    if (!scrabbleBoard.IsOpenSpace(move.X_coordinate, j))
                    {
                        j--;
                        break;
                    }
                }

                var upLane = new ScrabbleLane
                {
                    Direction = false,
                    Length = j - move.Y_coordinate,
                    X_coordinate = move.X_coordinate,
                    Y_coordinate = j,
                };

                lanes.Add(upLane);

                // search in the down direction
                for (j = move.Y_coordinate - 1; j > 0; j--)
                {
                    if (!scrabbleBoard.IsOpenSpace(move.X_coordinate, j))
                    {
                        j++;
                        break;
                    }
                }

                var downLane = new ScrabbleLane
                {
                    Direction = true,
                    Length = j - move.Y_coordinate,
                    X_coordinate = move.X_coordinate,
                    Y_coordinate = j,
                };

                lanes.Add(upLane);
            }
        }

        return lanes;
    }


    // this needs work
    // doesn't take into account bonus tiles
    // 
    private ScrabbleMove makeStartingMove(string playerTiles)
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

        return new ScrabbleMove
        {
            Points = 0,
            Word = chosenWord,
            Direction = true, // doesn't matter because bonus tiles are equivalent here, 
            // this is the only place on the board with this symmetry
            // one way is arguably harder to read by a human
        };
    }

    // this might be difficult or impossible at some point
    // should I really track every open lane?
    // Can I track every open lane through an entire of a Scrabble game?
    private void updateOpenLanes(ScrabbleMove newMove)
    {

    }

    // prime objectives

    // A. At any time if a player uses every tile, will get 50pt bonus

    // 1. pick words with the most points

    // 2. pick words that utilize bonus tiles
    // - aka pick words that block your opponent from utilizing bonus tiles

    // 3. Should I utilize small words in the midgame in order to focus on bonus tiles?
    // - 


    // Z. (end game) when bag is empty prioritize words that use as many of your letters as possible



}
