using System;
using System.Collections.Generic;
using DBManager;
using Entities;
using Entities.TransferEntities;
using Newtonsoft.Json;
using LogicForUI.Interfaces;
using Refit;
using User = Entities.User;

namespace LogicForUI
{
    public class UIDataSupplier
    {
        public readonly ITaskWebInterface DataAccess;

        public UIDataSupplier()
        {
            var connection =
                System.Configuration.ConfigurationManager.
                    ConnectionStrings["ServerConnection"].ConnectionString;
            DataAccess = RestService.For<ITaskWebInterface>(connection);
        }

        public string ToDeserializable(string s)
        {
            return s.Replace(@"\", @"").TrimStart('"').TrimStart('\\').TrimEnd('"').TrimEnd('\\');
        }

        public void DeleteTask(User user, CTask task)
        {
            string data = JsonConvert.SerializeObject(task);
            DataAccess.DeleteTaskFromUserHistory(user.Name, task.Id);
        }

        public List<Entities.CTask> GetUserData(string userName)
        {
            string backup = DataAccess.GetUserHistory(userName).Result;
            return JsonConvert.DeserializeObject<List<Entities.CTask>>(ToDeserializable(backup));
        }

        public int CreateTask(string taskName, string userName)
        {
            return DataAccess.CreateTask(taskName, userName).Result;
        }

        public void UpdateTask(User user, CTask task)
        {
            string data = "\"" +  JsonConvert.SerializeObject(new UserTaskTransfer {User = user, Task = task}).Replace("\"", "\\\"") + "\"";
            DataAccess.UpdateTask(data);
        }

        public IReadOnlyList<string> GetAllTags(string userName)
        {
            return this.DataAccess.ReadAllTagNamesFromDB(userName).Result;
        }

        public void CompleteForAllUsers(Entities.CTask t)
        {
            var userSupplier = new UIUserSupplier();
            foreach (string user in t.ConnectedUsers)
            {
                var tasks = new List<Entities.CTask>(this.GetUserData(user));
                foreach(var task in tasks)
                {
                    if (task.Id == t.Id)
                    {
                        task.SetActualDuration(t.ActualDuration);
                        string data = "\"" + JsonConvert.SerializeObject(new UserTaskTransfer { User = userSupplier.GetUserByName(user), Task = task }).Replace("\"", "\\\"") + "\"";

                        DataAccess.UpdateTask(data);
                        tasks.Remove(task);
                        break;
                    }
                }

                //this.DataAccess.SaveUserHistory(user, JsonConvert.SerializeObject(tasks));
            }
        }

        public void AddTaskToAllConnectedUsers(Entities.User user, Entities.CTask t)
        {
            string data = "\"" + JsonConvert.SerializeObject(new UserTaskTransfer { User = user, Task = t }).Replace("\"", "\\\"") + "\"";

            DataAccess.UpdateTask(data);
            /*foreach(string user in t.ConnectedUsers)
            {
                var tasks = new List<Entities.Task>(this.DataAccess.GetUserHistory(user));
                foreach (var task in tasks)
                {
                    if (task.Id == t.Id)
                    {
                        tasks.Remove(task);
                        continue;
                    }
                }
                tasks.Add(t);
                this.DataAccess.SaveUserHistory(user, tasks);
            }*/
        }
    }
}