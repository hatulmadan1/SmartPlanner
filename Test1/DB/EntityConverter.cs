using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFDataManager;

namespace EFDataManager
{
    public class EntityConverter
    {
        public Entities.User GetUser(User tmpUser)
        {
            Entities.User user = new Entities.User();

            user.Id = tmpUser.Id;
            user.Name = tmpUser.Name;

            return user;
        }
        public Entities.CTask GetTask(Task tmpTask)
        {
            Entities.CTask task = new Entities.CTask();

            task.Id = tmpTask.Id;
            task.TaskName = tmpTask.Name;
            task.ActualDuration = tmpTask.ActualDuration != null ? (TimeSpan)tmpTask.ActualDuration : TimeSpan.Zero;
            task.PredictedDuration = tmpTask.PredictedDuration != null ? (TimeSpan)tmpTask.PredictedDuration : TimeSpan.Zero;
            task.Tags = new List<Entities.Tag>(new TagControllerEF().
                GetAllTagsOfTask((int)task.Id));
            if (task.Tags == null)
            {
                task.Tags = new List<Entities.Tag>();
            }

            task.ConnectedUsers = new List<string>(new UserControllerEF().
                GetAllUsersOfTask((int)task.Id));
            if (task.ConnectedUsers == null)
            {
                task.ConnectedUsers = new List<string>();
            }


            return task;
        }

        public Entities.Tag GetTag(Tag tmpTag)
        {
            Entities.Tag tag = new Entities.Tag();

            tag.Id = tmpTag.Id;
            tag.Name = tmpTag.Name;
            tag.Quantity = tmpTag.Quantity;
            tag.Duration = tmpTag.Duration;

            return tag;
        }
    }
}
