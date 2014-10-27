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
    public class StartedAuthentication : DataObject
    {
        /**
       * Version of the protocol that the to-be-registered U2F token must speak. For
       * the version of the protocol described herein, must be "U2F_V2"
       */
        private readonly String _version;

        /** The websafe-base64-encoded challenge. */
        private readonly String _challenge;

        /**
       * The application id that the RP would like to assert. The U2F token will
       * enforce that the key handle provided above is associated with this
       * application id. The browser enforces that the calling origin belongs to the
       * application identified by the application id.
       */
        private readonly String _appId;

        /**
       * websafe-base64 encoding of the key handle obtained from the U2F token
       * during registration.
       */
        private readonly String _keyHandle;

        public StartedAuthentication(String challenge, String appId, String keyHandle)
        {
            _version = U2F.U2FVersion;
            _challenge = challenge;
            _appId = appId;
            _keyHandle = keyHandle;
        }

        public override int GetHashCode()
        {
            int hash = 23 + _version.Sum(c => c + 31);
            hash += _challenge.Sum(c => c + 31);
            hash += _appId.Sum(c => c + 31);
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
            StartedAuthentication other = (StartedAuthentication) obj;
            if (_appId == null)
            {
                if (other._appId != null)
                    return false;
            }
            else if (!_appId.Equals(other._appId))
                return false;
            if (_challenge == null)
            {
                if (other._challenge != null)
                    return false;
            }
            else if (!_challenge.Equals(other._challenge))
                return false;
            if (_keyHandle == null)
            {
                if (other._keyHandle != null)
                    return false;
            }
            else if (!_keyHandle.Equals(other._keyHandle))
                return false;
            if (_version == null)
            {
                if (other._version != null)
                    return false;
            }
            else if (!_version.Equals(other._version))
                return false;
            return true;
        }

        public String GetKeyHandle()
        {
            return _keyHandle;
        }

        public String GetChallenge()
        {
            return _challenge;
        }

        public String GetAppId()
        {
            return _appId;
        }

        public static StartedAuthentication FromJson(String json)
        {
            return JsonConvert.DeserializeObject<StartedAuthentication>(json);
        }
    }
}