using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using EFDataManager;

namespace ServerOnWeb.Controllers
{
    [RoutePrefix("api/users")]
    public class UserController : ApiController
    {
        private readonly UserControllerEF _userControllerEf;

        public UserController()
        {
            _userControllerEf = new UserControllerEF();
        }

        [HttpGet, Route("")]
        public IReadOnlyList<string> GetAllUsers() //ok
        {
            return _userControllerEf.GetAllUsers();
        }

        [HttpGet, Route("allfortask/{taskId}")]
        public IReadOnlyCollection<string> GetAllUsersOfTask(int taskId) //ok
        {
            return _userControllerEf.GetAllUsersOfTask(taskId);
        }

        [HttpGet, Route("user/{name}")]
        public Entities.User GetUserByName(string name) //ok
        {
            return _userControllerEf.GetUserByName(name);
        }

        [HttpPost, Route("new/{userName}")]
        public void CreateUser(string userName) //ok
        {
            _userControllerEf.CreateUser(userName);
        }

        [HttpDelete, Route("delete/{userName}")]
        public void DeleteUser(string userName) //ok
        {
            _userControllerEf.DeleteUser(userName);
        }

        [HttpGet, Route("existance/{userName}")] //ok
        public bool IsUserExist(string userName)
        {
            return _userControllerEf.IsUserExist(userName);
        }
    }
}