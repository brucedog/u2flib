using System.Collections.Generic;
using DemoU2FSite.Models;
using u2flib;
using u2flib.Data;
using u2flib.Data.Messages;

namespace DemoU2FSite.Services
{
    public class MemeberShipService : IMemeberShipService
    {
        public static Dictionary<string, string> storage = new Dictionary<string, string>();
        private const string DemoAppId = "http://localhost:52701";

        public string GetDeviceRegistration(string name, string password)
        {
            return string.Empty;
        }

        public bool IsUserRegistered(string userName, string password)
        {
            string deviceRegistration;
            return storage.TryGetValue(userName.Trim(), out deviceRegistration);
        }

        public ServerChallenge GenerateServerChallenge(string userName)
        {
            string deviceRegistration = storage[userName.Trim()];

            DeviceRegistration registration = DeviceRegistration.FromJson(deviceRegistration);
            StartedAuthentication startedAuthentication = U2F.StartAuthentication(DemoAppId, registration);
            storage.Add(startedAuthentication.Challenge, startedAuthentication.ToJson());

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
            string deviceRegistration;
            return storage.TryGetValue(userName.Trim(), out deviceRegistration);
        }

        public bool AuthenticateUser(string userName, string deviceResponse)
        {
            string deviceRegistration;
            if (storage.TryGetValue(userName, out deviceRegistration))
            {
                DeviceRegistration registration = DeviceRegistration.FromJson(deviceRegistration);
                AuthenticateResponse authenticateResponse = AuthenticateResponse.FromJson(deviceResponse);
                string challenge = authenticateResponse.GetClientData().Challenge;
                string theChallenge;

                if (storage.TryGetValue(challenge, out theChallenge))
                {
                    StartedAuthentication authentication = StartedAuthentication.FromJson(theChallenge);
                    storage.Remove(theChallenge);
                    U2F.FinishAuthentication(authentication, authenticateResponse, registration);

                    return true;
                }
            }

            return false;
        }

        public ServerRegisterResponse GenerateServerRegisteration(string userName, string password)
        {
            // user has already tried register
            if (IsUserRegistered(userName, string.Empty))
            {
                storage.Remove(userName.Trim());
            }

            StartedRegistration startedRegistration = U2F.StartRegistration(DemoAppId);
            string startedRegistrationJson = startedRegistration.ToJson();
            storage.Add(userName.Trim(), startedRegistrationJson);

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
            string theChallenge;
            if (storage.TryGetValue(userName, out theChallenge))
            {
                StartedRegistration startedRegistration = StartedRegistration.FromJson(theChallenge);
                DeviceRegistration registration = U2F.FinishRegistration(startedRegistration, registerResponse);
                storage.Remove(userName);
                storage.Add(userName.Trim(), registration.ToJson());
            }
        }
    }
}