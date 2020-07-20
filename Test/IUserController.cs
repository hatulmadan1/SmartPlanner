using System.Collections.Generic;

namespace Server
{
    public interface IUserController
    {
        //IUserController GetUserController();
        IReadOnlyList<string> GetAllUsers();
        IReadOnlyCollection<string> GetAllUsersOfTask(int taskId);
        Entities.User GetUserByName(string userName);
        void CreateUser(string userName);
        void DeleteUser(string userName);
        bool IsUserExist(string userName);
    }
}
