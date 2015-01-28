using System.Data.Entity;
using DataModels;

namespace Repositories.Context
{
    public interface IDataContext
    {
        int SaveChanges();

        DbSet<User> Users { get; set; }
        
        DbSet<DeviceRegistration> Devices { get; set; }

        DbSet<AuthenticationRequest> AuthenticationRequests { get; set; }
    }
}