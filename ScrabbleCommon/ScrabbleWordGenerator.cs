using System.Text;

public class ScrabbleWordGenerator
{
    // expect full filePath
    public ScrabbleWordGenerator(string filePath)
    {
        // create dictionary
        officialScrabbleDictionary = new HashSet<string>();
        _12letterWords = new HashSet<string>();
        _13letterWords = new HashSet<string>();
        _14letterWords = new HashSet<string>();
        _15letterWords = new HashSet<string>();

        // open file
        StreamReader stream = new StreamReader(filePath);

        // used this array to figure out dictionary composition, use this again for other dictionarys
        //var arr = new int[16];

        // readfile/fill dictionary
        string? line;
        while ((line = stream.ReadLine()) != null)
        {
            // example line : "WHATCHAMACALLIT a {thingy=n} [n]"
            string[] entry = line.Split(' ');

            switch (entry[0].Length)
            {
                case 12:
                    _12letterWords.Add(entry[0]);
                    break;
                case 13:
                    _13letterWords.Add(entry[0]);
                    break;
                case 14:
                    _14letterWords.Add(entry[0]);
                    break;
                case 15:
                    _15letterWords.Add(entry[0]);
                    break;
                default:
                    officialScrabbleDictionary.Add(entry[0]);
                    break;
            }

            //arr[entry[0].Length]++;
        }

        // close file
        stream.Close();


        // for (int i = 0; i < 16; i++)
        // {
        //     Console.WriteLine(i + " letter words: " + arr[i]);
        // }
        // //Console.WriteLine(officialScrabbleDictionary.Count);
    }

    HashSet<string> officialScrabbleDictionary;
    HashSet<string> _12letterWords;
    HashSet<string> _13letterWords;
    HashSet<string> _14letterWords;
    HashSet<string> _15letterWords;


    public List<(string Word, int Score)> GetBingoList(string playerLetters)
    {
        playerLetters = ConvertLowerWordToUpperWord(playerLetters);


        // check if the string has illegal letter selection 
        // (too many of a single letter than is available in bag)
        if (!checkTileDistribution(playerLetters))
        {
            Console.WriteLine("Illegal Scrabble tile combination, see tile distribution");
        }

        //
        HashSet<string> bingoWords = new HashSet<string>();


        // checking permuation of an input string grows at n!
        // 7! = 5040 (wow)
        //
        // checking all substrings' permutations adds another n!

        // may have to change my approach to handle full board bingos (15 length)
        // in that case just search through dictionary values and pick out matches to input string?
        // try to "spell out" each entry using the letters available, upper bounded by 15 x 


        switch (playerLetters.Length)
        {
            case 12:
                spellWordsOut(playerLetters, _12letterWords, bingoWords);
                break;
            case 13:
                spellWordsOut(playerLetters, _13letterWords, bingoWords);
                break;
            case 14:
                spellWordsOut(playerLetters, _14letterWords, bingoWords);
                break;
            case 15:
                spellWordsOut(playerLetters, _15letterWords, bingoWords);
                break;
            default:
                // get n-length words (where is n is the length of the user input string and is 11 or less)
                getPermutations(playerLetters.ToCharArray(), 0, playerLetters.Length - 1, bingoWords);

                //get smaller words?
                getSmallerPermutations(playerLetters, bingoWords);
                break;
        }



        return GetPointValues(bingoWords);
    }

// to do
    public List<string> GetWords(string playerLetters)
    {
        return new List<string>();
    }

    private void spellWordsOut(string playerLetters, HashSet<string> dictionary, HashSet<string> validWords)
    {
        List<char> chars;

        foreach (var word in dictionary)
        {
            chars = [.. playerLetters];

            for (int i = 0; i < playerLetters.Length; i++)
            {
                if (chars.Contains(word[i]))
                {
                    chars.Remove(word[i]);
                }
            }

            if (chars.Count == 0)
            {
                validWords.Add(word);
            }
        }
    }

    private void getSmallerPermutations(string playerLetters, HashSet<string> validWords)
    {
        StringBuilder sb;

        // remove one letter and get permutations, go smaller
        for (int i = 0; i < playerLetters.Length - 1; i++)
        {
            // // remove ith letter from string
            sb = new StringBuilder(playerLetters);
            sb.Remove(i, 1);
            string substring = sb.ToString();

            getPermutations(substring.ToCharArray(), 0, playerLetters.Length - 2, validWords);

            getSmallerPermutations(substring, validWords);
        }
    }

    // would it be more efficient to change this array into a list?
    private void getPermutations(char[] array, int left, int right, HashSet<string> potentialWords)
    {
        if (left == right)
        {
            string potentialWord = new string(array);

            // should I do this here?
            // aren't I checking a bigger hashset first?
            if (checkDictionary(potentialWord))
            {
                if (!potentialWords.Contains(potentialWord))
                {
                    potentialWords.Add(potentialWord);
                }
            }
        }
        else
        {
            for (int i = left; i <= right; i++)
            {
                swap(ref array[left], ref array[i]);
                getPermutations(array, 1 + left, right, potentialWords);
                swap(ref array[left], ref array[i]);
            }
        }
    }

    private static void swap(ref char a, ref char b)
    {
        char temp = a;
        a = b;
        b = temp;
    }


    // for now, don't consider board position and associated bonus points (dl(double letter bonus),tl,dw,tw,etc)
    private static List<(string, int)> GetPointValues(HashSet<string> words)
    {
        List<(string, int)> bingoPoints = new List<(string, int)>(words.Count);

        int wordValue;

        foreach (string word in words)
        {
            wordValue = 0;

            foreach (char letter in word)
            {
                wordValue += letterValues[letter];
            }

            bingoPoints.Add((word, wordValue));
        }

        return bingoPoints;
    }

    public static string ConvertLowerWordToUpperWord(string lower)
    {
        StringBuilder sb = new StringBuilder(lower.Length);

        foreach (char character in lower)
        {
            sb.Append(ConverLowerToUpper(character));
        }

        return sb.ToString();
    }

    public static char ConverLowerToUpper(char letter)
    {
        int asciiValue = (int)letter;

        // is upperCase?
        if ((letter > 64) && (letter < 91))
        {
            return letter;
        }
        // is lowerCase
        else if ((letter > 96) && (letter < 123))
        {
            asciiValue = asciiValue - 32;

            return (char)asciiValue;
        }

        return letter;
    }

    // maybe this should not be a separate function,
    // will do this inline in the next version
    private static char convertUpperToLowerCase(char letter)
    {
        int asciiValue = (int)letter;

        // is upperCase?
        if ((letter > 64) && (letter < 91))
        {
            asciiValue = asciiValue + 32;

            return (char)asciiValue;
        }
        // is lowerCase
        else if ((letter > 96) && (letter < 123))
        {
            return letter;
        }

        throw new Exception("Bad inputs");
    }

    public static int GetTilePointValue(char tile)
    {
        if (!letterValues.ContainsKey(tile))
        {
            return int.MinValue;
        }

        return letterValues[tile];
    }

    // Point value assigned to each letter by the game Scrabble
    // shouldn't this be in the ScrabbleBoard object?
    // too much of hassel to change right now
    private static Dictionary<char, int> letterValues = new Dictionary<char, int>
    {
        {'*', 0},
        {'A', 1},
        {'B', 3},
        {'C', 3},
        {'D', 2},
        {'E', 1},
        {'F', 4},
        {'G', 2},
        {'H', 4},
        {'I', 1},
        {'J', 8},
        {'K', 5},
        {'L', 1},
        {'M', 3},
        {'N', 1},
        {'O', 1},
        {'P', 3},
        {'Q', 10},
        {'R', 1},
        {'S', 1},
        {'T', 1},
        {'U', 1},
        {'V', 4},
        {'W', 4},
        {'X', 8},
        {'Y', 4},
        {'Z', 10}
    };

    // Check that a possible word actually exists in some official Scrabble Dictionary 
    // (there are many choices for actual dictionary verison American/International/other)
    // Works for words smaller than 12 letters
    private bool checkDictionary(string possibleWord)
    {
        // for testing
        //return true;

        //return phoneyScrabbleDictionary.Contains(possibleWord);
        return officialScrabbleDictionary.Contains(possibleWord);

    }

    // have to change code to read uppercase
    // if I want to use this for testing purposes, convert to uppercase because thats easier to physically retyping this
    private static HashSet<string> phoneyScrabbleDictionary = new HashSet<string>
    {
        "argents",
        "garnets",
        "strange",
        "agents",
        "angers",
        "argent",
        "gaster",
        "grants",
        "ranges",
        "retags",
        "serang",
        "strang",
        "antres",
        "sterna",
        "angers",
        "garnet",
        "gaters",
        "graste",
        "greats",
        "rengas",
        "sanger",
        "stager",
        "targes",
        "astern",
        "transe"
    };

    private static bool checkTileDistribution(string playerInput)
    {
        var array = new int[27];

        for (int i = 0; i == playerInput.Length; i++)
        {
            int characterAscii = (int)ConverLowerToUpper(playerInput[i]);

            if (characterAscii == 42)
            {
                array[0]++;
            }
            else if ((characterAscii >= 65) && (characterAscii <= 90))
            {
                array[characterAscii - 64]++;
            }
            else
            {
                Console.WriteLine("Unexpected character encountered. Input string must contain valid Scrabble tile characters (A-Z, a-z, or ' ' (blank tile))");
            }
        }

        // check distribution of Scrabble
        for (int j = 1; j < 27; j++)
        {
            if (array[j] > LetterDistList[j])
            {
                return false;
            }
        }

        return true;
    }

    public static List<int> LetterDistList = new List<int>
    {
        2, 9, 2, 2, 4, 12, 2, 3, 2, 9, 1, 1, 4, 2, 6, 8, 2, 1, 6, 4, 6, 4, 2, 2, 1, 2, 1
    };

    // keep to read letter values, this is easier on the eyes but is overkill programatically
    // private static Dictionary<char, int> letterDistribution = new Dictionary<char, int>
    // {
    //     {' ', 2},
    //     {'A', 9},
    //     {'B', 2},
    //     {'C', 2},
    //     {'D', 4},
    //     {'E', 12},
    //     {'F', 2},
    //     {'G', 3},
    //     {'H', 2},
    //     {'I', 9},
    //     {'J', 1},
    //     {'K', 1},
    //     {'L', 4},
    //     {'M', 2},
    //     {'N', 6},
    //     {'O', 8},
    //     {'P', 2},
    //     {'Q', 1},
    //     {'R', 6},
    //     {'S', 4},
    //     {'T', 6},
    //     {'U', 4},
    //     {'V', 2},
    //     {'W', 2},
    //     {'X', 1},
    //     {'Y', 2},
    //     {'Z', 1}
    // };

    public bool CheckDictionary(string scabbleWord)
    {
        // capitalize word
        string upperCase = ConvertLowerWordToUpperWord(scabbleWord);

        return officialScrabbleDictionary.Contains(upperCase) ||
        _12letterWords.Contains(upperCase) ||
        _13letterWords.Contains(upperCase) ||
        _14letterWords.Contains(upperCase) ||
        _15letterWords.Contains(upperCase);
    }

    // right now only using this for a single character lane
    // might have to redesign lanes to include more than one char, which will necesitate a change here
    // in char[] must be 7 characters long
    public HashSet<string> GetPossibleScrabbleWords(in char[] playerTiles, in char laneChar)
    {
        HashSet<string> possibleWords = new HashSet<string>();

        var totalTiles = new char[8];

        for (int i = 0; i < 7; i++)
        {
            totalTiles[i] = playerTiles[i];
        }

        totalTiles[7] = laneChar;

        getPossibleScrabbleWordHelper(totalTiles, possibleWords);
        
        return possibleWords;
    }

    private void getPossibleScrabbleWordHelper(char[] totalTiles, HashSet<string> possibleWords)
    {
        if (containsBlanks(totalTiles, out int[] indexes))
        {
            // indexes are set to this default value
            if (indexes[1] == int.MaxValue)
            {
                for (int i = 0; i < 26; i++)
                {
                    // value of 'A' = 65, 'Z' = 90

                    // int va = 65 + i;
                    // var a = (char)va;

                    // switch out every letter of the alphabet in the blank index spot and call permutations
                    totalTiles[indexes[0]] = (char)(65 + i);

                    string tiles = new string(totalTiles);

                    getPermutations(totalTiles, 0, totalTiles.Length - 1, possibleWords);

                    getSmallerPermutations(tiles, possibleWords);
                }
            }
            else
            {
                for (int i = 0; i < 26; i++)
                {
                    // value of 'A' = 65, 'Z' = 90

                    // switch out every letter of the alphabet in the blank index spot
                    totalTiles[indexes[0]] = (char)(65 + i);

                    for (int j = 0; j < 26; j++)
                    {
                        // switch out every letter of the alphabet in the blank index spot and call permutations
                        totalTiles[indexes[1]] = (char)(65 + j);

                        string tiles = new string(totalTiles);

                        getPermutations(totalTiles, 0, totalTiles.Length - 1, possibleWords);

                        getSmallerPermutations(tiles, possibleWords);
                    }
                }
            }
        }
        else
        {
            string tiles = new string(totalTiles);

            getPermutations(totalTiles, 0, totalTiles.Length - 1, possibleWords);

            getSmallerPermutations(tiles, possibleWords);
        }

    }

    // test this
    public bool IsValidTileRack(string tileRack)
    {
        HashSet<string> possibleWords = new HashSet<string>();

        var totalTiles = new char[7];

        for (int i = 0; i < 7; i++)
        {
            totalTiles[i] = tileRack[i];
        }

        getPossibleScrabbleWordHelper(totalTiles, possibleWords);

        if (possibleWords.Count == 0)
        {
            return false;
        }

        return true;
    }
    
    private bool containsBlanks(char[] totalTiles, out int[] indexes)
    {
        //I don't want to use linq here because at most there only supposed to be two 

        indexes = new int[2];
        indexes[0] = int.MaxValue;
        indexes[1] = int.MaxValue;
        int indexindex = 0;

        if (totalTiles.Contains('*'))
        {
            // is there a more efficient way to do this?
            // there is 
            for (int i = 0; i < totalTiles.Length; i++)
            {
                if (totalTiles[i] == '*')
                {
                    indexes[indexindex] = i;
                    indexindex++;
                }
            }
        }
        
        if (indexindex >= 3)
        {
            throw new Exception("unexpected number of blank tiles");
        }

        if (indexindex > 0)
        {
            return true;
        }

        return false;
    }
}