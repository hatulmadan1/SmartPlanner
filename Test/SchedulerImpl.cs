using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Server
{
    public class SchedulerImpl : IScheduler
    {
        private readonly ScheduleComputation _scheduleComputation;

        public SchedulerImpl()
        {
            _scheduleComputation = new ScheduleComputation();
        }

        public IReadOnlyList<Entities.CTask> GetSchedule(string userName)
        {
            return LoadSchedule(userName);
        }

        public IReadOnlyList<Entities.CTask> PredictSchedule(Entities.User user, TimeSpan preferedDuration, IReadOnlyList<Entities.CTask> userTasks)
        {
            return _scheduleComputation.PredictSchedule(user, preferedDuration, userTasks);
        }

        public void AddTaskToSchedule(string userName, Entities.CTask task)
        {
            List<Entities.CTask> onEdit = LoadSchedule(userName);

            if (!onEdit.Any(q => q.Id == task.Id))
            {
                onEdit.Add(task);
            }
            
            SaveSchedule(userName, onEdit);
        }

        public bool DeleteTaskFromSchedule(string userName, Entities.CTask task)
        {
            List<Entities.CTask> onEdit = LoadSchedule(userName);
            bool result = false;

            try
            {
                onEdit.Remove(onEdit.First(t => t.Id == task.Id));
                result = true;
            }
            finally
            {
                SaveSchedule(userName, onEdit);
            }
            return result;
        }
        public void SaveSchedule(string userName, List<Entities.CTask> schedule)
        {
            File.WriteAllText(GetPath(userName), JsonConvert.SerializeObject(schedule));
        }

        private List<Entities.CTask> LoadSchedule(string userName)
        {
            string schedule = File.ReadAllText(GetPath(userName));
            List<Entities.CTask> result = JsonConvert.DeserializeObject<List<Entities.CTask>>(schedule);
            return result ?? new List<Entities.CTask>();
        }

        private string GetPath(string userName)
        {
            return @"..\..\..\Server\Schedule" + userName + ".json";
        }

        public void AddTaskToSchedule(Entities.User user, Entities.CTask task)
        {
            throw new NotImplementedException();
        }

        public bool DeleteTaskFromSchedule(Entities.User user, Entities.CTask task)
        {
            throw new NotImplementedException();
        }
    }
}
