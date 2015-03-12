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
    public class StartedRegistration : DataObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartedRegistration"/> class.
        /// </summary>
        /// <param name="challenge">The challenge.</param>
        /// <param name="appId">The application identifier.</param>
        public StartedRegistration(String challenge, String appId)
        {
            Version = U2F.U2FVersion;
            Challenge = challenge;
            AppId = appId;
        }

        /// <summary>
        /// Gets or sets the version.
        /// Version of the protocol that the to-be-registered U2F token must speak. For
        /// the version of the protocol described herein, must be "U2F_V2"
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public String Version { get; private set; }

        /// <summary>
        /// Gets the challenge.
        /// The websafe-base64-encoded challenge.
        /// </summary>
        /// <value>
        /// The challenge.
        /// </value>
        public String Challenge { get; private set; }

        /// <summary>
        /// Gets the application identifier.
        /// The application id that the RP would like to assert. The U2F token will
        /// enforce that the key handle provided above is associated with this
        /// application id. The browser enforces that the calling origin belongs to the
        /// application identified by the application id.
        /// </summary>
        /// <value>
        /// The application identifier.
        /// </value>
        public String AppId { get; private set; }

        public override int GetHashCode()
        {
            int hash = Version.Sum(c => c + 31);
            hash += Challenge.Sum(c => c + 31);
            hash += AppId.Sum(c => c + 31);

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
            StartedRegistration other = (StartedRegistration) obj;
            if (AppId == null)
            {
                if (other.AppId != null)
                    return false;
            }
            else if (!AppId.Equals(other.AppId))
                return false;
            if (Challenge == null)
            {
                if (other.Challenge != null)
                    return false;
            }
            else if (!Challenge.Equals(other.Challenge))
                return false;
            if (Version == null)
            {
                if (other.Version != null)
                    return false;
            }
            else if (!Version.Equals(other.Version))
                return false;
            return true;
        }
    }
}