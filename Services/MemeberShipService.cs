using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using BaseLibrary;
using DataModels;
using u2flib;
using u2flib.Data;
using u2flib.Data.Messages;
using u2flib.Util;

namespace Services
{
    public class MemeberShipService : IMemeberShipService
    {
        private readonly IUserRepository _userRepository;
<<<<<<< HEAD

        // NOTE: THIS HAS TO BE UPDATED TO MATCH YOUR SITE/EXAMPLE
        private const string DemoAppId = "http://localhost:52701";
=======
        private const string DemoAppId = "https://localhost:44301";
>>>>>>> origin/master

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
            _userRepository.SaveUserAuthenticationRequest(userName, startedRegistration.AppId, startedRegistration.Challenge, startedRegistration.Version);

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

            RegisterResponse registerResponse = RegisterResponse.FromJson<RegisterResponse>(deviceResponse);

            var user = _userRepository.FindUser(userName);
            
            if (user == null 
                || user.AuthenticationRequest == null 
                || user.AuthenticationRequest.Count == 0)
                return false;

            // When the user is registration they should only ever have one auth request.
            AuthenticationRequest authenticationRequest = user.AuthenticationRequest.First();

            StartedRegistration startedRegistration = new StartedRegistration(authenticationRequest.Challenge, authenticationRequest.AppId);
            DeviceRegistration registration = U2F.FinishRegistration(startedRegistration, registerResponse);
            
            _userRepository.RemoveUsersAuthenticationRequests(userName);
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

            AuthenticateResponse authenticateResponse = AuthenticateResponse.FromJson<AuthenticateResponse>(deviceResponse);
            
            var device = user.DeviceRegistrations.FirstOrDefault(f=> f.KeyHandle.SequenceEqual(Utils.Base64StringToByteArray(authenticateResponse.KeyHandle)));
            
            if (device == null || user.AuthenticationRequest == null)
                return false;

            // User will have a authentication request for each device they have registered so get the one that matches the device key handle
            AuthenticationRequest authenticationRequest = user.AuthenticationRequest.First(f => f.KeyHandle.Equals(authenticateResponse.KeyHandle));
            DeviceRegistration registration = new DeviceRegistration(device.KeyHandle, device.PublicKey, device.AttestationCert, device.Counter);

            StartedAuthentication authentication = new StartedAuthentication(authenticationRequest.Challenge, authenticationRequest.AppId, authenticationRequest.KeyHandle);

            U2F.FinishAuthentication(authentication, authenticateResponse, registration);
            
            _userRepository.RemoveUsersAuthenticationRequests(user.Name);
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

        public List<ServerChallenge> GenerateServerChallenge(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            User user = _userRepository.FindUser(userName);

            if (user == null)
                return null;

            // We only want to generate challenges for un-compromised devices
            List<Device> device = user.DeviceRegistrations.Where(w => w.IsCompromised == false).ToList();

            if (device.Count == 0)
                return null;

            List<ServerChallenge> serverChallenges = new List<ServerChallenge>();
            foreach (var registeredDevice in device)
            {
                DeviceRegistration registration = new DeviceRegistration(registeredDevice.KeyHandle, registeredDevice.PublicKey, registeredDevice.AttestationCert, registeredDevice.Counter);
                StartedAuthentication startedAuthentication = U2F.StartAuthentication(DemoAppId, registration);

                serverChallenges.Add(new ServerChallenge
                {
                    appId = startedAuthentication.AppId,
                    challenge = startedAuthentication.Challenge,
                    keyHandle = startedAuthentication.KeyHandle,
                    version = startedAuthentication.Version
                });

                _userRepository.SaveUserAuthenticationRequest(userName, startedAuthentication.AppId, startedAuthentication.Challenge,
                                                              startedAuthentication.KeyHandle);
            }

            return serverChallenges;
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