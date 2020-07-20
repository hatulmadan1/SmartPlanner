using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Refit;

namespace LogicForUI.Interfaces
{
    public interface ITaskWebInterface
    {
        [Get("/api/tasks/history/{userName}")]
        Task<string> GetUserHistory(string userName);

        [Post("/api/tasks/tohistory/{userName}/task/{taskId}")]
        Task AddTaskToUserHistory(string userName, int taskId);

        [Delete("/api/tasks/delete/{userName}/task/{taskId}")]
        //[Headers("Content-Type: application/json; charset=UTF-8")]
        Task DeleteTaskFromUserHistory(string userName, int taskId);

        [Get("/api/tasks/alltagnames/{userName}")]
        Task<IReadOnlyList<string>> ReadAllTagNamesFromDB(string userName);

        [Get("/api/tasks/foruser/{userName}/tag/{requiredName}")]
        Task<string> ReadFullTagFromDB(string userName, string requiredName);

        [Post("/api/tasks/update")]
        [Headers("Content-Type: application/json; charset=UTF-8")]
        Task UpdateTask([Body]string userTaskTransferString);

        [Get("/api/tasks/new/{taskName}/for/{userName}")]
        Task<int> CreateTask(string taskName, string userName);

        [Get("/api/tasks/existance/{taskId}")]
        Task<bool> IsTaskExist(int taskId);
    }
}
