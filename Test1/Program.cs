using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection =
                System.Configuration.ConfigurationManager.
                    ConnectionStrings["SmartPlannerEntities"].ConnectionString;
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                var user = context.Users.FirstOrDefault(q => q.Id == 1);
                Console.WriteLine(user.Name);
            }

        }
    }
}
