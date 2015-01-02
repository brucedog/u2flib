using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using DemoU2FSite.Models;
using DemoU2FSite.Repository;
using u2flib;
using u2flib.Data;
using u2flib.Data.Messages;
using DeviceRegistration = u2flib.Data.DeviceRegistration;

namespace DemoU2FSite.Services
{
    public class MemeberShipService : IMemeberShipService
    {
        private IDataContext _dataContext;
        private const string DemoAppId = "http://localhost:52701";

        public MemeberShipService(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public bool IsUserRegistered(string userName, string password)
        {
            //   string hashedPassword = HashPassword(password);
            return _dataContext.Users.Any(w => w.Name.Equals(userName.Trim()));
        }

        private string HashPassword(string password)
        {
            // TODO salt password
            byte[] bytes = new byte[password.Length*sizeof (char)];
            Buffer.BlockCopy(password.ToCharArray(), 0, bytes, 0, bytes.Length);
            var hasher = SHA256Managed.Create();
            var results = hasher.ComputeHash(bytes);

            return Convert.ToBase64String(results);
        }

        public ServerChallenge GenerateServerChallenge(string userName)
        {
            var user = _dataContext.Users.FirstOrDefault(w => w.Name.Equals(userName));

            if (user == null)
                return null;
            // TODO  this would have to change 
            var device = user.DeviceRegistrations.FirstOrDefault();

            if (device == null)
                return null;

            DeviceRegistration registration = new DeviceRegistration(device.KeyHandle, device.PublicKey, device.AttestationCert, device.Counter);
            StartedAuthentication startedAuthentication = U2F.StartAuthentication(DemoAppId, registration);
            user.AuthenticationRequest = new AuthenticationRequest
                                         {
                                             AppId = startedAuthentication.AppId,
                                             Challenge = startedAuthentication.Challenge,
                                             KeyHandle = startedAuthentication.KeyHandle
                                         };
            _dataContext.SaveChanges();

            return new ServerChallenge
                   {
                       AppId = startedAuthentication.AppId,
                       Challenge = startedAuthentication.Challenge,
                       KeyHandle = startedAuthentication.KeyHandle,
                       Version = startedAuthentication.Version
                   };
        }

        public bool HasChallengeForUser(string userName)
        {
            return _dataContext.Users.Any(w => w.Name.Equals(userName.Trim())
                && w.DeviceRegistrations != null);
        }

        public bool AuthenticateUser(string userName, string deviceResponse)
        {
            var user = _dataContext.Users.FirstOrDefault(w => w.Name.Equals(userName));
                
            if(user == null)
                return false;

            AuthenticateResponse authenticateResponse = AuthenticateResponse.FromJson(deviceResponse);
            byte[] keyHandle = Base64StringToByteArray(authenticateResponse.KeyHandle);
            // TODO get matching keyhandle
            var device = user.DeviceRegistrations.FirstOrDefault();

            if (device == null || user.AuthenticationRequest == null)
                return false;

            DeviceRegistration registration = new DeviceRegistration(device.KeyHandle, device.PublicKey, device.AttestationCert, device.Counter);
            
            StartedAuthentication authentication = new StartedAuthentication(user.AuthenticationRequest.Challenge, user.AuthenticationRequest.AppId, user.AuthenticationRequest.KeyHandle);
            
            U2F.FinishAuthentication(authentication, authenticateResponse, registration);
            
            user.AuthenticationRequest = null;
            _dataContext.SaveChanges();

            return true;
        }

        public ServerRegisterResponse GenerateServerRegisteration(string userName, string password)
        {
            StartedRegistration startedRegistration = U2F.StartRegistration(DemoAppId);
            string hashedPassword = HashPassword(password);
            _dataContext.Users.Add(new User
                                   {
                                       Name = userName,
                                       Password = hashedPassword,
                                       CreatedOn = DateTime.Now,
                                       UpdatedOn = DateTime.Now,
                                       AuthenticationRequest = new AuthenticationRequest
                                                               {
                                                                   AppId = startedRegistration.AppId,
                                                                   Challenge = startedRegistration.Challenge,
                                                                   Version = startedRegistration.Version
                                                               }
                                   });
            _dataContext.SaveChanges();

            return new ServerRegisterResponse
                   {
                       AppId = startedRegistration.AppId,
                       Challenge = startedRegistration.Challenge,
                       Version = startedRegistration.Version
                   };
        }

        public void CompleteRegisteration(string userName, string deviceResponse)
        {
            RegisterResponse registerResponse = RegisterResponse.FromJson(deviceResponse);

            var user = _dataContext.Users.FirstOrDefault(w => w.Name.Equals(userName));

            if (user == null && user.AuthenticationRequest == null)
                return;

            StartedRegistration startedRegistration = new StartedRegistration(user.AuthenticationRequest.Challenge, user.AuthenticationRequest.AppId);
            DeviceRegistration registration = U2F.FinishRegistration(startedRegistration, registerResponse);

            user.UpdatedOn = DateTime.Now;
            user.AuthenticationRequest = null;
            user.DeviceRegistrations.Add(new Repository.DeviceRegistration
                                         {
                                             AttestationCert = registration.AttestationCert,
                                             Counter = registration.Counter,
                                             CreatedOn = DateTime.Now,
                                             KeyHandle = registration.KeyHandle,
                                             PublicKey = registration.PublicKey
                                         });
            _dataContext.SaveChanges();
        }

        private byte[] Base64StringToByteArray(string input)
        {
            input = input.Replace('-', '+');
            input = input.Replace('_', '/');

            int mod4 = input.Length % 4;
            if (mod4 > 0)
            {
                input += new string('=', 4 - mod4);
            }

            return Convert.FromBase64String(input);
        }
    }
}