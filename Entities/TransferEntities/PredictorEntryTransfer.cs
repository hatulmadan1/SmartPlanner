using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.TransferEntities
{
    [Serializable]
    public class PredictorEntryTransfer
    {
        public Entities.User User { get; set; }
        public TimeSpan Duration { get; set; }
        public IReadOnlyList<CTask> TaskList { get; set; }
    }
}
