using System.Data.Entity;
using DataModels;

namespace BaseLibrary
{
    public interface IDataContext
    {
        int SaveChanges();

        DbSet<User> Users { get; set; }
        
        DbSet<Device> Devices { get; set; }

        DbSet<AuthenticationRequest> AuthenticationRequests { get; set; }
    }
}