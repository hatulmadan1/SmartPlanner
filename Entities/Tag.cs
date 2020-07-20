using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Entities
{
    [Serializable]
    public class Tag
    {
        public String Name { get; set; }
        public int Id { get; set; }
        public TimeSpan Duration { get; set; }
        public int Quantity { get; set; }

        public Tag()
        {
            
        }

        public override String ToString()
        {
            return "#" + this.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
