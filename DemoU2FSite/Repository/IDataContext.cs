using System.Data.Entity;

namespace DemoU2FSite.Repository
{
    public interface IDataContext
    {
        int SaveChanges();

        DbSet<User> Users { get; set; }
        
        DbSet<DeviceRegistration> Devices { get; set; }

        DbSet<AuthenticationRequest> AuthenticationRequests { get; set; }
    }
}