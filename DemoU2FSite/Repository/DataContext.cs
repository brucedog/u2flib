using System;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace DemoU2FSite.Repository
{
    public class DataContext : DbContext, IDataContext
    {
        public DataContext() : base("DataContext"){}

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                      eve.Entry.Entity.GetType().Name, eve.Entry.State);

                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                // zero means no items were saved to the DB
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                // zero means no items were saved to the DB
                return 0;
            }
        }

        public DbSet<User> Users { get; set; }

        public DbSet<DeviceRegistration> Devices { get; set; }

        public DbSet<AuthenticationRequest> AuthenticationRequests { get; set; }
    }
}