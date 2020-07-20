using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                context.UsersTasks.FirstOrDefault();
            }
        }
    }
}
