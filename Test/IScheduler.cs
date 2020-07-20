using System;
using System.Collections.Generic;

namespace Server
{
    public interface IScheduler
    {
        IReadOnlyList<Entities.CTask> GetSchedule(string userName);
        IReadOnlyList<Entities.CTask> PredictSchedule(Entities.User user, TimeSpan preferedDuration, IReadOnlyList<Entities.CTask> userTasks);
        void AddTaskToSchedule(Entities.User user, Entities.CTask task);
        void AddTaskToSchedule(string userName, Entities.CTask task);
        bool DeleteTaskFromSchedule(Entities.User user, Entities.CTask task);
    }
}
