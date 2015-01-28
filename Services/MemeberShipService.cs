using System;
using System.Linq;
using System.Security.Cryptography;
using DataModels;
using Repositories;
using u2flib;
using u2flib.Data.Messages;
using DeviceRegistration = u2flib.Data.DeviceRegistration;

namespace Services
{
    public class MemeberShipService : IMemeberShipService
    {
        private readonly IUserRepository _userRepository;
        private const string DemoAppId = "http://localhost:52701";

        public MemeberShipService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        #region IMemeberShipService methods

        public ServerRegisterResponse GenerateServerRegistration(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                return null;

            StartedRegistration startedRegistration = U2F.StartRegistration(DemoAppId);
            string hashedPassword = HashPassword(password);

            _userRepository.AddUser(userName, hashedPassword);
            _userRepository.AddAuthenticationRequest(userName, startedRegistration.AppId, startedRegistration.Challenge, startedRegistration.Version);

            return new ServerRegisterResponse
            {
                AppId = startedRegistration.AppId,
                Challenge = startedRegistration.Challenge,
                Version = startedRegistration.Version
            };
        }

        public bool CompleteRegistration(string userName, string deviceResponse)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(deviceResponse))
                return false;

            RegisterResponse registerResponse = RegisterResponse.FromJson(deviceResponse);

            var user = _userRepository.FindUser(userName);

            if (user == null || user.AuthenticationRequest == null)
                return false;

            StartedRegistration startedRegistration = new StartedRegistration(user.AuthenticationRequest.Challenge, user.AuthenticationRequest.AppId);
            DeviceRegistration registration = U2F.FinishRegistration(startedRegistration, registerResponse);
            
            _userRepository.RemoveUsersAuthenticationRequest(userName);
            _userRepository.AddDeviceRegistration(userName, registration.AttestationCert, registration.Counter, registration.KeyHandle, registration.PublicKey);

            return true;
        }

        public bool AuthenticateUser(string userName, string deviceResponse)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(deviceResponse))
                return false;
            
            User user = _userRepository.FindUser(userName);
            if (user == null)
                return false;

            AuthenticateResponse authenticateResponse = AuthenticateResponse.FromJson(deviceResponse);

            byte[] keyHandle = Base64StringToByteArray(authenticateResponse.KeyHandle);

            var device = user.DeviceRegistrations.FirstOrDefault();

            if (device == null || user.AuthenticationRequest == null)
                return false;

            DeviceRegistration registration = new DeviceRegistration(device.KeyHandle, device.PublicKey, device.AttestationCert, device.Counter);

            StartedAuthentication authentication = new StartedAuthentication(user.AuthenticationRequest.Challenge, user.AuthenticationRequest.AppId, user.AuthenticationRequest.KeyHandle);

            U2F.FinishAuthentication(authentication, authenticateResponse, registration);
            
            _userRepository.RemoveUsersAuthenticationRequest(user.Name);
            _userRepository.UpdateDeviceCounter(user.Name, device.PublicKey, registration.Counter);

            return true;
        }

        public bool IsUserRegistered(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;

            User user = _userRepository.FindUser(userName);

            if (user == null)
                return false;

            return user.DeviceRegistrations.Count > 0;
        }

        public ServerChallenge GenerateServerChallenge(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            User user = _userRepository.FindUser(userName);

            if (user == null)
                return null;

            // TODO  this would have to change 
            var device = user.DeviceRegistrations.FirstOrDefault();

            if (device == null)
                return null;

            DeviceRegistration registration = new DeviceRegistration(device.KeyHandle, device.PublicKey, device.AttestationCert, device.Counter);
            StartedAuthentication startedAuthentication = U2F.StartAuthentication(DemoAppId, registration);

            _userRepository.SaveUserAuthenticationRequest(userName, startedAuthentication.AppId, startedAuthentication.Challenge,
                                                          startedAuthentication.KeyHandle);
            

            return new ServerChallenge
            {
                AppId = startedAuthentication.AppId,
                Challenge = startedAuthentication.Challenge,
                KeyHandle = startedAuthentication.KeyHandle,
                Version = startedAuthentication.Version
            };
        }

        public bool IsValidUserNameAndPassword(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                return false;

            User user = _userRepository.FindUser(userName);

            if (user == null)
                return false;

            string hashedPassword = HashPassword(password);

            return user.Password.Equals(hashedPassword);
        }

        #endregion

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

        private string HashPassword(string password)
        {
            // TODO salt password
            byte[] bytes = new byte[password.Length * sizeof(char)];
            Buffer.BlockCopy(password.ToCharArray(), 0, bytes, 0, bytes.Length);
            var hasher = SHA256Managed.Create();
            var results = hasher.ComputeHash(bytes);

            return Convert.ToBase64String(results);
        }
    }
}