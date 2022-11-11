using System.Diagnostics;
using ConsoleApp.Data;
using ConsoleApp.KNN;
using ConsoleApp.NaiveBayes;

string datasetCSVFilePath = "Data/dry_bean_dataset.csv";
string trainDataSavePath = "Data/dry_bean_dataset_train.csv";
string testDataSavePath = "Data/dry_bean_dataset_test.csv";

string naiveBayesModelSavePath = "Data/Models/NaiveBayesModel.csv";

//! Paths are changing on debuging and running from console
// Console.WriteLine(Environment.CurrentDirectory);

string projectDirectory = Environment.CurrentDirectory;
string pathVariable = "";

if (projectDirectory.Contains("bin"))
{
    projectDirectory = projectDirectory.Split("bin")[0];
}

if (!projectDirectory.Contains("ConsoleApp"))
{
    pathVariable = "ConsoleApp";
}

string filePath = Path.Combine(projectDirectory, pathVariable, datasetCSVFilePath);
string trainDataPath = Path.Combine(projectDirectory, pathVariable, trainDataSavePath);
string testDataPath = Path.Combine(projectDirectory, pathVariable, testDataSavePath);

string naiveBayesModelPath = Path.Combine(projectDirectory, pathVariable, naiveBayesModelSavePath);


DataProcessing dataProcessing = new DataProcessing();

//! Warning: If you do not have train and test data files as csv comment next line which,
//! includes ReadTrainAndTestData function then uncomment the line includes SeperateTrainAndTest function
//! if you have train and data files:
// Tuple<List<string>, List<string>> seperatedData = await dataProcessing.ReadTrainAndTestData(trainDataFilePath: trainDataPath, testDataFilePath: testDataPath);

//! if you do not want to save train and test data:
//! Use SeperateTrainAndTest method with only 1 argument trainDataPercentile
//! Warning: If you do not have train and test data files as csv uncomment next line
Tuple<List<string>, List<string>> seperatedData = await dataProcessing.SeperateTrainAndTest(
        dataFilePath: filePath, trainDataPercentile: 0.7f, trainDataFilePath: trainDataPath, testDataFilePath: testDataPath, random: true);
List<string> trainData = seperatedData.Item1;
List<string> testData = seperatedData.Item2;

NaiveBayes naiveBayes = new NaiveBayes();

Stopwatch timer = new Stopwatch();

//! if you save the model file you can read model with:
//! if you do not have any model saved: comment next line and run train:
// await naiveBayes.ReadModel(naiveBayesModelPath);

//! in case you have not saved Naive Bayes model then you should train algorithm with function below (uncomment next line for train)
await naiveBayes.TrainNaiveBayesModelAsync(trainDataset: trainData, saveModel: true, modelPath: naiveBayesModelPath);

timer.Start();
double naiveBayesTestResult = naiveBayes.TestNaiveBayesModel(testDataset: testData);
timer.Stop();

TimeSpan timerElapsedNaiveBayes = timer.Elapsed;

Console.WriteLine("Naive Bayes Accuracy : " + Math.Round(naiveBayesTestResult, 2));
Console.WriteLine("Naive Bayes Accuracy : " + Math.Round(naiveBayesTestResult, 2) * 100 + "%\n");

int minutesNaiveBayes = Convert.ToInt32(timerElapsedNaiveBayes.TotalMinutes);
int secondsNaiveBayes = Convert.ToInt32(timerElapsedNaiveBayes.TotalSeconds) % 60;
int milliSecondsNaiveBayes = Convert.ToInt32(timerElapsedNaiveBayes.TotalMilliseconds) % 1000;

Console.WriteLine("Naive Bayes Test Timer : " + secondsNaiveBayes + " Seconds and " + milliSecondsNaiveBayes + " Milliseconds\n");

KNN knn = new KNN();

Console.WriteLine("KNN Calculation Could Take 5 Minutes(.net7.0) or More (9 Minutes with .net6.0) Depending to the Device.");
Console.WriteLine("Takes 7 Minutes .net7.0, 10 Minutes with .net6.0 (If you are running on laptop be sure your device plugged in to outlet.)");
Console.WriteLine("KNN Accuracy Calculating ...");

timer.Reset();
timer.Start();
double knnTestResult = knn.TestKNN(trainDataset: trainData, testDataset: testData, K: 5, useWeights: false);
timer.Stop();

TimeSpan timerElapsedKNN = timer.Elapsed;

Console.WriteLine("KNN Accuracy : " + Math.Round(knnTestResult, 2));
Console.WriteLine("KNN Accuracy : " + Math.Round(knnTestResult, 2) * 100 + "%");

int minutesKNN = Convert.ToInt32(timerElapsedKNN.TotalMinutes);
int totalSecondsKNN = Convert.ToInt32(timerElapsedKNN.TotalSeconds);
int secondsKNN = totalSecondsKNN % 60;

Console.WriteLine("KNN Test Timer : " + minutesKNN + " Minutes and " + secondsKNN + " Seconds");

Console.Write("\nPress Enter For Exit...");
Console.ReadLine();