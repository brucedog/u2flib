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
using Newtonsoft.Json;

namespace u2flib.Data.Messages
{
    public class RegisterResponse : DataObject
    {
        /** websafe-base64(raw registration response message) */
        private readonly String _registrationData;

        /** websafe-base64(UTF8(stringified(client data))) */
        private readonly String _clientData;

        public RegisterResponse(String registrationData, String clientData)
        {
            _registrationData = registrationData;
            _clientData = clientData;
        }

        public String GetRegistrationData()
        {
            return _registrationData;
        }

        public ClientData GetClientData()
        {
            return new ClientData(_clientData);
        }

        public static RegisterResponse FromJson(String json)
        {
            return JsonConvert.DeserializeObject<RegisterResponse>(json);
        }

        public override int GetHashCode()
        {
            int hash = _registrationData.Sum(c => c + 31);
            hash += _clientData.Sum(c => c + 31);

            return hash;
        }

        public override bool Equals(Object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (this.GetType() != obj.GetType())
                return false;
            RegisterResponse other = (RegisterResponse) obj;
            if (_clientData == null)
            {
                if (other._clientData != null)
                    return false;
            }
            else if (!_clientData.Equals(other._clientData))
                return false;
            if (_registrationData == null)
            {
                if (other._registrationData != null)
                    return false;
            }
            else if (!_registrationData.Equals(other._registrationData))
                return false;
            return true;
        }
    }
}