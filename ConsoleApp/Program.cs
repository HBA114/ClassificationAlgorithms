using ConsoleApp.Data;
using ConsoleApp.NaiveBayes;

// string filePath = "ConsoleApp/Data/dry_bean_dataset.csv";
// string trainDataPath = "ConsoleApp/Data/dry_bean_dataset_train.csv";
// string testDataPath = "ConsoleApp/Data/dry_bean_dataset_test.csv";

//! Paths are changing on debuging and running from console

string filePath = "Data/dry_bean_dataset.csv";
string trainDataPath = "Data/dry_bean_dataset_train.csv";
string testDataPath = "Data/dry_bean_dataset_test.csv";
string naiveBayesModelPath = "Data/Models/NaiveBayesModel.csv";

DataProcessing dataProcessing = new DataProcessing(filePath);

// if you do not want to save train and test data:
// Use SeperateTrainAndTest method with only 1 argument trainDataPercentile

Tuple<List<string>, List<string>> seperatedData = await dataProcessing.SeperateTrainAndTest(0.7f, trainDataPath, testDataPath);
List<string> trainData = seperatedData.Item1;
List<string> testData = seperatedData.Item2;

// System.Console.WriteLine("Train Data Count : " + trainData.Count());
// System.Console.WriteLine("Test Data Count : " + testData.Count());

// TODO: Create NaiveBayes and KNN Algorithms And Test Them

// if you want to save the NaiveBayes model give true as Constructor Parameter as "saveModel: true"
// if you already saved a model give saved model's path as path

NaiveBayes naiveBayes = new NaiveBayes(saveModel: true, modelPath: naiveBayesModelPath);

await naiveBayes.TrainNaiveBayesModelAsync(trainData: trainData);