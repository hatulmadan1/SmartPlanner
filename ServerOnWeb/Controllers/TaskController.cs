using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls.WebParts;
using EFDataManager;
using Entities;
using Entities.TransferEntities;
using Newtonsoft.Json;
using Refit;

namespace ServerOnWeb.Controllers
{
    [RoutePrefix("api/tasks")]
    
    public class TaskController : ApiController
    {
        private readonly TaskControllerEF _taskControllerEf;

        public TaskController()
        {
            _taskControllerEf = new TaskControllerEF();
        }

        public string ToDeserializable(string s)
        {
            return s.Replace(@"\", @"").TrimStart('"').TrimStart('\\').TrimEnd('"').TrimEnd('\\');
        }

        [HttpGet, Route("history/{userName}")]
        public string GetUserHistory(string userName)
        {
            return JsonConvert.SerializeObject(_taskControllerEf.GetUserHistory(userName));
        }

        [HttpPost, Route("tohistory/{userName}/task/{taskId}")]
        public void AddTaskToUserHistory(string userName, int taskId)
        {
            _taskControllerEf.AddTaskToUserHistory(userName, taskId);
        }

        [HttpDelete, Route("delete/{userName}/task/{taskId}")]
        public void DeleteTaskFromUserHistory(string userName, int taskId)
        {
            _taskControllerEf.DeleteTaskFromUserHistory(userName, taskId);
        }

        [HttpGet, Route("alltagnames/{userName}")]
        public IReadOnlyList<string> ReadAllTagNamesFromDB(string userName)
        {
            return _taskControllerEf.ReadAllTagNamesFromDB(userName);
        }

        [HttpGet, Route("foruser/{userName}/tag/{requiredName}")]
        public string ReadFullTagFromDB(string userName, string requiredName)
        {
            return JsonConvert.SerializeObject(_taskControllerEf.ReadFullTagFromDB(userName, requiredName));
        }

        [HttpPost, Route("update")]
        [Headers("Content-Type: application/json; charset=UTF-8")]
        public void UpdateTask([FromBody]string userTaskTransferString)
        {
            UserTaskTransfer userTaskTransfer =
                JsonConvert.DeserializeObject<UserTaskTransfer>(ToDeserializable(userTaskTransferString));
            _taskControllerEf.UpdateTask(userTaskTransfer.User, userTaskTransfer.Task);
        }

        [HttpGet, Route("new/{taskName}/for/{userName}")]
        public int CreateTask(string taskName, string userName)
        {
            return _taskControllerEf.CreateTask(taskName, userName);
        }

        [HttpGet, Route("existance/{taskId}")]
        public bool IsTaskExist(int taskId)
        {
            return _taskControllerEf.IsTaskExist(taskId);
        }
    }
}