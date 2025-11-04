public static class Bingo
{
    // search for 7 and 6 letter words
    public static List<(string, int)> GetBingoList(string playerLetters)
    {
        // check if the string has illegal letter selection 
        // (too many of a single letter than is available in bag)


        // Get all 7 letter words
        // Maybe get 6 or 5 letter ones too
        //
        List<string> bingoWords = new List<string>();

        
        return null;
    }

    private static void getBingoRecursion(string subString)
    {
        // check if this word works

        
    }


    // for now, don't consider board position and associated bonus points (dl(double letter bonus),tl,dw,tw,etc)
    public static List<(string,int)> GetValues(List<string> words)
    {
        List<(string,int)> bingoPoints = new List<(string,int)>(words.Count);

        int wordValue;

        foreach (string word in words)
        {
            wordValue = 0;

            foreach (char letter in word)
            {
                wordValue += letterValues[convertUpperToLowerCase(letter)];
            }

            bingoPoints.Add((word,wordValue));
        }

        return bingoPoints;
    }

    // maybe this should not be a separate function,
    // will do this inline in the next version
    private static char convertUpperToLowerCase(char letter)
    {
        int asciiValue = (int)letter;

        // is upperCase?
        if ((letter > 64) && (letter < 91))
        {
            asciiValue = asciiValue - 32;

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
        {'a', 1},
        {'b', 3},
        {'c', 3},
        {'d', 2},
        {'e', 1},
        {'f', 4},
        {'g', 2},
        {'h', 4},
        {'i', 1},
        {'j', 8},
        {'k', 5},
        {'l', 1},
        {'m', 3},
        {'n', 1},
        {'o', 1},
        {'p', 3},
        {'q', 10},
        {'r', 1},
        {'s', 1},
        {'t', 1},
        {'u', 1},
        {'v', 4},
        {'w', 4},
        {'x', 8},
        {'y', 4},
        {'z', 10}
    };

    // Check that a possible word actually exists in some official Scrabble Dictionary 
    // (there are many choices for actual dictionary verison American/International/other)
    private static bool checkDictionary(string possibleWord)
    {
        return phoneyScrabbleDictionary.Contains(possibleWord);

        // making this its own separate function so I don't have to change it later when I query my database
        // or some external Scrabble Dictionary over https
    }

    private static HashSet<string> phoneyScrabbleDictionary = new HashSet<string>
    {
        "argents",
        "garnets",
        "strange",
        "agents",
        "angers",
        "argent",
        "garnet"
    };

    
}