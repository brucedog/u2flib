using System.Data.Entity;

namespace DemoU2FSite.Repository
{
    public class DataContext : DbContext
    {
        public DataContext() : base("DataContext"){}

        public DbSet<User> Users { get; set; }

        public DbSet<Device> Devices { get; set; }

        public DbSet<AuthenticationRequest> AuthenticationRequests { get; set; }
    }
}