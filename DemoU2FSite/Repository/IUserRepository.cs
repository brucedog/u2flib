namespace DemoU2FSite.Repository
{
    public interface IUserRepository
    {
        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        User FindUser(string userName);

        /// <summary>
        /// Updates the device counter.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="devicePublicKey">The device public key.</param>
        /// <param name="counter">The counter.</param>
        void UpdateDeviceCounter(string userName, byte[] devicePublicKey, uint counter);

        /// <summary>
        /// Removes the users authentication request.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        void RemoveUsersAuthenticationRequest(string userName);

        /// <summary>
        /// Saves the user authentication request.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="appId">The application identifier.</param>
        /// <param name="challenge">The challenge.</param>
        /// <param name="keyHandle">The key handle.</param>
        void SaveUserAuthenticationRequest(string username, string appId, string challenge, string keyHandle);

        /// <summary>
        /// Adds the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="hashedPassword">The hashed password.</param>
        void AddUser(string userName, string hashedPassword);

        /// <summary>
        /// Adds the authentication request.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="appId">The application identifier.</param>
        /// <param name="challenge">The challenge.</param>
        /// <param name="version">The version.</param>
        void AddAuthenticationRequest(string userName, string appId, string challenge, string version);

        /// <summary>
        /// Adds the device registration.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="attestationCert">The attestation cert.</param>
        /// <param name="counter">The counter.</param>
        /// <param name="keyHandle">The key handle.</param>
        /// <param name="publicKey">The public key.</param>
        void AddDeviceRegistration(string userName, byte[] attestationCert, uint counter, byte[] keyHandle, byte[] publicKey);
    }
}