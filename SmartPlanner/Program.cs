using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using BusinessLogic;
using DBManager;
using Entities;
using System.IO;

namespace SmartPlanner
{
    class Program
    {
        static List<Entities.CTask> Planner = new List<Entities.CTask>();
        static IDataAccess DataAccess = new DataAccessXmlImpl("user1");
        static ITimePredictor TimePredictor = new TimePredictorImpl(DataAccess);
        static string FileName = @"..\..\..\DBManager\TaskList.json";
        static void Main(string[] args)
        {
            String backup = File.ReadAllText(FileName);
            if (backup != "")
            {
                Planner = JsonConvert.DeserializeObject<List<CTask>>(backup);
            }
            while (true)
            {
                Console.WriteLine("\n--------------------Main menu--------------------");
                Console.WriteLine("What do you want to do?");
                Console.WriteLine("A TaskName - Add Task named TaskName  ");
                Console.WriteLine("S          - See all current Task  ");
                Console.WriteLine("E #i       - Edit a task #i from the list of current Tasks  ");
                Console.WriteLine("P #i       - Predict a duration for task #i  ");
                Console.WriteLine("I #i       - Info about task #i ");
                Console.WriteLine("D #i       - Delete task #i ");
                Console.WriteLine("Q          - Quit  \n");

                String answer = Console.ReadLine();

                String correctString = @"(S|Q|([EPID]{1}\s#[0-9]+)|A\s\S+)\s*";

                if (answer.Length == 0 || !Regex.IsMatch(answer, correctString))
                {
                    Console.WriteLine("Sorry, there are no such command");
                    continue;
                }

                switch ((char)answer[0]) 
                {
                    case 'A':
                        String[] splitedAnswer = answer.Split(' ');
                        if (splitedAnswer.Length != 2)
                        {
                            Console.WriteLine("Wrong TaskName. Empty or with spaces. \n");
                            break;
                        }

                        Planner.Add(new CTask() { TaskName = splitedAnswer[1], PredictedDuration = TimeSpan.Zero });

                        break;
                    case 'S':
                        for (int i = 0; i < Planner.Count; i++)
                        {
                            Console.WriteLine(i.ToString() + " " + Planner[i].TaskName + "\n");
                        }
                        break;
                    case 'E':
                        int.TryParse(answer.Substring(3, answer.Length - 3), out int taskNum);
                        if (Planner.Count <= taskNum || taskNum < 0)
                        {
                            Console.WriteLine("No task with such number\n");
                            break;
                        }

                        while (true)
                        {
                            Console.WriteLine("\n--------------------Task menu--------------------");
                            Console.WriteLine("T          - view all existing Tags");
                            Console.WriteLine("A TagName  - Add a tag TagName");
                            Console.WriteLine("E hh:mm:ss - Enter actual duration");
                            Console.WriteLine("D TagName  - Delete a tag TagName");
                            Console.WriteLine("I          - Info about current task");
                            Console.WriteLine("P          - Predict duration");
                            Console.WriteLine("B          - Back to Main menu\n");

                            String[] toEdit = Console.ReadLine().Split(' ');
                            if (toEdit[0] == "B")
                            {
                                break;
                            }
                            switch (toEdit.Length)
                            {
                                case 1:
                                    switch (toEdit[0])
                                    {
                                        case "T":
                                            foreach (Tag currentTag in DataAccess.ReadAllTagsFromDB())
                                            {
                                                Console.Write(currentTag.Name + ", ");
                                            }
                                            Console.WriteLine();
                                            break;
                                        case "I":
                                            Console.Write(Planner[taskNum].ToString());
                                            break;
                                        case "P":
                                            if (Planner[taskNum].PredictedDuration == TimeSpan.Zero)
                                            {
                                                Planner[taskNum].PredictedDuration = TimePredictor.PredictTimeForTask(Planner[taskNum]);
                                            }
                                            break;
                                        default:
                                            Console.WriteLine("Wrong command. Please, try again\n");
                                            break;
                                    }
                                    break;
                                case 2:
                                    switch (toEdit[0])
                                    {
                                        case "A":
                                            Planner[taskNum].AddTag(toEdit[1]);
                                            break;
                                        case "D":
                                            Planner[taskNum].DeleteTag(toEdit[1]);
                                            break;
                                        case "E":
                                            string pattern = "([0-1]{1}[0-9]{1})|(2[0-3]{1})\\:[0-5]{1}[0-9]{1}\\:[0-5]{1}[0-9]{1}";
                                            if (!Regex.IsMatch(toEdit[1], pattern))
                                            {
                                                Console.WriteLine("Sorry, wrong time format. \n");
                                                break;
                                            }
                                            Planner[taskNum].SetActualDuration(TimeSpan.Parse(toEdit[1]));

                                            DataAccess.WriteToDB(Planner[taskNum]);

                                            Console.WriteLine("Task's statistic saved, statistic refreshed!\n");

                                            break;
                                        default:
                                            Console.WriteLine("Wrong command. Please, try again\n");
                                            break;
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Wrong command. Please, try again\n");
                                    break;
                            }
                        }
                        break;
                    case 'P':
                        int.TryParse(answer.Substring(3, answer.Length - 3), out int taskNumber);
                        if (Planner.Count <= taskNumber || taskNumber < 0) 
                        {
                            Console.WriteLine("No task with such number");
                            break;
                        }

                        if (Planner[taskNumber].PredictedDuration == TimeSpan.Zero)
                        {
                            Planner[taskNumber].PredictedDuration = TimePredictor.PredictTimeForTask(Planner[taskNumber]);
                        }

                        Console.WriteLine(Planner[taskNumber].TaskName + " time prediction is " + Planner[taskNumber].PredictedDuration.ToString());

                        break;
                    case 'I':
                        int.TryParse(answer.Substring(3, answer.Length - 3), out int taskNumb);
                        if (Planner.Count <= taskNumb || taskNumb < 0)
                        {
                            Console.WriteLine("No task with such number\n");
                            break;
                        }
                        Console.Write(Planner[taskNumb].ToString());
                        break;
                    case 'D':
                        int.TryParse(answer.Substring(3, answer.Length - 3), out int taskN);
                        if (Planner.Count <= taskN || taskN < 0)
                        {
                            Console.WriteLine("No task with such number\n");
                            break;
                        }
                        Planner.RemoveAt(taskN);
                        Console.WriteLine("Task is deleted. Be carefull, check number before editing\n");
                        break;
                    case 'Q':
                        File.WriteAllText(FileName, JsonConvert.SerializeObject(Planner));
                        return;
                    default:
                        Console.WriteLine("Wrong command. Please, Try again\n");
                        break;
                }
            }
        }
    }
}
