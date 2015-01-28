using System;
using System.Data.Entity;
using DataModels;

namespace Repositories.Context
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