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
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateResponse"/> class.
        /// </summary>
        /// <param name="clientData">The client data.</param>
        /// <param name="signatureData">The signature data.</param>
        /// <param name="keyHandle">The key handle.</param>
        public AuthenticateResponse(String clientData, String signatureData, String keyHandle)
        {
            ClientData = clientData;
            SignatureData = signatureData;
            KeyHandle = keyHandle;
        }

        public ClientData GetClientData()
        {
            return new ClientData(ClientData);
        }

        /// <summary>
        /// Gets the signature data.
        /// websafe-base64(raw response from U2F device) 
        /// </summary>
        /// <value>
        /// The signature data.
        /// </value>
        public String SignatureData { get; private set; }

        /// <summary>
        /// Gets the Client data.
        /// </summary>
        /// <value>
        /// The Client data.
        /// </value>
        public String ClientData { get; private set; }

        /// <summary>
        ///  keyHandle originally passed
        /// </summary>
        /// <value>
        /// The key handle.
        /// </value>
        public String KeyHandle { get; private set; }


        public static AuthenticateResponse FromJson(String json)
        {
            return JsonConvert.DeserializeObject<AuthenticateResponse>(json);
        }

        public override int GetHashCode()
        {
            int hash = ClientData.Sum(c => c + 31);
            hash += SignatureData.Sum(c => c + 31);
            hash += KeyHandle.Sum(c => c + 31);

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

            AuthenticateResponse other = (AuthenticateResponse) obj;

            if (ClientData == null)
            {
                if (other.ClientData != null)
                    return false;
            }
            else if (!ClientData.Equals(other.ClientData))
                return false;

            if (KeyHandle == null)
            {
                if (other.KeyHandle != null)
                    return false;
            }
            else if (!KeyHandle.Equals(other.KeyHandle))
                return false;

            if (SignatureData == null)
            {
                if (other.SignatureData != null)
                    return false;
            }
            else if (!SignatureData.Equals(other.SignatureData))
                return false;

            return true;
        }
    }
}