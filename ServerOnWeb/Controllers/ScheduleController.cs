using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using EFDataManager;
using Entities.TransferEntities;
using Newtonsoft.Json;

namespace ServerOnWeb.Controllers
{
    [RoutePrefix("api/schedule")]
    public class ScheduleController : ApiController
    {
        private readonly ScheduleControllerEF _scheduleControllerEf;

        public ScheduleController()
        {
            _scheduleControllerEf = new ScheduleControllerEF();
        }

        [HttpGet, Route("foruser/{userName}")]
        public string GetSchedule(string userName)
        {
            return JsonConvert.SerializeObject(_scheduleControllerEf.GetSchedule(userName));
        }

        [HttpGet, Route("predict/{userName}/{durationSeconds}")]
        public string PredictSchedule(string userName, int durationSeconds)
        {
            return JsonConvert.SerializeObject(_scheduleControllerEf.PredictSchedule(userName, durationSeconds));
        }

        [HttpPost, Route("toschedule/user/{userId}/task/{taskId}")]
        public void AddTaskToSchedule(int userId, int taskId)
        {
            _scheduleControllerEf.AddTaskToSchedule(userId, taskId);
        }

        [HttpPost, Route("toschedule/{userName}/task/{taskId}")]
        public void AddTaskToSchedule(string userName, int taskId)
        {
            _scheduleControllerEf.AddTaskToSchedule(userName, taskId);
        }

        [HttpDelete, Route("delete/{userId}/task/{taskId}")]
        public bool DeleteTaskFromSchedule(int userId, int taskId)
        {
            return _scheduleControllerEf.DeleteTaskFromSchedule(userId, taskId);
        }
    }
}