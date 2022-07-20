namespace text_frequency_analysis;

public static class TextAnalysis
{
    static readonly HashSet<char> PunctuationMarks = new() 
        {' ', '.', ',', '-', ';', ':', '[', '(', ']', ')', '"', '\'', '{', '}', '/', 
            '\\', '\n', '\r', '\t', '!', '?'};

    public static Dictionary<string, int> FrequencySyllable(string s, int syllableLength)
    {
        Dictionary<string, int> strCounts = new Dictionary<string, int>();
        
        Queue<char> strQueue = new Queue<char>();

        foreach (char c in s)
        {
            if (PunctuationMarks.Contains(c))
            {
                strQueue.Clear();
                continue;
            }

            strQueue.Enqueue(Char.ToLower(c));
    
            if (strQueue.Count < syllableLength) continue;

            string str = new string(strQueue.ToArray());

            if (strCounts.ContainsKey(str)) strCounts[str]++;
            else strCounts[str] = 1;

            strQueue.Dequeue();
        }

        return strCounts;
    }
}