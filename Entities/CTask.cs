using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Entities
{
    public class CTask
    {
        public int Id { get; set; }
        public String TaskName { get; set; }
        public List<string> ConnectedUsers { get; set; }
        public List<Tag> Tags { get; set; }
        public TimeSpan ActualDuration { get; set; }
        public TimeSpan PredictedDuration { get; set; }
        private bool _isSetActualDuration = false;

        public CTask()
        {
            Id = -1;
            Tags = new List<Tag>();
            ConnectedUsers = new List<string>();
        }

        public void SetId()
        {
            this.Id = GetFreshID();
        }

        public CTask(CTask t)
        {
            this.Id = t.Id;
            this.TaskName = t.TaskName;
            this.ConnectedUsers = t.ConnectedUsers;
            this.Tags = t.Tags;
            this.PredictedDuration = t.PredictedDuration;
            this.ActualDuration = t.ActualDuration;
        }

        public void AddTag(String tagName)
        {
            this.Tags.Add(new Tag { Name = tagName });
        }

        public void DeleteTag(String tagName)
        {
            foreach (var currentTag in this.Tags.Where(currentTag => currentTag.Name.CompareTo(tagName) == 0))
            {
                Tags.Remove(currentTag);
                break;
            }
        }

        public void ConnectUser(string userName)
        {
            this.ConnectedUsers.Add(userName);
        }

        public void DisconnectUser(string userName)
        {
            this.ConnectedUsers.Remove(userName);
        }

        public void SetActualDuration(TimeSpan duration)
        {
            if (!_isSetActualDuration)
            {
                this.ActualDuration = duration;
                _isSetActualDuration = true;

                foreach (Tag currentTag in Tags)
                {
                    currentTag.Duration = duration;
                }
            }
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            int i = 1;

            sb.AppendLine("\nName: " + this.TaskName);
            sb.Append("Connected users: ");
            foreach (string user in ConnectedUsers)
            {
                sb.Append(user + "; ");
                i++;
                if (i % 3 == 0)
                {
                    sb.Append("\n");
                }
            }
            if (i % 3 != 0) { sb.Append("\n"); }

            i = 1;
            sb.Append("Tags: ");
            foreach(Tag tag in this.Tags)
            {
                sb.Append("#" + tag.Name + " ");
                i++;
                if (i % 3 == 0)
                {
                    sb.Append("\n");
                }
            }
            if (i % 3 != 0) { sb.Append("\n"); }
            sb.AppendLine("Predicted duration: " + this.PredictedDuration.ToString());
            sb.AppendLine("Actual duration: " + this.ActualDuration.ToString());
            return sb.ToString();
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        private int GetFreshID()
        {
            string path = @"..\..\..\Server\TaskID.json";
            int id = int.Parse(File.ReadAllText(path));
            id++;
            File.WriteAllText(path, id.ToString());
            return id;
        }
    }
}
