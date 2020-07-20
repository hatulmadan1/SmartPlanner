using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager;
using Entities;

namespace BusinessLogic
{
    public class TimePredictorImpl : ITimePredictor
    {
        private IDataAccess DataSource;

        const int timeStub = 3661;

        public TimePredictorImpl(IDataAccess dataAccess)
        {
            DataSource = dataAccess;
        }
        public Double PredictTimeForTag(string userName, Tag tag)
        {
            Tag tagHistory = DataSource.ReadFullTagFromDB(userName, tag.Name);

            if (tagHistory == null)
            {
                return timeStub;
            }

            return tagHistory.Duration.TotalSeconds;
        }

        public TimeSpan PredictTimeForTask(string userName, CTask task)
        {
            if (task.Tags.Count == 0)
            {
                return TimeSpan.FromSeconds(timeStub);
            }

            double commonTime = task.Tags.AsParallel().
                Select(q => PredictTimeForTag(userName, q)).
                Aggregate((x, y) => x + y);

            commonTime = System.Math.Round(commonTime / task.Tags.Count);

            return TimeSpan.FromSeconds(commonTime);
        }
    }
}
