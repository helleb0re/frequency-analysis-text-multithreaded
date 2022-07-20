using System.Diagnostics;
using text_frequency_analysis;

// Инициализация и включение таймера
Stopwatch stopWatch = new Stopwatch();
stopWatch.Start();
// Чтение данных из файла, путь к которому указывается при запуске программы
string s = File.ReadAllText(args[0]);

int syllableLength = 3; // Длина набора символов слова
int numThreads = Environment.ProcessorCount;
int chunk = s.Length / numThreads; // Длина текста, обработка которого будет выполняться одним потоком
int d = s.Length % numThreads; // Остаток текста, который необходимо определить какому-либо потоку дополнительно

Thread[] threads = new Thread[numThreads];

Dictionary<string, int> strCounts = new Dictionary<string, int>(); // Общий сбор данных
// Цикл для запуска потоков
for (int i = 0; i < numThreads; i++)
{
    // Получаем срез строки
    string shortStr = s.Substring(i * chunk, chunk + (i + 1 == numThreads ? d : 0));
    // Учитываем тот факт, что задача разбивается на несколько потоков, поэтому целостность текста нарушается
    // Для этого нужно добавить к срезу дополнительно последние символы из предыдущего среза
    if (i != 0) shortStr = s.Substring(i * chunk - syllableLength + 1, syllableLength - 1) + shortStr;
    
    // Подготавливаем поток на запуск
    threads[i] = new Thread(() =>
    {
        // запускаем метод, который непосредственно выполняет частотный анализ
        var dict = TextAnalysis.FrequencySyllable(shortStr, 3);
        // блокируем общий сбор данных для того, чтобы добавить туда новые данные
        lock (strCounts)
        {
            MergeDictionaries(strCounts, dict);
        }
        
    });
    // запускаем поток
    threads[i].Start();
}

// ожидаем выполнение каждого потока
foreach (Thread thread in threads)
    thread.Join();

// сортируем исходные данные по значению и берем 10 первых
var res = String.Join(", ", strCounts
    .OrderByDescending((x => x.Value))
    .Take(10)
    .Select(x => $"{x.Key}"));
// останавливаем таймер
stopWatch.Stop();
// выводим результат
Console.WriteLine(res);
Console.WriteLine($"Затраченное время: {stopWatch.Elapsed.TotalMilliseconds}");

// функция для слияния общих данных и данных, полученных из второстепенных потоков
static void MergeDictionaries(Dictionary<string, int> mainDict, Dictionary<string, int> dict)
{
    foreach (KeyValuePair<string, int> kv in dict)
    {
        if (mainDict.ContainsKey(kv.Key))
            mainDict[kv.Key] += kv.Value;
        else
            mainDict[kv.Key] = kv.Value;
    }
}