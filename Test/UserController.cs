using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Server
{
    public class UserController : IUserController
    {
        private HashSet<string> ExistingUsers;
        private static UserController instance;
        private string path = @"..\..\..\Server\ExistingUsers.json";

        private UserController()
        {
            String loaded = File.ReadAllText(path);
            ExistingUsers = JsonConvert.DeserializeObject<HashSet<string>>(loaded);
        }

        public static IUserController GetUserController()
        {
            return instance ?? (instance = new UserController());
        }

        public IReadOnlyList<string> GetAllUsers()
        {
            return new List<string>(instance.ExistingUsers);
        }

        public IReadOnlyCollection<string> GetAllUsersOfTask(int taskId)
        {
            throw new NotImplementedException();
        }

        public void CreateUser(string userName)
        {
            if (instance.IsUserExist(userName))
            {
                return;
            }

            string pathSchedule = @"..\..\..\Server\Schedule" + userName + ".json";
            string pathTaskList = @"..\..\..\DBManager\TaskList" + userName + ".json";
            string pathDBStatistics = @"..\..\..\DBManager\DBStatistics" + userName + ".xml";
            File.WriteAllText(pathSchedule, "");
            File.WriteAllText(pathTaskList, "");

            XDocument db = new XDocument();
            XElement root = new XElement("tags");
            db.Add(root);
            db.Save(pathDBStatistics);

            instance.ExistingUsers.Add(userName);

            instance.Save();
        }

        public void DeleteUser(string userName)
        {
            if (!instance.IsUserExist(userName))
            {
                return;
            }

            instance.ExistingUsers.Remove(userName);

            instance.Save();
        }

        public bool IsUserExist(string userName)
        {
            return instance.ExistingUsers.Contains(userName);
        }

        private void Save()
        {
            File.WriteAllText(instance.path, JsonConvert.SerializeObject(instance.ExistingUsers));
        }

        public Entities.User GetUserByName(string userName)
        {
            return new Entities.User {Name = userName};
        }
    }
}
