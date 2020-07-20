using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;

namespace BusinessLogic
{
    public interface ITimePredictor
    {
        Double PredictTimeForTag(string userName, Tag tag);
        TimeSpan PredictTimeForTask(string userName, CTask task);
    }
}
