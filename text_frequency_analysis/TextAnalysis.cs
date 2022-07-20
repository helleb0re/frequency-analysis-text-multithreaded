namespace text_frequency_analysis;

public static class TextAnalysis
{
    // набор символов, которые не могут являться частью слова
    static readonly HashSet<char> PunctuationMarks = new() 
        {' ', '.', ',', '-', ';', ':', '[', '(', ']', ')', '"', '\'', '{', '}', '/', 
            '\\', '\n', '\r', '\t', '!', '?', '<', '>', '+', '_', '=', '\f', '\v', '\0'};

    public static Dictionary<string, int> FrequencySyllable(string s, int syllableLength)
    {
        // объявляем хранилище данных
        Dictionary<string, int> strCounts = new Dictionary<string, int>();
        
        string syllable = "";
        
        // главный цикл
        foreach (char c in s)
        {
            // проверяем, является ли символ недопустимым
            if (PunctuationMarks.Contains(c))
            {
                // если да, то очищаем слог и переходим к следующему символу
                syllable = "";
                continue;
            }
            // добавляем символ в проверяемый слог
            syllable += Char.ToLower(c);
            // если длина слога меньше заданного, то переходим к следующему символу
            if (syllable.Length < syllableLength) continue;
            
            // заносим данные о найденном слоге
            if (strCounts.ContainsKey(syllable)) strCounts[syllable]++;
            else strCounts[syllable] = 1;
            
            // удаляем первый символ
            syllable = syllable.Remove(0, 1);
        }

        return strCounts;
    }
}