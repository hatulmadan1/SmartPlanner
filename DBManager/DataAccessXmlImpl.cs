using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Entities;
using Newtonsoft.Json;
using System.IO;

namespace DBManager
{
    public class DataAccessXmlImpl //: IDataAccess
    {
        public List<CTask> GetUserHistory(string userName)
        {
            String backup = File.ReadAllText(GetUserHistoryPath(userName));
            List<CTask> result = JsonConvert.DeserializeObject<List<Entities.CTask>>(backup);
            return result != null ? result : new List<CTask>();
        }
        public void SaveUserHistory(string userName, IReadOnlyList<CTask> history)
        {
            File.WriteAllText(GetUserHistoryPath(userName), JsonConvert.SerializeObject(history));
        }
        public void AddTaskToUserHistory(string userName, CTask t)
        {
            var history = new List<CTask>(GetUserHistory(userName));
            history.Add(t);
            SaveUserHistory(userName, history);
        }

        public void DeleteTaskFromUserHistory(string userName, CTask task)
        {
            var history = new List<CTask>(GetUserHistory(userName));
            foreach (CTask t in history)
            {
                if (t.Id == task.Id)
                {
                    history.Remove(t);
                    break;
                }
            }
            SaveUserHistory(userName, history);
        }
        public IReadOnlyList<string> ReadAllTagNamesFromDB(string userName)
        {
            XDocument DBXmlVersion = XDocument.Load(GetPath(userName));

            var items = from element in DBXmlVersion.Element("tags").Elements("tag").Elements("name")
                        orderby element.Value
                        select element.Value;

            return items.ToList<string>();
        }

        public Tag ReadFullTagFromDB(string userName, string requiredName)
        {
            XDocument DBXmlVersion = XDocument.Load(GetPath(userName));

            var items = from element in DBXmlVersion.Element("tags").Elements("tag")
                where element.Element("name").Value == requiredName
                select new Tag
                {
                    Name = element.Element("name").Value,
                    Duration = TimeSpan.Parse(element.Element("duration").Value),
                    Quantity = int.Parse(element.Element("quantity").Value)
                };

            if (items.ToList<Tag>().Count == 0)
            {
                return null;
            }

            return items.ToList<Tag>()[0];
        }

        public CTask WriteToDB(User user, CTask task)
        {
            XDocument DBStatServ = XDocument.Load(GetPath(user.Name));

            foreach (Tag currentTag in task.Tags)
            {
                Tag t = ReadFullTagFromDB(user.Name, currentTag.Name);

                if (t == null)
                {
                    DBStatServ.Element("tags").Add(new XElement("tag",
                        new XElement("name", currentTag.Name), 
                        new XElement("duration", task.ActualDuration.ToString()),
                        new XElement("quantity", "1")));
                }
                else
                {
                    t.Quantity++;

                    foreach (XElement tag in DBStatServ.Element("tags").Elements("tag"))
                    {
                        string name = tag.Element("name").Value;
                        TimeSpan duration = TimeSpan.Parse(tag.Element("duration").Value);
                        int quantity = int.Parse(tag.Element("quantity").Value);

                        if (name == currentTag.Name)
                        {
                            duration = TimeSpan.FromSeconds((duration.TotalSeconds * quantity + task.ActualDuration.TotalSeconds) / t.Quantity );
                            tag.Element("quantity").Value = t.Quantity.ToString();
                            break;
                        }
                    }
                }
            }

            DBStatServ.Save(GetPath(user.Name));
            return task;
        }

        private string GetPath(string userName)
        {
            return @"..\..\..\DBManager\DBStatistics" + userName + ".xml";
        }

        private string GetUserHistoryPath(string userName)
        {
            return @"..\..\..\DBManager\TaskList" + userName + ".json";
        }
    }
}
