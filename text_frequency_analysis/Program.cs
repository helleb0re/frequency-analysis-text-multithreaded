using System.Diagnostics;
using text_frequency_analysis;


Stopwatch stopWatch = new Stopwatch();
stopWatch.Start();
int syllableLength = 3;
string s = File.ReadAllText("input.txt");

int numThreads = Environment.ProcessorCount;
int chunk = s.Length / numThreads;
int d = s.Length % numThreads;

Thread[] threads = new Thread[numThreads];

Dictionary<string, int> strCounts = new Dictionary<string, int>();
for (int i = 0; i < numThreads; i++)
{
    string shortStr = s.Substring(i * chunk, chunk + (i + 1 == numThreads ? d : 0));
    
    if (i != 0) shortStr = s.Substring(i * chunk - syllableLength + 1, syllableLength - 1) + shortStr;

    threads[i] = new Thread(() =>
    {
        // // var tasks = new List<Task<Dictionary<string, int>>>();
        // int k = 10;
        // var tasks = new Task<Dictionary<string, int>>[k];
        //
        // int chunkTask = shortStr.Length / k;
        // int dTask = shortStr.Length % k;
        //
        // for (int j = 0; j < k; j++)
        // {
        //     int index = j;
        //     tasks[j] = new Task<Dictionary<string, int>>(() => 
        //         TextAnalysis.FrequencySyllable(
        //             shortStr.Substring(index * chunkTask, chunkTask + (index + 1 == k ? dTask : 0)),
        //             3));
        //     tasks[j].Start();
        //     // tasks.Add(Task<Dictionary<string, int>>.Factory.StartNew(() => 
        //     //     TextAnalysis.FrequencySyllable(sArray[index], 3)));
        // }
        //
        // Task.WaitAll(tasks);
        //
        // var strCountsTasks = new Dictionary<string, int>();
        //
        // foreach (var t in tasks)
        //     TextAnalysis.MergeDictionaries(ref strCountsTasks, t.Result);
        
        var dict = TextAnalysis.FrequencySyllable(shortStr, 3);
        lock (strCounts)
        {
            MergeDictionaries(ref strCounts, dict);
        }
        
    });
    threads[i].Start();
}

foreach (Thread thread in threads)
{
    thread.Join();
}

// var res = TextAnalysis.FrequencySyllable(s, 3);

// Console.WriteLine(res.Values.Sum());
// Console.WriteLine(strCounts.Values.Sum());

var res = strCounts;

Console.WriteLine(String.Join(", ", res
    .OrderByDescending((x => x.Value))
    .Take(10)
    .Select(x => $"{x.Key}={x.Value}")));

stopWatch.Stop();
Console.WriteLine($"Затраченное время: {stopWatch.Elapsed.TotalMilliseconds}");


static void MergeDictionaries(ref Dictionary<string, int> dict1, Dictionary<string, int> dict2)
{
    if (dict1.Count == 0)
    {
        dict1 = dict2.ToDictionary(entry => entry.Key, entry => entry.Value);
    }
    else
    {
        foreach (KeyValuePair<string,int> kv in dict2)
        {
            if (dict1.ContainsKey(kv.Key))
            {
                dict1[kv.Key] += kv.Value;
            }
            else
            {
                dict1[kv.Key] = kv.Value;
            }
        }
    }
}