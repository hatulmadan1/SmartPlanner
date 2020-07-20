using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
