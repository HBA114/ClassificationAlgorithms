using ConsoleApp.Exceptions;

namespace ConsoleApp.Data;
public class DataProcessing
{
    private bool _random;
    private string? _dataFilePath;

    public DataProcessing() { }

    public DataProcessing(string dataFilePath, bool random) : this()
    {
        _dataFilePath = dataFilePath;
        _random = random;
    }

    public async Task<Tuple<List<string>, List<string>>> ReadTrainAndTestData(string trainDataFilePath, string testDataFilePath)
    {
        List<string> trainData = await ReadData(trainDataFilePath);
        List<string> testData = await ReadData(testDataFilePath);

        return new(trainData, testData);
    }

    public async Task<Tuple<List<string>, List<string>>> SeperateTrainAndTest(string dataFilePath, float trainDataPercentile = 0.7f, string trainDataFilePath = "", string testDataFilePath = "", bool random = false)
    {
        _dataFilePath = dataFilePath;
        _random = random;
        // TODO: Test Random!
        if (trainDataPercentile <= 0 || trainDataPercentile >= 1)
            throw new DataProcessingException("Train Data Percentile should be between 0 and 1. Example : 0.7");

        List<string> trainData = new List<string>();
        List<string> testData = new List<string>();

        List<string> data = await ReadData(_dataFilePath!);

        trainData.Add(data[0]);
        testData.Add(data[0]);

        // TODO: Her bir sınıf için test ve eğitim verisinde çeşitliliğin sağlanması gerekir.
        if (_random)
        {
            int trainDataCounter = Int32.Parse(Math.Round((trainDataPercentile * 100), 0).ToString());
            int counter = Int32.Parse(((data.Count() * trainDataCounter) / 100).ToString());

            Random rnd = new Random();

            List<DataColumn> trainDataColumns = new List<DataColumn>();
            List<DataColumn> testDataColumns = new List<DataColumn>();

            for (int i = 0; i < counter; i++)
            {
                int index = rnd.Next(1, data.Count());
                // trainData.Add(data[index]);
                string dataString = data[index];
                List<string> dataColumns = dataString.Split(",").ToList();
                string className = dataColumns[dataColumns.Count() - 1];
                trainDataColumns.Add(new DataColumn(className, dataString));
                data.Remove(data[index]);
            }
            for (int i = 1; i < data.Count(); i++)
            {
                string dataString = data[i];
                List<string> dataColumns = dataString.Split(",").ToList();
                string className = dataColumns[dataColumns.Count() - 1];
                testDataColumns.Add(new DataColumn(className, dataString));
            }

            trainDataColumns = trainDataColumns.OrderBy(x => x.ClassName).ToList();
            testDataColumns = testDataColumns.OrderBy(x => x.ClassName).ToList();
            for (int i = 0; i < trainDataColumns.Count(); i++)
            {
                trainData.Add(trainDataColumns[i].Data);
            }
            for (int i = 0; i < testDataColumns.Count(); i++)
            {
                testData.Add(testDataColumns[i].Data);
            }
        }
        else
        {
            int trainDataCounter = Int32.Parse((trainDataPercentile * 10).ToString());
            int testDataCounter = 10 - trainDataCounter;

            // Add Column Headers each data
            for (int i = 1; i < data.Count();)
            {
                if (i + 10 < data.Count())
                {
                    for (int j = i; j < i + trainDataCounter; j++)
                    {
                        trainData.Add(data[j]);
                    }
                    i += trainDataCounter;
                    for (int j = i; j < i + testDataCounter; j++)
                    {
                        testData.Add(data[j]);
                    }
                    i += testDataCounter;
                }
                else
                {
                    trainData.Add(data[i]);
                    i++;
                }
            }
        }

        if (trainDataFilePath != "")
        {
            await File.WriteAllLinesAsync(trainDataFilePath, trainData);
        }

        if (testDataFilePath != "")
        {
            await File.WriteAllLinesAsync(testDataFilePath, testData);
        }

        return new(trainData, testData);
    }

    private async Task<List<string>> ReadData(string filePath)
    {
        string[] data = await File.ReadAllLinesAsync(filePath);
        List<string> rows = data.ToList();
        return rows;
    }
}

public class DataColumn
{
    public string ClassName { get; set; }
    public string Data { get; set; }

    public DataColumn(string className, string data)
    {
        ClassName = className;
        Data = data;
    }
}