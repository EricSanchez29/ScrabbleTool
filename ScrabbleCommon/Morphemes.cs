
namespace ScrabbleCommon;

public static class Morphemes // definition: any of the smallest meaningful constituents within (a linguistic expression and particularly within) a word
{
    static HashSet<string>? officialScrabbleDictionary;
    static HashSet<string>? possibleRootWords;

    static Dictionary<string, List<string>> root_prefixRootSuffix = new Dictionary<string, List<string>>();

    public static void GenerateMorphemeFile(string filePath)
    {
        officialScrabbleDictionary = new HashSet<string>();
        possibleRootWords = new HashSet<string>();

        // open file
        StreamReader stream = new StreamReader(filePath);

        // readfile/fill dictionary
        string? line;
        while ((line = stream.ReadLine()) != null)
        {
            // example line : "WHATCHAMACALLIT a {thingy=n} [n]"
            string[] entry = line.Split(' ');

            if (entry[0].Length < 15)
            {
                possibleRootWords.Add(entry[0]);
            }

            officialScrabbleDictionary.Add(entry[0]);
        }

        // close file
        stream.Close();

        checkForRootWords(possibleRootWords);

        StreamWriter sw = new StreamWriter(GlobalVariables.RootDictionaryFileName);

        foreach (var dictionaryItem in root_prefixRootSuffix)
        {
            sw.WriteLine(dictionaryItem.Key);

            foreach (var values in dictionaryItem.Value)
            {
                sw.WriteLine("- " + values);
            }
        }

        sw.Close();
    }


    private static void checkForRootWords(HashSet<string> wordsLists)
    {
        foreach (var word in wordsLists)
        {
            var list = new List<string>();

            foreach (var officialWord in officialScrabbleDictionary!)
            {
                if (officialWord == word)
                {
                    continue;
                }

                if (officialWord.Contains(word))
                {
                    list.Add(officialWord);
                }
            }

            if (list.Count == 0)
            {
                continue;
            }

            root_prefixRootSuffix.Add(word, list);
        }
    }
    
    public static Dictionary<string, List<string>> GetRootWordDictionary(string filePath)
    {
        var rootStructure = new Dictionary<string, List<string>>();

        StreamReader sr = new StreamReader(filePath);

        string? line;

        var list = new List<string>();

        string key = string.Empty;

        while ((line = sr.ReadLine()) != null)
        {
            if (line[0] == '-')
            {
                var superString = line.Split(' ')[1];

                list.Add(superString);
            }
            else
            {
                if (list.Count() > 0)
                {
                    rootStructure.Add(key, list);

                    list = new List<string>();
                }

                key = line;
            }
        }

        sr.Close();

        return rootStructure;
    }
}

/*
    The goal here is to identify root words with suffixes/prefixes that could be attached to create longer words

    - generate a txt file with 4+ letter root words and their suffixes/prefixes

    - create some data type to load the file data into (right now thinking a dictionary<string,List<string>>)

    // should I create a manual list of suffixes and prefixes?
*/