﻿using System.Diagnostics;
using System.Text;

using Data;

using KnnAlgorithm;

using NaiveBayesAlgorithm;

#region Path Define
string datasetCSVFilePath = "Data/DataFiles/dry_bean_dataset.csv";
string trainDataSavePath = "Data/DataFiles/dry_bean_dataset_train.csv";
string testDataSavePath = "Data/DataFiles/dry_bean_dataset_test.csv";
string naiveBayesModelSavePath = "Data/Models/NaiveBayesModel.csv";
#endregion

#region Path Combine
string projectDirectory = Environment.CurrentDirectory;

if (projectDirectory.Contains("ConsoleApp"))
    projectDirectory = projectDirectory.Split("ConsoleApp")[0];

string filePath = Path.Combine(projectDirectory, datasetCSVFilePath);
string trainDataPath = Path.Combine(projectDirectory, trainDataSavePath);
string testDataPath = Path.Combine(projectDirectory, testDataSavePath);
string naiveBayesModelPath = Path.Combine(projectDirectory, naiveBayesModelSavePath);
#endregion

#region Creating Class references and Assigning new Class
Tuple<List<string>, List<string>>? separatedData = null;
List<string>? trainData = null;
List<string>? testData = null;
int K = 5;
bool useWeights = false;
Stopwatch timer = new Stopwatch();
StringBuilder sb = new StringBuilder();
#endregion

List<string> baseOperations = new() { "Data Operations", "Naive Bayes Operations", "KNN", "Exit" };
List<string> dataOperations = new() { "Read Data", "Separate Data" };
List<string> naiveBayesOperations = new() { "Run Test With Saved Model", "Train New Model and Run Test" };
List<string> knnOperations = new() { "Run KNN Test", "Define K (Neighbor Count, default 5)", "Use Weights (default false)", "Main Menu" };

#region Test
// KNN knn2 = new KNN();
// DataProcessing dataProcessing2 = new DataProcessing();
// var result = await dataProcessing2.SeparateTrainAndTest(
//                         dataFilePath: filePath, trainDataPercentile: 0.7f, trainDataFilePath: trainDataPath, testDataFilePath: testDataPath, random: true);

// Console.WriteLine("KNN Accuracy Calculating Array ...");



// timer.Reset();
// timer.Start();
// double knnTestResultTest = knn2.TestKNNArray(trainDataset: result.Item1!.ToArray(), testDataset: result.Item2!.ToArray(), K: K, useWeights: useWeights);
// timer.Stop();

// TimeSpan timerElapsedKNNArrayTest = timer.Elapsed;

// Console.WriteLine("KNN Accuracy Array : " + Math.Round(knnTestResultTest, 2));
// Console.WriteLine("KNN Accuracy Array : " + Math.Round(knnTestResultTest, 2) * 100 + "%");

// int minutesKNNArrayTest = Convert.ToInt32(timerElapsedKNNArrayTest.TotalMinutes);
// int totalSecondsKNNArrayTest = Convert.ToInt32(timerElapsedKNNArrayTest.TotalSeconds);
// int secondsKNNArrayTest = totalSecondsKNNArrayTest % 60;

// Console.WriteLine("KNN Test Timer : " + minutesKNNArrayTest + " Minutes and " + secondsKNNArrayTest + " Seconds");

#endregion



bool exit = false;
Console.Clear();
while (!exit)
{
    #region Base Operation
    int baseOperation = 0;
    // add a switch case for operation choose
    // clearing string builder
    sb.Remove(0, sb.Length);

    // adding current options to string builder
    for (int i = 0; i < baseOperations.Count; i++)
    {
        sb.Append((i + 1) + ": " + baseOperations[i] + "\n");
    }
    while (true)
    {
        Console.WriteLine(sb.ToString());
        Console.WriteLine("Please enter number...");
        string? input = Console.ReadLine();
        Int32.TryParse(input, out baseOperation);
        if (baseOperation < 1 || baseOperation > baseOperations.Count)
            continue;
        else
        {
            if ((trainData == null || testData == null) && baseOperation != 1 && baseOperation != 4)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("There is no Train And Test Data. Please select one of Data Operation options.");
                Console.ForegroundColor = ConsoleColor.White;
                continue;
            }
            break;
        };
    }
    #endregion

    switch (baseOperation)
    {
        case 1:
            DataProcessing dataProcessing = new DataProcessing();
            #region Read Data or Separate Data
            int selectedDataOperation = 0;
            sb.Remove(0, sb.Length);    // resetting string builder

            // Adding current options to string builder
            for (int i = 0; i < dataOperations.Count; i++)
            {
                sb.Append((i + 1) + ": " + dataOperations[i] + "\n");
            }

            while (true)
            {
                Console.WriteLine(sb.ToString());
                Console.WriteLine("Please enter number...");
                string? input = Console.ReadLine();
                Int32.TryParse(input, out selectedDataOperation);
                if (selectedDataOperation < 1 || selectedDataOperation > dataOperations.Count)
                    continue;
                else break;
            }

            if (selectedDataOperation == 1)
            {
                //! Warning: If you do not have train and test data files as csv comment next line which,
                //! includes ReadTrainAndTestData function then uncomment the line includes SeparateTrainAndTest function
                //! if you have train and data files:
                separatedData = await dataProcessing.ReadTrainAndTestData(trainDataFilePath: trainDataPath, testDataFilePath: testDataPath);
            }
            else
            {
                //! if you do not want to save train and test data:
                //! Use SeparateTrainAndTest method with only 1 argument trainDataPercentile
                //! Warning: If you do not have train and test data files as csv uncomment next line
                separatedData = await dataProcessing.SeparateTrainAndTest(
                        dataFilePath: filePath, trainDataPercentile: 0.7f, trainDataFilePath: trainDataPath, testDataFilePath: testDataPath, random: true);
            }
            trainData = separatedData.Item1;
            testData = separatedData.Item2;
            #endregion

            break;
        case 2:
            NaiveBayes naiveBayes = new NaiveBayes();
            #region Train Naive Bayes or Read From Saved Model
            int selectedNaiveBayesOperation = 0;
            sb.Remove(0, sb.Length);    // resetting string builder

            // Adding current options to string builder
            for (int i = 0; i < naiveBayesOperations.Count; i++)
            {
                sb.Append((i + 1) + ": " + naiveBayesOperations[i] + "\n");
            }

            while (true)
            {
                Console.WriteLine(sb.ToString());
                Console.WriteLine("Please enter number...");
                string? input = Console.ReadLine();
                Int32.TryParse(input, out selectedDataOperation);
                if (selectedDataOperation < 1 || selectedDataOperation > dataOperations.Count)
                    continue;
                else break;
            }

            if (selectedNaiveBayesOperation == 1)
            {
                //! if you save the model file you can read model with:
                //! if you do not have any model saved: comment next line and run train:
                await naiveBayes.ReadModel(naiveBayesModelPath);
            }
            else
            {
                //! in case you have not saved Naive Bayes model then you should train algorithm with function below (uncomment next line for train)
                await naiveBayes.TrainNaiveBayesModel(trainDataset: trainData!, saveModel: true, modelPath: naiveBayesModelPath);
            }
            timer.Reset();
            timer.Start();
            double naiveBayesTestResult = naiveBayes.TestNaiveBayesModel(testDataset: testData!);
            timer.Stop();

            TimeSpan timerElapsedNaiveBayes = timer.Elapsed;
            Console.WriteLine("Naive Bayes Accuracy : " + Math.Round(naiveBayesTestResult, 2));
            Console.WriteLine("Naive Bayes Accuracy : " + Math.Round(naiveBayesTestResult, 2) * 100 + "%\n");

            int minutesNaiveBayes = Convert.ToInt32(timerElapsedNaiveBayes.TotalMinutes);
            int secondsNaiveBayes = Convert.ToInt32(timerElapsedNaiveBayes.TotalSeconds) % 60;
            int milliSecondsNaiveBayes = Convert.ToInt32(timerElapsedNaiveBayes.TotalMilliseconds) % 1000;

            Console.WriteLine("Naive Bayes Test Timer : " + secondsNaiveBayes + " Seconds and " + milliSecondsNaiveBayes + " Milliseconds\n");
            #endregion
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
            break;
        case 3:
            Knn knn = new Knn();
            #region KNN Testing
            bool runKNN = true;
            while (runKNN)
            {
                int selectedKNNOperation = 0;
                sb.Remove(0, sb.Length);    // resetting string builder

                // Adding current options to string builder
                for (int i = 0; i < knnOperations.Count; i++)
                {
                    sb.Append((i + 1) + ": " + knnOperations[i] + "\n");
                }

                while (true)
                {
                    Console.WriteLine(sb.ToString());
                    Console.WriteLine("Please enter number...");
                    string? input = Console.ReadLine();
                    Int32.TryParse(input, out selectedKNNOperation);
                    if (selectedKNNOperation < 1 || selectedKNNOperation > knnOperations.Count)
                        continue;
                    else break;
                }

                switch (selectedKNNOperation)
                {
                    case 1:
                        #region KNN Test
                        Console.WriteLine("KNN Calculation Could Take 5 Minutes or More Depending to the Device.");
                        Console.WriteLine("Note : Takes 7 Minutes .net7.0, 10 Minutes with .net6.0 (If you are running project on a laptop be sure your device plugged in to outlet.)");
                        Console.WriteLine("KNN Accuracy Calculating List ...");

                        timer.Reset();
                        timer.Start();
                        double knnTestResult = knn.TestKNN(trainDataset: trainData!, testDataset: testData!, K: K, useWeights: useWeights);
                        timer.Stop();

                        TimeSpan timerElapsedKNNList = timer.Elapsed;

                        int minutesKNNList = Convert.ToInt32(timerElapsedKNNList.TotalMinutes);
                        int totalSecondsKNNList = Convert.ToInt32(timerElapsedKNNList.TotalSeconds);
                        int secondsKNNList = totalSecondsKNNList % 60;

                        Console.WriteLine("KNN Test Timer : " + minutesKNNList + " Minutes and " + secondsKNNList + " Seconds");

                        Console.WriteLine("KNN Accuracy List : " + Math.Round(knnTestResult, 2));
                        Console.WriteLine("KNN Accuracy List : " + Math.Round(knnTestResult, 2) * 100 + "%");

                        // Console.WriteLine("KNN Accuracy Calculating Array ...");

                        // timer.Reset();
                        // timer.Start();
                        // double knnTestResult2 = knn.TestKNNArray(trainDataset: trainData!.ToArray(), testDataset: testData!.ToArray(), K: K, useWeights: useWeights);
                        // timer.Stop();

                        // TimeSpan timerElapsedKNNArray = timer.Elapsed;

                        // Console.WriteLine("KNN Accuracy Array : " + Math.Round(knnTestResult2, 2));
                        // Console.WriteLine("KNN Accuracy Array : " + Math.Round(knnTestResult2, 2) * 100 + "%");

                        // int minutesKNNArray = Convert.ToInt32(timerElapsedKNNArray.TotalMinutes);
                        // int totalSecondsKNNArray = Convert.ToInt32(timerElapsedKNNArray.TotalSeconds);
                        // int secondsKNNArray = totalSecondsKNNArray % 60;

                        // Console.WriteLine("KNN Test Timer : " + minutesKNNArray + " Minutes and " + secondsKNNArray + " Seconds");
                        #endregion
                        break;
                    case 2:
                        #region K Options
                        while (true)
                        {
                            Console.WriteLine("Please enter number for K (neighbor count)...");
                            string? input = Console.ReadLine();
                            Int32.TryParse(input, out K);
                            if (K < 1)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Please Enter a Number Greater Than 0");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue;
                            }
                            else break;
                        }
                        #endregion
                        break;
                    case 3:
                        #region Weight Options
                        int selectedWeightOption = 0;
                        while (true)
                        {
                            Console.WriteLine("1: True\n2: False");
                            Console.WriteLine("Please enter number...");
                            string? input = Console.ReadLine();
                            Int32.TryParse(input, out selectedWeightOption);
                            if (selectedWeightOption < 1 || selectedWeightOption > 2)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Please Enter a Number Of True or False...");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue;
                            }
                            else
                            {
                                if (selectedWeightOption == 1) useWeights = true;
                                else useWeights = false;
                                break;
                            };
                        }
                        #endregion
                        break;
                    case 4:
                        runKNN = false;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid input! Press enter and try again please...");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.ReadLine();
                        continue;
                }
            }
            #endregion
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
            break;
        case 4:
            exit = true;
            break;
        default:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid input! Press enter and try again please...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadLine();
            continue;
    }
    if (!exit)
        Console.Clear();
}
