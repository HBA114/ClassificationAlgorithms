using System.Drawing;
using System.Text;
using System.Diagnostics;
using ConsoleApp.Data;
using ConsoleApp.KNN;
using ConsoleApp.NaiveBayes;

#region Path Define
string datasetCSVFilePath = "Data/dry_bean_dataset.csv";
string trainDataSavePath = "Data/dry_bean_dataset_train.csv";
string testDataSavePath = "Data/dry_bean_dataset_test.csv";
string naiveBayesModelSavePath = "Data/Models/NaiveBayesModel.csv";
#endregion

#region Path Combine
string projectDirectory = Environment.CurrentDirectory;
string pathVariable = "";
if (projectDirectory.Contains("bin"))
    projectDirectory = projectDirectory.Split("bin")[0];

if (!projectDirectory.Contains("ConsoleApp"))
    pathVariable = "ConsoleApp";

string filePath = Path.Combine(projectDirectory, pathVariable, datasetCSVFilePath);
string trainDataPath = Path.Combine(projectDirectory, pathVariable, trainDataSavePath);
string testDataPath = Path.Combine(projectDirectory, pathVariable, testDataSavePath);
string naiveBayesModelPath = Path.Combine(projectDirectory, pathVariable, naiveBayesModelSavePath);
#endregion

#region Creating Class references and Assigning new Class
Tuple<List<string>, List<string>>? seperatedData = null;
List<string>? trainData = null;
List<string>? testData = null;
int K = 0;
bool useWeights = false;
Stopwatch timer = new Stopwatch();
StringBuilder sb = new StringBuilder();
#endregion

List<string> baseOperations = new() { "Data Operations", "Naive Bayes Operations", "KNN", "Exit" };
List<string> dataOperations = new() { "Read Data", "Seperate Data" };
List<string> naiveBayesOperations = new() { "Run Test With Saved Model", "Train New Model and Run Test" };
List<string> knnOperations = new() { "Run KNN Test", "Define K (Neighbour Count, default 5)", "Use Weights (default false)", "Main Menu" };

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
            #region Read Data or Seperate Data
            int selectedDataOpeation = 0;
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
                Int32.TryParse(input, out selectedDataOpeation);
                if (selectedDataOpeation < 1 || selectedDataOpeation > dataOperations.Count)
                    continue;
                else break;
            }

            if (selectedDataOpeation == 1)
            {
                //! Warning: If you do not have train and test data files as csv comment next line which,
                //! includes ReadTrainAndTestData function then uncomment the line includes SeperateTrainAndTest function
                //! if you have train and data files:
                seperatedData = await dataProcessing.ReadTrainAndTestData(trainDataFilePath: trainDataPath, testDataFilePath: testDataPath);
            }
            else
            {
                //! if you do not want to save train and test data:
                //! Use SeperateTrainAndTest method with only 1 argument trainDataPercentile
                //! Warning: If you do not have train and test data files as csv uncomment next line
                seperatedData = await dataProcessing.SeperateTrainAndTest(
                        dataFilePath: filePath, trainDataPercentile: 0.7f, trainDataFilePath: trainDataPath, testDataFilePath: testDataPath, random: true);
            }
            trainData = seperatedData.Item1;
            testData = seperatedData.Item2;
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
                Int32.TryParse(input, out selectedDataOpeation);
                if (selectedDataOpeation < 1 || selectedDataOpeation > dataOperations.Count)
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
            KNN knn = new KNN();
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
                        Console.WriteLine("KNN Accuracy Calculating ...");

                        timer.Reset();
                        timer.Start();
                        double knnTestResult = knn.TestKNN(trainDataset: trainData!, testDataset: testData!, K: K, useWeights: useWeights);
                        timer.Stop();

                        TimeSpan timerElapsedKNN = timer.Elapsed;

                        Console.WriteLine("KNN Accuracy : " + Math.Round(knnTestResult, 2));
                        Console.WriteLine("KNN Accuracy : " + Math.Round(knnTestResult, 2) * 100 + "%");

                        int minutesKNN = Convert.ToInt32(timerElapsedKNN.TotalMinutes);
                        int totalSecondsKNN = Convert.ToInt32(timerElapsedKNN.TotalSeconds);
                        int secondsKNN = totalSecondsKNN % 60;

                        Console.WriteLine("KNN Test Timer : " + minutesKNN + " Minutes and " + secondsKNN + " Seconds");
                        #endregion
                        break;
                    case 2:
                        #region K Options
                        while (true)
                        {
                            Console.WriteLine("Please enter number for K (neighbour count)...");
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