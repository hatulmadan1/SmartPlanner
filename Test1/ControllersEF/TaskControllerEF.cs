using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DBManager;
using EFDataManager;
using Tag = Entities.Tag;

namespace EFDataManager
{
    public class TaskControllerEF: IDataAccess
    {
        public List<Entities.CTask> GetUserHistory(string userName)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                var entityConverter = new EntityConverter();
                
                return context.UsersTasks.Where(q => q.User.Name.Equals(userName))
                    .Select(q => q.Task).
                    ToList<Task>().
                    Select(q => entityConverter.GetTask(q)).ToList<Entities.CTask>();
            }
        }

        public void SaveUserHistory(string userName, IReadOnlyList<Entities.CTask> history)
        {
            Entities.User user = new UserControllerEF().GetUserByName(userName);
            foreach (var task in history)
            {
                WriteToDB(user, task);
            }
        }

        public void AddTaskToUserHistory(string userName, int taskId)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                var user = new UserControllerEF().GetUserByName(userName);
                var userTask = new UsersTask
                {
                    TaskId = (int) taskId, 
                    UserId = user.Id
                };
                if (context.UsersTasks.
                    FirstOrDefault(q => q.TaskId == userTask.TaskId && q.UserId == userTask.UserId) == null)
                {
                    context.UsersTasks.Add(userTask);
                    context.Entry(userTask).State = EntityState.Added;
                }
                
                context.SaveChanges();
            }
        }

        public void DeleteTaskFromUserHistory(string userName, int taskId)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                var user = new UserControllerEF().GetUserByName(userName);
                var userTask = context.UsersTasks.FirstOrDefault(q => q.UserId == user.Id && q.TaskId == taskId);
                context.UsersTasks.Remove(userTask);
                context.Entry(userTask).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public IReadOnlyList<string> ReadAllTagNamesFromDB(string userName)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                var user = new UserControllerEF().GetUserByName(userName);
                return context.Tags.
                    Where(q => q.UserId == user.Id).
                    Select(q => q.Name).
                    ToList<string>();
            }
        }

        public Entities.Tag ReadFullTagFromDB(string userName, string requiredName)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                var user = new UserControllerEF().GetUserByName(userName);
                var tag = context.Tags.Where(q => q.UserId == user.Id).FirstOrDefault(q => q.Name.Equals(requiredName));
                if (tag == null)
                {
                    return new TagControllerEF().CreateTag(requiredName, user.Id);
                }
                return new EntityConverter().GetTag(tag);
            }
        }

        public Entities.CTask WriteToDB(Entities.User user, Entities.CTask task)
        {
            throw new NotImplementedException();
        }

        public int CreateTask(string taskName, string userName)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                Task dbTask = new Task
                {
                    Name = taskName,
                    ActualDuration = TimeSpan.Zero,
                    PredictedDuration = TimeSpan.Zero,
                };
                var taskToBeReturned = context.Tasks.Add(dbTask);
                
                context.Entry(dbTask).State = EntityState.Added;
                context.SaveChanges();
                AddTaskToUserHistory(userName, taskToBeReturned.Id);
                
                return taskToBeReturned.Id;
            }
        }

        public void UpdateTask(Entities.User user, Entities.CTask task)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                var dbTask = context.Tasks.FirstOrDefault(q => q.Id == task.Id);
                dbTask.Name = task.TaskName;
                dbTask.ActualDuration = task.ActualDuration;
                if (task.ActualDuration > TimeSpan.Zero)
                {
                    var tagController = new TagControllerEF();
                    foreach (var tag in task.Tags)
                    {
                        tag.Duration = task.ActualDuration;
                        tagController.UpdateTag(tag);
                    }

                    DeleteTaskFromUserHistory(user.Name, task.Id);
                    
                }
                dbTask.PredictedDuration = task.PredictedDuration;
                
                context.Entry(dbTask).State = EntityState.Modified;
                if (task.ActualDuration <= TimeSpan.Zero)
                {
                    UpdateTaskUsers(task);
                    UpdateTaskTags(task, user.Id);
                }

                context.SaveChanges();
            }
        }

        public bool IsTaskExist(int taskId)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                return context.Tasks.FirstOrDefault(q => q.Id == taskId) != null;
            }
        }

        private void UpdateTaskUsers(Entities.CTask task)
        {
            foreach (var user in task.ConnectedUsers)
            {
                AddTaskToUserHistory(user, (int)task.Id);
            }
        }

        private void UpdateTaskTags(Entities.CTask task, int userId)
        {
            var tagController = new TagControllerEF();
            foreach (var tag in task.Tags)
            {
                if (!tagController.IsTagExist(tag.Name, userId))
                { 
                    tag.Id = tagController.CreateTag(tag.Name, userId).Id;
                    AddTaskTagConnection((int)task.Id, tag.Id);
                }
                else
                {
                    AddTaskTagConnection((int)task.Id, tagController.GetTag(tag.Name, userId).Id);
                }
            }
        }

        private void CleanTaskTagConnection(int taskId)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                var connections = context.TasksTags.Where(q => q.TaskId == taskId);
                foreach (var connection in connections)
                {
                    DeleteTaskTagConnection(taskId, connection.TagId);
                }
            }
             
        }

        private void AddTaskTagConnection(int taskId, int tagId)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                var taskTagConnection = context.TasksTags.FirstOrDefault(q => q.TaskId == taskId && q.TagId == tagId);
                if (taskTagConnection == null)
                {
                    taskTagConnection = new TasksTag
                    {
                        TaskId = taskId,
                        TagId = tagId
                    };
                    context.TasksTags.Add(taskTagConnection);
                    context.Entry(taskTagConnection).State = EntityState.Added;
                    context.SaveChanges();
                }
            }
        }

        private void DeleteTaskTagConnection(int taskId, int tagId)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                var taskTagConnection = context.TasksTags.FirstOrDefault(q => q.TaskId == taskId && q.TagId == tagId);

                context.TasksTags.Remove(taskTagConnection);
                context.Entry(taskTagConnection).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }
    }
}
