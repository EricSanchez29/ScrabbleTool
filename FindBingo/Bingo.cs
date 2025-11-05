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
        string line;
        while ((line = stream.ReadLine()) != null)
        {
            // process : "WHATCHAMACALLIT a {thingy=n} [n]"
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

    // search for 7 and 6 letter words
    public List<(string, int)> GetBingoList(string playerLetters)
    {
        playerLetters = convertLowerWordToUpperWord(playerLetters);


        // check if the string has illegal letter selection 
        // (too many of a single letter than is available in bag)



        // Get all 7 letter words
        // Maybe get 6 or 5 letter ones too
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

    private void spellWordsOut(string playerLetters, HashSet<string> dictionary, HashSet<string> validWords)
    {
        List<char> chars;

        foreach (var word in dictionary)
        {
            // this is the problem, hashset has to have unique elemnts

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

    private void getPermutations(char[] array, int left, int right, HashSet<string> bingos)
    {
        if (left == right)
        {
            string potentialWord = new string(array);

            // should I do this here?
            if (checkDictionary(potentialWord))
            {
                if (!bingos.Contains(potentialWord))
                {
                    bingos.Add(potentialWord);
                }
            }
        }
        else
        {
            for (int i = left; i <= right; i++)
            {
                swap(ref array[left], ref array[i]);
                getPermutations(array, 1 + left, right, bingos);
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
    private static List<(string,int)> GetPointValues(HashSet<string> words)
    {
        List<(string,int)> bingoPoints = new List<(string,int)>(words.Count);

        int wordValue;

        foreach (string word in words)
        {
            wordValue = 0;

            foreach (char letter in word)
            {
                wordValue += letterValues[letter];
            }

            bingoPoints.Add((word,wordValue));
        }

        return bingoPoints;
    }

    private static string convertLowerWordToUpperWord(string lower)
    {
        StringBuilder sb = new StringBuilder(lower.Length);

        foreach (char character in lower)
        {
            sb.Append(converLowerToUpper(character));
        }

        return sb.ToString();
    }

    // need to test
    private static char converLowerToUpper(char letter)
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

        throw new Exception("Bad inputs");
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

    // Point value assigned to each letter by the game Scrabble
    private static Dictionary<char, int> letterValues = new Dictionary<char, int>
    {
        {' ', 0},
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
    private bool checkDictionary(string possibleWord)
    {
        // for testing
        //return true;

        //convert phoneyDictionaryToUpperCase
        

        //return phoneyScrabbleDictionary.Contains(possibleWord);
        return officialScrabbleDictionary.Contains(possibleWord);

        // making this its own separate function so I don't have to change it later when I query my database
        // or some external Scrabble Dictionary over https
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

    private static Dictionary<char, int> letterDistribution = new Dictionary<char, int>
    {
        {' ', 2},
        {'A', 9},
        {'B', 2},
        {'C', 2},
        {'D', 4},
        {'E', 12},
        {'F', 2},
        {'G', 3},
        {'H', 2},
        {'I', 9},
        {'J', 1},
        {'K', 1},
        {'L', 4},
        {'M', 2},
        {'N', 6},
        {'O', 8},
        {'P', 2},
        {'Q', 1},
        {'R', 6},
        {'S', 4},
        {'T', 6},
        {'U', 4},
        {'V', 2},
        {'W', 2},
        {'X', 1},
        {'Y', 2},
        {'Z', 1}
    };
}