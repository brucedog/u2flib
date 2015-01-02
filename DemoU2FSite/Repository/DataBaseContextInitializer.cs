using System;
using System.Data.Entity;

namespace DemoU2FSite.Repository
{
    public class DataBaseContextInitializer : DropCreateDatabaseAlways<DataContext>    
    {
        protected override void Seed(DataContext context)
        {
            context.Users.Add(new User {Name = "Bryce", CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now});
            context.SaveChanges();
        }
    }
}