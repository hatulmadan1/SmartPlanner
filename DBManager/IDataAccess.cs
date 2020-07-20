using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;

namespace DBManager
{
    public interface IDataAccess
    {
        List<CTask> GetUserHistory(string userName);
        void SaveUserHistory(string userName, IReadOnlyList<CTask> history);
        void AddTaskToUserHistory(string userName, int taskId);
        void DeleteTaskFromUserHistory(string userName, int taskId);
        IReadOnlyList<string> ReadAllTagNamesFromDB(string userName);
        Tag ReadFullTagFromDB(string userName, string requiredName);
        CTask WriteToDB(User user, CTask task);
    }
}
