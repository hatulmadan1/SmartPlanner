using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EFDataManager;
using Server;
using User = Entities.User;

namespace EFDataManager
{
    public class UserControllerEF : IUserController
    {
        private EntityConverter converter;

        public UserControllerEF()
        {
            converter = new EntityConverter();
        }
        public IReadOnlyList<string> GetAllUsers()
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                return context.Users.
                    Select(q => q.Name).
                    ToList<string>();
            }
        }

        public IReadOnlyCollection<string> GetAllUsersOfTask(int taskId)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                return context.UsersTasks.
                    Where(q => q.TaskId == taskId).
                    Select(q => q.User.Name).ToHashSet<string>();
            }
        }

        public Entities.User GetUserByName(string name)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                return converter.GetUser(
                    context.Users.FirstOrDefault(
                        q => q.Name.Equals(name)
                        )
                    );
            }
        }

        public void CreateUser(string userName)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                var user = new User { Name = userName};
                if (IsUserExist(userName))
                {
                    return;
                }
                context.Users.Add(user);
                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
                
            }
        }

        public void DeleteUser(string userName)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                var user = context.Users.FirstOrDefault(q => q.Name.Equals(userName));
                context.Users.Remove(user);
                context.Entry(user).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public bool IsUserExist(string userName)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                return context.Users.
                    FirstOrDefault(q => q.Name.Equals(userName)) != null;
            }
        }
    }
}
