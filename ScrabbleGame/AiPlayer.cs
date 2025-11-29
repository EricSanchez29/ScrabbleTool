using System.Text;

public class ScrabbleBot : IPLayer
{
    public ScrabbleBot(ScrabbleWordGenerator wordGenerator, IBoard player)
    {
        generator = wordGenerator;
        scrabbleBoard = player;
    }

    private ScrabbleWordGenerator generator;

    private IBoard scrabbleBoard;

    private List<ScrabbleMove> opponentMoves = new List<ScrabbleMove>();

    private List<ScrabbleMove> botMoves = new List<ScrabbleMove>();

    private List<ScrabbleLane>? openLanes = null;

    private string centerSquare = "H8";

    private (int X, int Y) centerCoordinate = new(7, 7);

    private List<char> tileRack = new List<char>(7);

    // to do: need to check that the bot has a potential bingo (+50 pts)
    public ScrabbleMove MakeMove(string playerTiles)
    {
        // Case 1: AI making the first move
        // if center is empty, no one has made a legal scrabble move yet
        if (scrabbleBoard.GetTileChar(centerSquare) == '\\')
        {
            // need to update lanes, and moves list
            return makeStartingMove(playerTiles);
        }

        // get opponent's most recent move
        var oppLastMove = scrabbleBoard.GetLastMove();

        if (openLanes == null)
        {
            // Case 2: Opponent made the first move, AI goes 2nd
            // create lanes for empty list
            openLanes = getLanesFromWord(oppLastMove);
        }
        else
        {
            // Case 3: this is the AI's 2nd move or later
            updateOpenLanes(oppLastMove);
        }        

        // do I need to keep track of moves?
        opponentMoves.Add(oppLastMove);

        var potentialMoves = new List<ScrabblePotentialMove>();

        foreach (var lane in openLanes)
        {
            var words = generator.GetBingoList(playerTiles + lane.Tile);

            // every single word is considered for this lane
            foreach (var word in words)
            {

                // if (word.Word == "INTENTS")
                // {
                //     int breakpoint = 0;
                // }

                // only consider words that contain the letter in the lane
                if (!word.Word.Contains(lane.Tile))
                {
                    // for some reason im not reaching this code, 
                    // how does every word already have the tile character?
                    // does the permutation algorithm bias towards the last (and maybe first) letter of the input string?
                    continue;
                }

                // find best position for word 
                var bestMove = getBestMove(lane, word.Word);

                // validate that the move makes sense
                if (bestMove.Score == 0) { continue; }

                if (!generator.CheckDictionary(bestMove.Word)) { continue; }
                
                if (!scrabbleBoard.IsValidCoordinate(bestMove.X_coordinate, bestMove.Y_coordinate)) { continue; }

                potentialMoves.Add(new ScrabblePotentialMove(bestMove, lane));
            }
        }

        // sort potential moves by score
        var topScoringMoves = potentialMoves.OrderByDescending(x => x.Move.Score);

        // choose highest scoring potential move
        var topScoringMove = topScoringMoves.First();

        // remove tiles bot just used
        updateTileRack(topScoringMove.Move.Word);

        // remove lane from list
        openLanes.Remove(topScoringMove.Lane);

        // record bot move
        botMoves.Add(topScoringMove.Move);

        // return move
        return topScoringMove.Move;
    }

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


    // spaces for words, includes letter of a word already on the board
    // need properly orient lanes with new reverse property
    // check this function but also change code that uses lanes
    private List<ScrabbleLane> getLanesFromWord(in ScrabbleMove move)
    {
        var lanes = new List<ScrabbleLane>();

        for (int i = 0; i < move.Word.Length; i++)
        {
            // word direction is down
            if (!move.Direction)
            {
                int j;
                ScrabbleLane? leftLane = null;
                ScrabbleLane? rightLane = null;

                // search in the right direction
                for (j = move.X_coordinate + 1; j < 15; j++)
                {
                    if (!scrabbleBoard.IsOpenSpace(j, move.Y_coordinate))
                    {
                        j--;
                        break;
                    }
                }

                rightLane = new ScrabbleLane
                {
                    Tile = move.Word[i],
                    TilePosition = 0,
                    Direction = true,
                    Length = j - move.X_coordinate,
                    X_coordinate = move.X_coordinate,
                    Y_coordinate = move.Y_coordinate + i,
                };

                // search in the left direction
                for (j = move.X_coordinate - 1; j >= 0; j--)
                {
                    if (!scrabbleBoard.IsOpenSpace(j, move.Y_coordinate))
                    {
                        break;
                    }
                }

                j++;

                leftLane = new ScrabbleLane
                {
                    Tile = move.Word[i],
                    TilePosition = move.X_coordinate,
                    Direction = true,
                    Length = move.X_coordinate - j,
                    X_coordinate = j,
                    Y_coordinate = move.Y_coordinate + i,
                };

                // if lanes are found on both ends make them into one big lane
                if ((leftLane != null) && (rightLane != null))
                {
                    lanes.Add(new ScrabbleLane
                    {
                        Tile = move.Word[i],
                        TilePosition = leftLane.Length - 1,
                        Direction = true,
                        Length = leftLane.Length + rightLane.Length - 1,
                        X_coordinate = leftLane.X_coordinate,
                        Y_coordinate = leftLane.Y_coordinate,
                    });
                }
                else
                {
                    if (leftLane != null)
                    {
                        lanes.Add(leftLane);
                    }

                    if (rightLane != null)
                    {
                        lanes.Add(rightLane);
                    }
                }
            }
            // word direction is across
            else
            {
                int j;
                ScrabbleLane? upLane = null;
                ScrabbleLane? downLane = null;

                // search in the down direction
                for (j = move.Y_coordinate + 1; j < 15; j++)
                {
                    if (!scrabbleBoard.IsOpenSpace(move.X_coordinate, j))
                    {
                        j--;
                        break;
                    }
                }

                downLane = new ScrabbleLane
                {
                    Tile = move.Word[i],
                    TilePosition = 0,
                    Direction = false,
                    Length = j - move.Y_coordinate,
                    X_coordinate = move.X_coordinate + i,
                    Y_coordinate = move.Y_coordinate,
                };

                // search in the up direction
                for (j = move.Y_coordinate - 1; j >= 0; j--)
                {
                    if (!scrabbleBoard.IsOpenSpace(move.X_coordinate, j))
                    {
                        break;
                    }
                }

                // since I want to use index j as a position I need to undo the last decrement
                // which allowed the for loop to exit (either -1 position or the first non open space)
                j++;

                upLane = new ScrabbleLane
                {
                    Tile = move.Word[i],
                    TilePosition = 0, // fix this
                    Direction = false,
                    Length = move.Y_coordinate - j + 1,
                    X_coordinate = move.X_coordinate + i,
                    Y_coordinate = j,
                };

                // if lanes are found on both ends make them into one big lane
                if ((upLane != null) && (downLane != null))
                {
                    lanes.Add(new ScrabbleLane
                    {
                        Tile = move.Word[i],
                        TilePosition = upLane.Length - 1,
                        Direction = false,
                        Length = upLane.Length + downLane.Length - 1,
                        X_coordinate = upLane.X_coordinate,
                        Y_coordinate = upLane.Y_coordinate,
                    });
                }
                else
                {
                    if (upLane != null)
                    {
                        lanes.Add(upLane);
                    }

                    if (downLane != null)
                    {
                        lanes.Add(downLane);
                    }
                }
            }
        }

        return lanes;
    }

    // this might be difficult or impossible at some point
    // should I really track every open lane?
    // Can I track every open lane through an entire of a Scrabble game?
    private void updateOpenLanes(ScrabbleMove newMove)
    {
        // makes sure to validate possible lanes
    }

    // assumes that the lane character is included within the word
    // need to find the starting position of the word that lines up with
    // the lane character that is already on the board
    private ScrabbleMove getBestMove(ScrabbleLane lane, string word)
    {
        // find the indexes of all the occurences of the lane.tile character in the string
        List<int> indexes = new List<int>();
        int currentIndex = 0;

        while (currentIndex < word.Length)
        {
            // Find the next occurrence of the substring starting from currentIndex
            int foundIndex = word.IndexOf(lane.Tile, currentIndex);

            // If an occurrence is found
            if (foundIndex != -1)
            {
                indexes.Add(foundIndex);
                // Move the search starting position past the found occurrence
                currentIndex = foundIndex + 1;
            }
            else
            {
                // No more occurrences found, exit the loop
                break;
            }
        }

        int topScore = 0;
        ScrabbleBase bestCoordinate = new ScrabbleBase();

        for (int i = 0; i < indexes.Count; i++)
        {
            var coordinate = getCoordinateFromWordIndex(indexes[i], lane);

            int wordScore = scrabbleBoard.GetMoveScore(coordinate, word);

            if (wordScore > topScore)
            {
                topScore = wordScore;
                bestCoordinate = coordinate;
            }
        }

        return new ScrabbleMove(bestCoordinate)
        {
            Word = word,
            Score = topScore,
        };
    }
    
    private ScrabbleBase getCoordinateFromWordIndex(int index, ScrabbleLane lane)
    {
        var pos = new ScrabbleBase();

        if (lane.Direction) // x direction
        {
            // calculate scabble lane character position
            int laneCharacterPosition = lane.X_coordinate + lane.TilePosition;


            // how far from the beginning of the word is the index
            // same as the index value

            // subtract that distance from the lane character position

            pos.X_coordinate = laneCharacterPosition - index;
            pos.Y_coordinate = lane.Y_coordinate;
            pos.Direction = lane.Direction;

        }
        else
        {
            int laneCharacterPosition = lane.Y_coordinate + lane.TilePosition;

            pos.Y_coordinate = laneCharacterPosition - index;
            pos.X_coordinate = lane.X_coordinate;
            pos.Direction = lane.Direction;
        }

        return pos;
    }

    // need to create lanes for my own word
    private ScrabbleMove makeStartingMove(string playerTiles)
    {
        // get potential words
        var potentialWords = generator.GetBingoList(playerTiles).OrderBy(x => x.Score);
        if (potentialWords.Count() == 0)
        {
            return new ScrabbleMove
            {
                Word = string.Empty,
            };
        }

        var potentialMoves = new List<ScrabbleMove>();

        // find points for each word
        foreach (var word in potentialWords)
        {
            // for longer words calculate best move that uses double letter tile
            if (word.Word.Length > 4)
            {
                // find placement where i get the max points (DL with high tile value)
                var postition = bestInitTilePlacment(word.Word);

                var potentialMove = new ScrabbleMove
                {
                    Direction = true,
                    Word = word.Word,
                    X_coordinate = postition.x,
                    Y_coordinate = postition.y,
                };

                potentialMove.Score = scrabbleBoard.GetMoveScore(potentialMove, word.Word);
                potentialMoves.Add(potentialMove);
            }
            // for shorter words, (n <= 4) default to center square
            else
            {
                var potentialMove = new ScrabbleMove
                {
                    Direction = true,
                    Word = word.Word,
                    X_coordinate = centerCoordinate.X,
                    Y_coordinate = centerCoordinate.Y,
                };

                potentialMove.Score = scrabbleBoard.GetMoveScore(potentialMove, word.Word);
                potentialMoves.Add(potentialMove);
            }
        }
        // order by calculated score and pick hightest one
        // I'm ignoring the fact that there could be two words with the same score
        // - should the secondary

        var list = potentialMoves.OrderByDescending(x => x.Score);
        return list.First();
    }

    // defaulting to Across for direction of word
    private (int x, int y) bestInitTilePlacment(string word)
    {
        // the best move is the one where the highest value tile is placed on the double letter bonus tile
        // while still having one letter tile on the center tile (double word)
        (int x, int y) bestMove = centerCoordinate;

        bool validWordFound = false;

        var wordCopy = new StringBuilder(word);

        while (!validWordFound)
        {
            var highValIndex = findHighestValueTileIndex(wordCopy.ToString());

            if (highValIndex + 1 <= word.Length)
            {
                // legal move
                validWordFound = true;

                if (highValIndex <= 2)
                {
                    // calculate new starting position so that the highVal tile is at the right coordinate
                    bestMove.x = bestMove.x - (highValIndex + 4);
                }

                else if (highValIndex >= 4)
                {
                    // calculate new starting position so that the highVal tile is at the right coordinate
                    bestMove.x = bestMove.x - (highValIndex - 4);
                }
            }
            else
            {
                wordCopy.Replace(word[highValIndex], ' ', highValIndex, 1);
            }
        }

        return bestMove;
    }

    // find the index of the tile in the word with the largest tile value
    private int findHighestValueTileIndex(string word)
    {
        int bestTileValue = 0;
        int bestTileIndex = 0;

        // what if the first tile is blank
        // need to skip tiles sothat I don't
        for (; bestTileIndex < word.Length; bestTileIndex++)
        {
            if (word[bestTileIndex] == ' ')
            {
                continue;
            }
            else
            {
                bestTileValue = ScrabbleWordGenerator.GetTilePointValue(word[0]);
                break;
            }
        }

        if (bestTileIndex == word.Length)
        {
            Console.WriteLine();
            throw new Exception("Something went wrong");
        }

        for (int i = bestTileIndex + 1; i < word.Length; i++)
        {
            if (word[i] == ' ')
            {
                continue;
            }

            int val = ScrabbleWordGenerator.GetTilePointValue(word[i]);
            if (val > bestTileValue)
            {
                bestTileValue = val;
                bestTileIndex = i;
            }
        }

        return bestTileIndex;
    }

    public void DrawTiles()
    {
        string newTiles;

        if (tileRack.FirstOrDefault() == default(char))
        {
            newTiles = scrabbleBoard.DrawTiles(7);
        }
        else
        {
            newTiles = scrabbleBoard.DrawTiles(7 - tileRack.Count);
        }

        for (int i = 0; i < newTiles.Length; i++)
        {
            tileRack.Add(newTiles[i]);
        }
    }

    // Right now I'm not including board tiles in the word string
    private void updateTileRack(string wordTiles)
    {
        for (int i = 0; i < wordTiles.Length; i++)
        {
            if (tileRack.Find(x => x == wordTiles[i]) == default(char))
            {
                //throw new Exception("Word cannot be created using");
                continue;
            }

            tileRack.Remove(wordTiles[i]);
        }
    }

    public void MakeMove()
    {
        StringBuilder sb = new StringBuilder();

        foreach (char tile in tileRack)
        {
            sb.Append(tile);
        }

        var move = MakeMove(sb.ToString());

        scrabbleBoard.AddWord(move);
    }

    // prime objectives

    // Alpha (start game) At any time if a player uses every tile, will get 50pt bonus

    // 1. pick words with the most points

    // 2. pick words that utilize bonus tiles
    // - aka pick words that block your opponent from utilizing bonus tiles

    // 3. Should I utilize small words in the midgame in order to focus on bonus tiles?
    // - 

    // near the end you should try to take all remaining tiles from opponent, aka prioritize longer words

    // Omega (end game) when bag is empty prioritize words that use as many of your letters as possible



}
