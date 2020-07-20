using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using EFDataManager;
using Entities;
using Entities.TransferEntities;
using LogicForUI.Interfaces;
using Newtonsoft.Json;
using Refit;
using User = Entities.User;

namespace LogicForUI
{
    public class UIScheduleSupplier
    {
        private IScheduleWebInterface Scheduler;

        public UIScheduleSupplier()
        {
            var connection =
                System.Configuration.ConfigurationManager.
                    ConnectionStrings["ServerConnection"].ConnectionString;
            Scheduler = RestService.For<IScheduleWebInterface>(connection);
        }

        public string ToDeserializable(string s)
        {
            return s.Replace(@"\", @"").TrimStart('"').TrimStart('\\').TrimEnd('"').TrimEnd('\\');
        }

        public IReadOnlyList<CTask> GetSchedule(string user)
        {
            return JsonConvert.DeserializeObject<IReadOnlyList<Entities.CTask>>(ToDeserializable(Scheduler.GetSchedule(user).Result));
        }

        public IReadOnlyList<CTask> GetPredictedSchedule(User user, TimeSpan prefered, List<CTask> userTasks)
        {

            return JsonConvert.DeserializeObject<IReadOnlyList<CTask>>(
                ToDeserializable(Scheduler.PredictSchedule(user.Name, (int) prefered.TotalSeconds).Result));
        }

        public void DeleteTaskFromSchedule(User user, CTask toBeDeleted)
        {
            Scheduler.DeleteTaskFromSchedule(user.Id, toBeDeleted.Id);
        }

        public void RefreshTaskInSchedule(User userName, CTask oldTask, CTask newTask)
        {
            if (Scheduler.DeleteTaskFromSchedule(userName.Id, oldTask.Id).Result)
            {
                if (newTask.ActualDuration == TimeSpan.FromSeconds(0))
                {
                    Scheduler.AddTaskToSchedule(userName.Id, newTask.Id);
                }
            }
        }

        public void ScheduleToSomeoneOther(string me, CTask t)
        {
            foreach (var user in t.ConnectedUsers.Where(user => user != me))
            {
                Scheduler.AddTaskToSchedule(user, t.Id);
            }
        }

        public void AddTaskToSchedule(User user, CTask task)
        {
            Scheduler.AddTaskToSchedule(user.Id, task.Id);
        }
    }
}