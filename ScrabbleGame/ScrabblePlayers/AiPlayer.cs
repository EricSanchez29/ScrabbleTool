using System.Data;
using System.Text;

public class ScrabbleBot : PlayerBase, IPlayer
{
    public ScrabbleBot(ScrabbleWordGenerator wordGenerator, IBoard board) : base(board, wordGenerator)
    {
    }


    // change this to metamove
    private List<ScrabbleMove> opponentMoves = new List<ScrabbleMove>();

    private List<ScrabbleMove> botMoves = new List<ScrabbleMove>();

    private List<ScrabbleLane>? openLanes = null;

    private string centerSquare = "H8";

    private (int X, int Y) centerCoordinate = new(7, 7);

    // to do: need to check that the bot has a potential bingo (+50 pts)
    public List<ScrabblePotentialMove> MakeMove(string playerTiles)
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

        updateOpenLanes(oppLastMove.GetMainMove());

        // if (openLanes == null)
        // {
        //     // for some reason I reached this code after AI went first

        //     // Case 2: Opponent made the first move, AI goes 2nd
        //     // create lanes for empty list
        //     openLanes = getLanesFromWord(oppLastMove.GetMainMove());
        // }
        // else
        // {
        //     // Case 3: this is the AI's 2nd move or later
        //     updateOpenLanes(oppLastMove.GetMainMove());
        // }

        // do I need to keep track of moves?
        opponentMoves.Add(oppLastMove.GetMainMove());

        var potentialMoves = new List<ScrabblePotentialMove>();

        foreach (var lane in openLanes!)
        {
            var words = generator.GetBingoList(playerTiles + lane.Tile);

            // every single word is considered for this lane
            foreach (var word in words)
            {
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

                // // this isn't really necessary
                // if (!generator.CheckDictionary(bestMove.Word)) { continue; }

                // // I've already called GetMoveScore before validating the coordinate, should I simply remove this?
                // if (!scrabbleBoard.IsValidCoordinate(bestMove.X_coordinate, bestMove.Y_coordinate)) { continue; }

                potentialMoves.Add(new ScrabblePotentialMove(bestMove, lane));
            }
        }

        // sort potential moves by score
        var topScoringMoves = potentialMoves.OrderByDescending(x => x.Move.Score);



        // // record bot move
        // botMoves.Add(topScoringMove.Move);

        // return move
        // why do I have to cast this?
        return topScoringMoves.ToList();
    }

        /*
            "Path finder" method

            2. Look for "hooks" (tiles from previous words) that will open up lanes in quadrant

            3. Use hook letter and player tiles (1 + 7) and find bingos (and also non bingo potential words)

            4. Apply bonus tile scores to potential words 

            5. Select best option ()
        */


    // spaces for words, includes letter of a word already on the board
    // check this function but also change code that uses lanes
    // 
    // this function is fundementally ignoring possible adjacent words
    //
    // need to also have a check for lenght, lane must be at least length 3 
    // - 1 space away from other letters and for min word length of 2
    private List<ScrabbleLane> getLanesFromFirstWord(in ScrabbleMove move)
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
                        j--; // test this
                        j--; // i can't have a new word next to an non open space unless I include the letter of that non open space
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
                j++; // need to test

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
    // Can I track every open lane through an entire Scrabble game?
    private void updateOpenLanes(ScrabbleMove newMove)
    {
        if (openLanes is null)
        {
            openLanes = getLanesFromFirstWord(newMove);
            return;
        }

        // create new lanes based on newMove
        //
        // look through openLanes and either
        // - modify lane if lane is perpendicular to new word
        // - delete lane if lane is parallel to the new word 
        // (currently can't handle prefixes or suffixes for word, would need to change the way I query the dictionary to do this well)
        //

        var newOpenLanes = new List<ScrabbleLane>();

        if (newMove.Direction)
        {
            for (int i = 0; i < newMove.Word.Length; i++)
            {
                ScrabbleLane laneFromNewMove;

                // if tile immediately above or below is occupied cannot form a lane
                // by this game's definition a lane can only have one char
                if (newMove.Y_coordinate > 0)
                {
                    if (!scrabbleBoard.IsOpenSpace(newMove.X_coordinate + i, newMove.Y_coordinate - 1))
                    {
                        continue;
                    }
                }

                if (newMove.Y_coordinate < 14)
                {
                    if (!scrabbleBoard.IsOpenSpace(newMove.X_coordinate + i, newMove.Y_coordinate + 1))
                    {
                        continue;
                    }
                }

                if (newMove.Y_coordinate == 0)
                {
                    laneFromNewMove = new ScrabbleLane
                    {
                        Direction = false,
                        X_coordinate = newMove.X_coordinate + i,
                        Y_coordinate = 0,
                        Tile = newMove.Word[i],
                        TilePosition = 0,
                    };
                }
                else
                {

                    int lane_y_coordinate = newMove.Y_coordinate - 1;

                    bool keepLooking = true;
                    // look above newMove
                    while (keepLooking)
                    {
                        if (scrabbleBoard.IsOpenSpace(newMove.X_coordinate, lane_y_coordinate))
                        {
                            if (lane_y_coordinate == 0)
                            {
                                // have reached the end of the board
                                break;
                            }

                            lane_y_coordinate--;
                        }
                        else
                        {
                            keepLooking = false;
                            lane_y_coordinate++;
                            lane_y_coordinate++; // need a space between words
                        }
                    }

                    // add char
                    laneFromNewMove = new ScrabbleLane()
                    {
                        Direction = false,
                        X_coordinate = newMove.X_coordinate + i,
                        Y_coordinate = lane_y_coordinate,
                        Tile = newMove.Word[i],
                        TilePosition = newMove.Y_coordinate - lane_y_coordinate, // check this math
                    };
                }

                if (newMove.Y_coordinate == 14)
                {
                    laneFromNewMove.Length = 14 - laneFromNewMove.Y_coordinate + 1;
                }
                else
                {

                    // look right
                    var keepLooking = true;
                    var lane_y_coordinate = newMove.Y_coordinate + 1;
                    while (keepLooking)
                    {
                        if (scrabbleBoard.IsOpenSpace(newMove.X_coordinate, lane_y_coordinate))
                        {
                            if (lane_y_coordinate == 14)
                            {
                                break;
                            }

                            lane_y_coordinate++;
                        }
                        else
                        {
                            keepLooking = false;
                            lane_y_coordinate--;
                            lane_y_coordinate--;
                        }
                    }

                    laneFromNewMove.Length = lane_y_coordinate - laneFromNewMove.Y_coordinate + 1;
                }

                newOpenLanes.Add(laneFromNewMove);
            }
        }
        else
        {
            for (int i = 0; i < newMove.Word.Length; i++)
            {
                ScrabbleLane laneFromNewMove;

                // if tile immediately to the left or the right is occupied cannot form a lane
                // by this game's definition a lane can only have one char
                if (newMove.X_coordinate > 0)
                {
                    // can only look to the left the coordinate if its not the edge of the board
                    if (!scrabbleBoard.IsOpenSpace(newMove.X_coordinate - 1, newMove.Y_coordinate + i))
                    {
                        continue;
                    }
                }

                if (newMove.X_coordinate < 14)
                {
                    if (!scrabbleBoard.IsOpenSpace(newMove.X_coordinate + 1, newMove.Y_coordinate + i))
                    {
                        continue;
                    }
                }

                if (newMove.X_coordinate == 0)
                {
                    laneFromNewMove = new ScrabbleLane()
                    {
                        Direction = true,
                        X_coordinate = 0,
                        Y_coordinate = newMove.Y_coordinate + i,
                        Tile = newMove.Word[i],
                        TilePosition = 0,
                    };
                }
                else
                {

                    int lane_x_coordinate = newMove.X_coordinate - 1;

                    bool keepLooking = true;
                    // look above newMove
                    while (keepLooking)
                    {
                        if (scrabbleBoard.IsOpenSpace(lane_x_coordinate, newMove.Y_coordinate))
                        {
                            if (lane_x_coordinate == 0)
                            {
                                // have reached the end of the board
                                break;
                            }

                            lane_x_coordinate--;
                        }
                        else
                        {
                            keepLooking = false;
                            lane_x_coordinate++;
                            lane_x_coordinate++; // need a space between words
                        }
                    }

                    // add char
                    laneFromNewMove = new ScrabbleLane()
                    {
                        Direction = true,
                        X_coordinate = lane_x_coordinate,
                        Y_coordinate = newMove.Y_coordinate + i,
                        Tile = newMove.Word[i],
                        TilePosition = newMove.X_coordinate - lane_x_coordinate,
                    };
                }

                // look right
                if (newMove.X_coordinate == 14)
                {
                    // newMove is already at the edge of the board
                    laneFromNewMove.Length = 14 - laneFromNewMove.X_coordinate + 1;
                }
                else
                {
                    bool keepLooking = true;
                    int lane_x_coordinate = newMove.X_coordinate + 1;
                    while (keepLooking)
                    {
                        if (scrabbleBoard.IsOpenSpace(lane_x_coordinate, newMove.Y_coordinate))
                        {
                            if (lane_x_coordinate == 14)
                            {
                                break;
                            }

                            lane_x_coordinate++;
                        }
                        else
                        {
                            keepLooking = false;
                            lane_x_coordinate--;
                            lane_x_coordinate--;
                        }
                    }

                    laneFromNewMove.Length = lane_x_coordinate - laneFromNewMove.X_coordinate + 1;
                }

                newOpenLanes.Add(laneFromNewMove);
            }
        }
        
        var lanesToRemove = new List<ScrabbleLane>();

        // check to see if any openLanes cross paths with the newMove
        if (newMove.Direction) // newWord is pointing Across
        {
            foreach (var openLane in openLanes)
            {
                if (openLane.Direction)
                {
                    // parallel lanes
                    if (openLane.Y_coordinate == newMove.Y_coordinate)
                    {
                        // does the lane cross paths with the newWord
                        // - either they occupy the same tile space 
                        // - or there is not at least one tile space between them

                        // lane starts before the word starts
                        if (openLane.X_coordinate < newMove.X_coordinate)
                        {
                            if (openLane.X_coordinate + openLane.Length >= newMove.X_coordinate)
                            {
                                // need to be removed but can't do that in a foreach loop
                                //openLanes.Remove(openLane);

                                lanesToRemove.Add(openLane);
                            }
                        }
                        // word starts before the lane starts
                        else if (openLane.X_coordinate > newMove.X_coordinate)
                        {
                            lanesToRemove.Add(openLane);

                            // at a later date I could modify lanes to include whole words 
                            // and search the dictionary using a substring of a word in order to find candidate words

                        }
                        else
                        {
                            // the openLane and newMove both start on the same tile 
                            // so lane should be deleted

                            lanesToRemove.Add(openLane);

                            // at a later date I could modify lanes to include whole words 
                            // and search the dictionary using a substring of a word in order to find candidate words
                        }
                    }
                }
                else
                {
                    // perpendicular lanes
                    if ((openLane.X_coordinate >= newMove.X_coordinate)
                    && (openLane.Y_coordinate < (newMove.Y_coordinate + newMove.Word.Length)))
                    {
                        // does the openLane intersect the path of the new word
                        if ((openLane.Y_coordinate + openLane.Length) >= newMove.Y_coordinate)
                        {
                            // either split this lane in two or resize

                            // // only split if lane is long enough to cross 
                            // if ((openLane.Y_coordinate + openLane.Length - newMove.Y_coordinate) > 2)
                            // {
                            //     // split lane
                            //     // make new lane, check the end for adjacent letters
                            //     var newLane = new ScrabbleLane()
                            //     {
                            //         Direction = true,
                            //         Length = openLane.Y_coordinate + openLane.Length - newMove.Y_coordinate, // check this math
                            //         X_coordinate = openLane.X_coordinate,
                            //         Y_coordinate = newMove.Y_coordinate,
                            //         TilePosition = 0,
                            //     };

                            //     newLane.Tile = scrabbleBoard.GetTileChar(newLane.X_coordinate, newLane.Y_coordinate);

                            //     newOpenLanes.Add(newLane);
                            // }

                            // resize original lane
                            openLane.Length = newMove.Y_coordinate - openLane.Y_coordinate - 1;
                        }
                    }
                }
            }
        }
        else // newWord is pointing Down
        {
            foreach (var openLane in openLanes)
            {
                if (openLane.Direction)
                {
                    // perpendicular lanes
                    if ((openLane.Y_coordinate >= newMove.Y_coordinate)
                    && (openLane.Y_coordinate < (newMove.Y_coordinate + newMove.Word.Length)))
                    {
                        // does the openLane intersect the path of the new word
                        if ((openLane.X_coordinate + openLane.Length) >= newMove.X_coordinate)
                        {
                            // either split this lane in two or resize

                            // // only split if lane is long enough to cross 
                            // if ((openLane.X_coordinate + openLane.Length - newMove.X_coordinate) > 2)
                            // {
                            //     // split lane
                            //     // make new lane, check the end for adjacent letters
                            //     var newLane = new ScrabbleLane()
                            //     {
                            //         Direction = true,
                            //         Length = openLane.X_coordinate + openLane.Length - newMove.X_coordinate, // check this math
                            //         X_coordinate = newMove.X_coordinate,
                            //         Y_coordinate = openLane.Y_coordinate,
                            //         TilePosition = 0,
                            //     };

                            //     newLane.Tile = scrabbleBoard.GetTileChar(newLane.X_coordinate, newLane.Y_coordinate);

                            //     newOpenLanes.Add(newLane);
                            // }

                            // resize original lane
                            openLane.Length = newMove.X_coordinate - openLane.X_coordinate - 1;

                        }
                    }
                }
                else
                {
                    // parallel lanes
                    if (openLane.X_coordinate == newMove.X_coordinate)
                    {
                        // does the lane cross paths with the newWord
                        // - either they occupy the same tile space 
                        // - or there is not at least one tile space between them

                        // lane starts before the word starts
                        if (openLane.Y_coordinate < newMove.Y_coordinate)
                        {
                            if (openLane.Y_coordinate + openLane.Length >= newMove.Y_coordinate)
                            {
                                // need to be removed but can't do that in a foreach loop
                                //openLanes.Remove(openLane);

                                lanesToRemove.Add(openLane);
                            }
                        }
                        // word starts before the lane starts
                        else if (openLane.Y_coordinate > newMove.Y_coordinate)
                        {
                            lanesToRemove.Add(openLane);

                            // at a later date I could modify lanes to include whole words 
                            // and search the dictionary using a substring of a word in order to find candidate words

                        }
                        else
                        {
                            // the openLane and newMove both start on the same tile 
                            // so lane should be deleted

                            lanesToRemove.Add(openLane);

                            // at a later date I could modify lanes to include whole words 
                            // and search the dictionary using a substring of a word in order to find candidate words
                        }
                    }
                }
            }
        }

        openLanes.AddRange(newOpenLanes);

        // look through every letter's coordinate of the newMove and create lanes when necessary
        

        foreach (var oldLane in lanesToRemove)
        {
            openLanes.Remove(oldLane);
        }
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

            
            if (!scrabbleBoard.IsValidCoordinate(coordinate.X_coordinate, coordinate.Y_coordinate)) { continue; }

            if (!scrabbleBoard.IsWordOutOfBounds(word.Length, coordinate.Direction, coordinate.X_coordinate, coordinate.Y_coordinate)) { continue; }

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
    public List<ScrabblePotentialMove> makeStartingMove(string playerTiles)
    {
        // get potential words
        var potentialWords = generator.GetBingoList(playerTiles).OrderBy(x => x.Score);
        if (potentialWords.Count() == 0)
        {
            return null!; // what should I do instead?
        }

        var potentialMoves = new List<ScrabblePotentialMove>();

        // find points for each word
        foreach (var word in potentialWords)
        {
            // for longer words calculate best move that uses double letter tile
            if (word.Word.Length > 4)
            {
                // find placement where i get the max points (DL with high tile value)
                var postition = bestInitTilePlacment(word.Word);

                var move = new ScrabbleMove
                {
                    Direction = true,
                    Word = word.Word,
                    X_coordinate = postition.x,
                    Y_coordinate = postition.y,
                };

                move.Score = scrabbleBoard.GetMoveScore(move, word.Word);
                potentialMoves.Add(new ScrabblePotentialMove(move, new ScrabbleLane()));
            }
            // for shorter words, (n <= 4) default to center square
            else
            {
                var move = new ScrabbleMove
                {
                    Direction = true,
                    Word = word.Word,
                    X_coordinate = centerCoordinate.X,
                    Y_coordinate = centerCoordinate.Y,
                };

                move.Score = scrabbleBoard.GetMoveScore(move, word.Word);
                // don't really need a scrabble lane for the first move
                potentialMoves.Add(new ScrabblePotentialMove(move, new ScrabbleLane()));
            }
        }
        // order by calculated score and pick hightest one
        // I'm ignoring the fact that there could be two words with the same score
        // - should the secondary

        var list = potentialMoves.OrderByDescending(x => x.Move.Score).ToList();

        return list;
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

    private void drawTiles()
    {
        if (scrabbleBoard.GetBagCount() == 0)
        {
            return;
        }

        string newTiles = scrabbleBoard.DrawTiles(7 - tileRack.Count);

        for (int i = 0; i < newTiles.Length; i++)
        {
            tileRack.Add(newTiles[i]);
        }
    }

    public void MakeMove(GameContext context)
    {
        var moveContext = new MoveContext(false);

        var sb = new StringBuilder();

        var metaMove = new ScrabbleMetaMove(new ScrabbleMove() { Word = string.Empty });

        string tilesToRemove = string.Empty;

        // is there a cleaner way to convert from char array to a string
        foreach (char tile in tileRack)
        {
            sb.Append(tile);
        }

        var potentialMoves = MakeMove(sb.ToString());

        if (potentialMoves is null)
        {
            tilesToRemove = passOrSwapMove(moveContext);
        }
        else
        {
            foreach (var potentialMove in potentialMoves)
            {
                tilesToRemove = scrabbleBoard.AddWord(potentialMove.Move, moveContext, GetRemainingTiles(), out metaMove);

                if (moveContext.GetRetryMove())
                {
                    continue;
                }

                // No retry means the move is valid so finish making the move

                // remove lane from list
                openLanes?.Remove(potentialMove.Lane);

                updateOpenLanes(potentialMove.Move);

                // remove tiles bot just used
                updateTileRack(tilesToRemove);

                drawTiles();

                score += metaMove.GetTotalScore();

                break;
            }

            // no valid move was found, decide if bot should pass or swap
            if (metaMove.GetMainMove().Word == string.Empty)
            {
                tilesToRemove = passOrSwapMove(moveContext);
            }
        }

        if (moveContext.GetPassMove())
        {
            Console.WriteLine();
            Console.WriteLine("ScrabbleBot is passing move");
            Console.WriteLine();
        }
        else if (moveContext.GetSwapTiles())
        {
            Console.WriteLine();
            Console.WriteLine("ScrabbleBot is swapping tiles");

            updateTileRack(tilesToRemove);

            drawTiles();
        }

        if (isFinalMove())
        {
            context.SetFinalMove();
        }
    }

    // if tiles available swap as many tiles as possible
    // otherwise pass
    private string passOrSwapMove(MoveContext moveContext)
    {
        int bagCount = scrabbleBoard.GetBagCount();

        var sb = new StringBuilder();

        if (bagCount == 0)
        {
            // no moves to make with current tiles, no new tiles to get
            // pass turn
            moveContext.SetPassMove();
            return string.Empty;
        }

        // if at least 7 tiles are left then I will end up swapping all tiles
        if (bagCount > 7)
        {
            bagCount = 7;
        }

        for (int i = 0; i < bagCount; i++)
        {
            sb.Append(tileRack[i]);
        }

        moveContext.SetSwapTiles();

        return sb.ToString();
    } 

    public void DrawInitialTiles(bool isPlayer1)
    {
        // I have room for some optimization here
        // I find possible scrabblemoves in this function and then might do the same action immediately after
        // if the bot goes first.
        drawInitialTiles(isPlayer1);
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
