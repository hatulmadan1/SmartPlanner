using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.TransferEntities;
using Refit;

namespace LogicForUI.Interfaces
{
    public interface IScheduleWebInterface
    {
        [Get("/api/schedule/foruser/{userName}")]
        Task<string> GetSchedule(string userName);

        [Get("/api/schedule/predict/{userName}/{durationSeconds}")]
        Task<string> PredictSchedule(string userName, int durationSeconds);

        [Post("/api/schedule/toschedule/user/{userId}/task/{taskId}")]
        Task AddTaskToSchedule(int userId, int taskId);

        [Post("/api/schedule/toschedule/{userName}/task/{taskId}")]
        Task AddTaskToSchedule(string userName, int taskId);

        [Delete("/api/schedule/delete/{userId}/task/{taskId}")]
        Task<bool> DeleteTaskFromSchedule(int userId, int taskId);
    }
}
