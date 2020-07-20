using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using EFDataManager;

namespace ServerOnWeb.Controllers
{
    [RoutePrefix("api/tags")]
    public class TagController : ApiController
    {
        private readonly TagControllerEF _tagControllerEf;

        public TagController()
        {
            _tagControllerEf = new TagControllerEF();
        }

        [HttpGet, Route("allfortask/{taskId}")]
        public IReadOnlyList<Entities.Tag> GetAllTagsOfTask(int taskId)
        {
            return _tagControllerEf.GetAllTagsOfTask(taskId);
        }

        [HttpGet, Route("tag/{tagName}/foruser/{userId}")]
        public Entities.Tag GetTag(string tagName, int userId)
        {
            return _tagControllerEf.GetTag(tagName, userId);
        }

        [HttpPost, Route("new/{tagName}/foruser/{userId}")]
        public Entities.Tag CreateTag(string tagName, int userId)
        {
            return _tagControllerEf.CreateTag(tagName, userId);
        }

        [HttpGet, Route("existance/{tagName}/foruser/{userId}")]
        public bool IsTagExist(string tagName, int userId)
        {
            return _tagControllerEf.IsTagExist(tagName, userId);
        }

        [HttpPost, Route("update")]
        public void UpdateTag([FromBody]Entities.Tag tag) //The only not tested guy :)
        {
            _tagControllerEf.UpdateTag(tag);
        }
    }
}