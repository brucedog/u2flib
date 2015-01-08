using System;
using System.Linq;
using DemoU2FSite.Repository.Context;

namespace DemoU2FSite.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IDataContext _dataContext;

        public UserRepository(IDataContext context)
         {
             _dataContext = context;
         }

        public User FindUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            return _dataContext.Users.FirstOrDefault(f => f.Name.Equals(userName.Trim()));
        }

        public void UpdateDeviceCounter(string userName, byte[] devicePublicKey, uint counter)
        {
            User user = _dataContext.Users.FirstOrDefault(f => f.Name.Equals(userName.Trim()));

            if (user != null)
            {
                DeviceRegistration device = user.DeviceRegistrations.FirstOrDefault(w => w.PublicKey.Equals(devicePublicKey));

                if (device != null)
                {
                    device.Counter = counter;
                    device.UpdatedOn = DateTime.Now;

                    _dataContext.SaveChanges();
                }
            }
        }

        public void RemoveUsersAuthenticationRequest(string userName)
        {
            User user = _dataContext.Users.FirstOrDefault(f => f.Name.Equals(userName.Trim()));

            if (user != null)
            {
                user.AuthenticationRequest = null;
                user.UpdatedOn = DateTime.Now;
                
                _dataContext.SaveChanges();
            }
        }

        public void SaveUserAuthenticationRequest(string userName, string appId, string challenge, string keyHandle)
        {
            User user = _dataContext.Users.FirstOrDefault(f => f.Name.Equals(userName.Trim()));

            if(user == null)
                return;

            user.AuthenticationRequest = new AuthenticationRequest
            {
                AppId = appId,
                Challenge = challenge,
                KeyHandle = keyHandle
            };

            _dataContext.SaveChanges();
        }

        public void AddUser(string userName, string hashedPassword)
        {
            _dataContext.Users.Add(new User
            {
                Name = userName,
                Password = hashedPassword,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now
            });

            _dataContext.SaveChanges();
        }

        public void AddAuthenticationRequest(string userName, string appId, string challenge, string version)
        {
            User user = _dataContext.Users.FirstOrDefault(f => f.Name.Equals(userName));

            if(user == null)
                return;

            user.UpdatedOn = DateTime.Now;
            user.AuthenticationRequest = new AuthenticationRequest
                {
                    AppId = appId,
                    Challenge = challenge,
                    Version = version
                };

            _dataContext.SaveChanges();
        }

        public void AddDeviceRegistration(string userName, byte[] attestationCert, uint counter, byte[] keyHandle, byte[] publicKey)
        {
            User user = _dataContext.Users.FirstOrDefault(f => f.Name.Equals(userName));

            if (user == null)
                return;

            user.UpdatedOn = DateTime.Now;
            user.AuthenticationRequest = null;
            user.DeviceRegistrations.Add(new DeviceRegistration
            {
                AttestationCert = attestationCert,
                Counter = counter,
                CreatedOn = DateTime.Now,
                KeyHandle = keyHandle,
                PublicKey = publicKey
            });

            _dataContext.SaveChanges();
        }
    }
}