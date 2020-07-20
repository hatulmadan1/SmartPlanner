using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EFDataManager;

namespace EFDataManager
{
    public class TagControllerEF
    {
        public Entities.Tag CreateTag(string tagName, int userId)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                Tag tag = new Tag
                {
                    Duration = TimeSpan.Zero,
                    Name = tagName,
                    Quantity = 1,
                    UserId = userId
                };
                var toBeReturned = context.Tags.Add(tag);
                context.Entry(tag).State = EntityState.Added;
                context.SaveChanges();
                return new EntityConverter().GetTag(toBeReturned);
            }
        }

        public bool IsTagExist(string tagName, int userId)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                return context.Tags.FirstOrDefault(q => q.Name.Equals(tagName) && q.UserId == userId) != null;
            }
        }

        public Entities.Tag GetTag(string tagName, int userId)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                return new EntityConverter().GetTag(context.Tags.FirstOrDefault(q => q.Name.Equals(tagName) && q.UserId == userId));
            }
        }

        public void UpdateTag(Entities.Tag tag)
        {
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                var dbtag = context.Tags.FirstOrDefault(q => q.Id == tag.Id);
                dbtag.Duration = TimeSpan.FromSeconds(
                    (dbtag.Duration.TotalSeconds * dbtag.Quantity + tag.Duration.TotalSeconds) / (dbtag.Quantity + 1)
                    );
                dbtag.Quantity++;
                context.Entry(dbtag).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public IReadOnlyList<Entities.Tag> GetAllTagsOfTask(int taskId)
        {
            var converter = new EntityConverter();
            using (SmartPlannerEntities context = new SmartPlannerEntities())
            {
                return context.TasksTags.
                    Where(q => q.TaskId == taskId).
                    Select(q => q.Tag).
                    ToList().Select(q => converter.GetTag(q)).ToList<Entities.Tag>();
            }
        }
    }
}
