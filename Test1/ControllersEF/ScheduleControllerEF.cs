using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EFDataManager;
using Server;
using User = Entities.User;

namespace EFDataManager
{
    public class ScheduleControllerEF //: Server.IScheduler
    {
        public IReadOnlyList<Entities.CTask> GetSchedule(string userName)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                var user = new UserControllerEF().GetUserByName(userName);
                var converter = new EntityConverter();
                return context.Schedules.
                    Where(q => q.UserId == user.Id).
                    Select(q => q.Task).
                    ToList().Select(q => converter.GetTask(q)).ToList<Entities.CTask>();
            }
        }

        public IReadOnlyList<Entities.CTask> PredictSchedule(string userName, int durationSeconds)
        {
            ScheduleComputation computation = new ScheduleComputation();
            Entities.User user = new UserControllerEF().GetUserByName(userName);
            CleanUserSchedule(user.Name);
            var schedule = computation.
                PredictSchedule(
                    user, 
                    TimeSpan.FromSeconds(durationSeconds), 
                    new List<Entities.CTask>(new TaskControllerEF().GetUserHistory(user.Name)));
            foreach (var userTask in schedule)
            {
                AddTaskToSchedule(user.Id, (int)userTask.Id);
            }

            return schedule;
        }

        private void CleanUserSchedule(string userName)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                var user = new UserControllerEF().GetUserByName(userName);
                var schedule = context.Schedules.
                    Where(q => q.UserId == user.Id);
                foreach (var userTask in schedule)
                {
                    context.Schedules.Remove(userTask);
                    context.Entry(userTask).State = EntityState.Deleted;
                }

                context.SaveChanges();
            }
        }

        public void AddTaskToSchedule(int userId, int taskId)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                var schedule = context.Schedules.FirstOrDefault(q => q.UserId == userId && q.TaskId == taskId);
                if (schedule == null)
                {
                    schedule = new Schedule
                    {
                        TaskId = (int)taskId,
                        UserId = userId
                    };
                    context.Schedules.Add(schedule);
                    context.Entry(schedule).State = EntityState.Added;
                }
                
                context.SaveChanges();
            }
        }

        public void AddTaskToSchedule(string userName, int taskId)
        {
            var user = new UserControllerEF().GetUserByName(userName);
            AddTaskToSchedule(user.Id, (int)taskId);
        }

        public bool DeleteTaskFromSchedule(int userId, int taskId)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                var schedule = context.Schedules.FirstOrDefault(q => q.TaskId == taskId && q.UserId == userId);
                if (schedule == null)
                {
                    return false;
                }
                context.Schedules.Remove(schedule);
                context.Entry(schedule).State = EntityState.Deleted;
                context.SaveChanges();
            }

            return true;
        }
    }
}
