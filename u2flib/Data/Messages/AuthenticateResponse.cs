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
    public class AuthenticateResponse : DataObject
    {
        /** websafe-base64(client data) */
        private readonly String _clientData;

        /** websafe-base64(raw response from U2F device) */
        private readonly String _signatureData;

        /** keyHandle originally passed */
        private readonly String _keyHandle;


        public AuthenticateResponse(String clientData, String signatureData, String keyHandle)
        {
            _clientData = clientData;
            _signatureData = signatureData;
            _keyHandle = keyHandle;
        }

        public ClientData GetClientData()
        {
            return new ClientData(_clientData);
        }

        public String GetSignatureData()
        {
            return _signatureData;
        }

        public String GetKeyHandle()
        {
            return _keyHandle;
        }

        public override int GetHashCode()
        {
            int hash = _clientData.Sum(c => c + 31);
            hash += _signatureData.Sum(c => c + 31);
            hash += _keyHandle.Sum(c => c + 31);

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
            AuthenticateResponse other = (AuthenticateResponse) obj;
            if (_clientData == null)
            {
                if (other._clientData != null)
                    return false;
            }
            else if (!_clientData.Equals(other._clientData))
                return false;
            if (_keyHandle == null)
            {
                if (other._keyHandle != null)
                    return false;
            }
            else if (!_keyHandle.Equals(other._keyHandle))
                return false;
            if (_signatureData == null)
            {
                if (other._signatureData != null)
                    return false;
            }
            else if (!_signatureData.Equals(other._signatureData))
                return false;
            return true;
        }

        public static AuthenticateResponse FromJson(String json)
        {
            return JsonConvert.DeserializeObject<AuthenticateResponse>(json);
        }
    }
}