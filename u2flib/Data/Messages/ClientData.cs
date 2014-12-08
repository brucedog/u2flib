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
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using u2flib.Exceptions;
using u2flib.Util;

namespace u2flib.Data.Messages
{
    public class ClientData
    {
        private const String TypeParam = "typ";
        private const String ChallengeParam = "challenge";
        private const String OriginParam = "origin";

        public string Type { get; private set; }
        public string Challenge { get; private set; }
        public string Origin { get; private set; }
        public string RawClientData { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientData"/> class.
        /// </summary>
        /// <param name="clientData">The client data.</param>
        /// <exception cref="U2fException">ClientData has wrong format</exception>
        public ClientData(String clientData)
        {
            RawClientData = Utils.GetString(Utils.Base64StringToByteArray(clientData));

			JObject element = JObject.Parse(RawClientData);
            if (element == null)
                throw new U2fException("ClientData has wrong format");

            JToken theType, theChallenge, theOrgin;
            if (!element.TryGetValue(TypeParam, out theType))
                throw new U2fException("Bad clientData: missing 'typ' param");
            if (!element.TryGetValue(ChallengeParam, out theChallenge))
                throw new U2fException("Bad clientData: missing 'challenge' param");

            Type = theType.ToString();
            Challenge = theChallenge.ToString();
            if (element.TryGetValue(OriginParam, out theOrgin))
                Origin = theOrgin.ToString();
        }

        public void CheckContent(String type, String challenge, HashSet<String> facets)
        {
            if (!type.Equals(this.Type))
            {
                throw new U2fException("Bad clientData: bad type " + type);
            }
            if (!challenge.Equals(this.Challenge))
            {
                throw new U2fException("Wrong challenge signed in clientData");
            }
            if (facets != null)
            {
                VerifyOrigin(Origin, CanonicalizeOrigins(facets));
            }
        }

        public String AsJson()
        {
            return RawClientData;
        }

        private static void VerifyOrigin(String origin, HashSet<String> allowedOrigins)
        {
            if (!allowedOrigins.Contains(CanonicalizeOrigin(origin)))
            {
                throw new UriFormatException(origin +
                    " is not a recognized home origin for this backend");
            }
        }

        public static HashSet<String> CanonicalizeOrigins(HashSet<String> origins)
        {
            HashSet<String> result = new HashSet<string>();
            foreach (string orgin in origins)
            {
                result.Add(CanonicalizeOrigin(orgin));
            }
            return result;
        }

        public static String CanonicalizeOrigin(String url)
        {
            try
            {
                Uri uri = new Uri(url);
                return uri.Scheme + "://" + uri.Authority;
            }
            catch (UriFormatException e)
            {
                throw new UriFormatException("specified bad origin", e);
            }
        }
    }
}