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
    public class StartedRegistration : DataObject
    {
        /**
       * Version of the protocol that the to-be-registered U2F token must speak. For
       * the version of the protocol described herein, must be "U2F_V2"
       */
        private readonly string _version;

        /** The websafe-base64-encoded challenge. */
        private readonly string _challenge;

        public String GetChallenge()
        {
            return _challenge;
        }

        /**
       * The application id that the RP would like to assert. The U2F token will
       * enforce that the key handle provided above is associated with this
       * application id. The browser enforces that the calling origin belongs to the
       * application identified by the application id.
       */
        private readonly string _appId;

        public String GetAppId()
        {
            return _appId;
        }

        public StartedRegistration(String challenge, String appId)
        {
            _version = U2F.U2FVersion;
            _challenge = challenge;
            _appId = appId;
        }

        public override int GetHashCode()
        {
            int hash = _version.Sum(c => c + 31);
            hash += _challenge.Sum(c => c + 31);
            hash += _appId.Sum(c => c + 31);

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
            StartedRegistration other = (StartedRegistration) obj;
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
            if (_version == null)
            {
                if (other._version != null)
                    return false;
            }
            else if (!_version.Equals(other._version))
                return false;
            return true;
        }

        public static StartedRegistration FromJson(String json)
        {
            return JsonConvert.DeserializeObject<StartedRegistration>(json);
        }
    }
}