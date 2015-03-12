/*
 * Code was copied from java example so there is a .NET version 
 * to work with.
 * 
 * Copyright 2014 Yubico.
 * Copyright 2014 Google Inc. All rights reserved.
 *
 * Use of this source code is governed by a BSD-style
 * license that can be found in the LICENSE file or at
 * https://developers.google.com/open-source/licenses/bsd
 */

using System;
using System.Linq;

namespace u2flib.Data.Messages
{
    public class RegisterResponse : DataObject
    {
        private readonly ClientData _clientDataRef;
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterResponse"/> class.
        /// </summary>
        /// <param name="registrationData">The registration data.</param>
        /// <param name="clientData">The client data.</param>
        public RegisterResponse(String registrationData, String clientData)
        {
            RegistrationData = registrationData;
            ClientData = clientData;
            _clientDataRef = new ClientData(ClientData);
        }

        /// <summary>
        /// Gets the registration data.
        /// </summary>
        /// <value>
        /// The registration data.
        /// </value>
        public String RegistrationData { get; private set; }

        /// <summary>
        /// Gets the Client data.
        /// </summary>
        /// <value>
        /// The Client data.
        /// </value>
        public String ClientData { get; private set; }

        public ClientData GetClientData()
        {
            return _clientDataRef;
        }

        public String GetRequestId()
        {
            return GetClientData().Challenge;
        }

        public override int GetHashCode()
        {
            int hash = RegistrationData.Sum(c => c + 31);
            hash += ClientData.Sum(c => c + 31);

            return hash;
        }

        public override bool Equals(Object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            RegisterResponse other = (RegisterResponse) obj;
            if (ClientData == null)
            {
                if (other.ClientData != null)
                    return false;
            }
            else if (!ClientData.Equals(other.ClientData))
                return false;
            if (RegistrationData == null)
            {
                if (other.RegistrationData != null)
                    return false;
            }
            else if (!RegistrationData.Equals(other.RegistrationData))
                return false;
            return true;
        }
    }
}