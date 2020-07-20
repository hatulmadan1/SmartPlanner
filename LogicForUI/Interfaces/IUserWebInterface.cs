using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;
using Server;

namespace LogicForUI.Interfaces
{
    public interface IUserWebInterface 
    {
        [Get("/api/users")]
        Task<IReadOnlyList<string>> GetAllUsers();

        [Get("/api/users/allfortask/{taskId}")]
        Task<IReadOnlyCollection<string>> GetAllUsersOfTask(int taskId);

        [Get("/api/users/user/{name}")]
        Task<Entities.User> GetUserByName(string name);

        [Post("/api/users/new/{userName}")]
        Task CreateUser(string userName);

        [Delete("/api/users/delete/{userName}")]
        Task DeleteUser(string userName);

        [Get("/api/users/existance/{userName}")]
        Task<bool> IsUserExist(string userName);
    }
}
