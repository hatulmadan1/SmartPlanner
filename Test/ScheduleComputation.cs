using System;
using System.Collections.Generic;
using Entities;

namespace Server
{
    public class ScheduleComputation
    {
        public IReadOnlyList<Entities.CTask> PredictSchedule(Entities.User user, TimeSpan preferedDuration, IReadOnlyList<Entities.CTask> userTasks)
        {
            Random random = new Random();
            List<Entities.CTask> result = new List<Entities.CTask>();
            List<Entities.CTask> tasksOfUser = new List<CTask>(userTasks);
            TimeSpan totalDuration = TimeSpan.Zero;
            int index = 0;
            TimeSpan delta = TimeSpan.FromSeconds(preferedDuration.TotalSeconds / 20);
            int down = tasksOfUser.Count * 5;

            while (down != 0 && Math.Abs(preferedDuration.TotalSeconds - totalDuration.TotalSeconds) > delta.TotalSeconds)
            {
                if (tasksOfUser.Count == 0)
                {
                    break;
                }
                index = random.Next(0, tasksOfUser.Count);
                if (totalDuration + tasksOfUser[index].PredictedDuration < preferedDuration + delta)
                {
                    result.Add(tasksOfUser[index]);
                    totalDuration += tasksOfUser[index].PredictedDuration;
                    tasksOfUser.RemoveAt(index);
                    down = tasksOfUser.Count * 5;
                }
                else
                {
                    down--;
                }
            }

            return result;
        }
    }
}
