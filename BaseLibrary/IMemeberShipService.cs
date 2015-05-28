using System.Collections.Generic;
using DataModels;

namespace BaseLibrary
{
    public interface IMemeberShipService
    {
        /// <summary>
        /// Generates the server registration.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        ServerRegisterResponse GenerateServerRegistration(string userName, string password);

        /// <summary>
        /// Completes the registration.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="deviceResponse">The device response.</param>
        bool CompleteRegistration(string userName, string deviceResponse);

        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="deviceResponse">The device response.</param>
        /// <returns></returns>
        bool AuthenticateUser(string userName, string deviceResponse);

        /// <summary>
        /// Determines whether [is user registered] [the specified user name] is completed registered.
        /// NOTE: method checks if use also registered a device to return true
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        bool IsUserRegistered(string userName);

        /// <summary>
        /// Generates the server challenge for each device a user has registered.
        /// NOTE: challenges will not be issued for compromised devices. 
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        List<ServerChallenge> GenerateServerChallenge(string userName);

        /// <summary>
        /// Determines whether [is valid user name and password] [the specified user name].
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        bool IsValidUserNameAndPassword(string userName, string password);
    }
}