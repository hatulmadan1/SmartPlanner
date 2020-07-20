using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.TransferEntities
{
    public class UserTaskTransfer
    {
        public Entities.CTask Task { get; set; }
        public Entities.User User { get; set; }
    }
}
