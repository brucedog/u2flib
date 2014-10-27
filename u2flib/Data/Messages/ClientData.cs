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
using System.Text;
using Newtonsoft.Json.Linq;
using u2flib.Exceptions;

namespace u2flib.Data.Messages
{
    public class ClientData
    {
        private const String TypeParam = "typ";
        private const String ChallengeParam = "challenge";
        private const String OriginParam = "origin";
        private readonly String _challenge;
        private readonly String _origin;
        private readonly String _rawClientData;

        public ClientData(String clientData)
        {
            _rawClientData = Encoding.UTF8.GetString(Convert.FromBase64String(clientData));

            JObject clientDataAsElement = JObject.Parse(_rawClientData);
            if (clientDataAsElement == null)
            {
                throw new U2fException("ClientData has wrong format");
            }

            JToken theChallenge;
            JToken theOrgin;
            clientDataAsElement.TryGetValue(ChallengeParam, out theChallenge);
            clientDataAsElement.TryGetValue(OriginParam, out theOrgin);
            _challenge = theChallenge.ToString();
            _origin = theOrgin.ToString();
        }

        public void CheckContent(String type, String challenge, HashSet<String> facets)
        {
            if (!type.Equals(type))
            {
                throw new U2fException("Bad clientData: bad type " + type);
            }
            if (!challenge.Equals(_challenge))
            {
                throw new U2fException("Wrong challenge signed in clientData");
            }
            if (facets != null)
            {
                VerifyOrigin(_origin, CanonicalizeOrigins(facets));
            }
        }

        public String AsJson()
        {
            return _rawClientData;
        }

        public String GetChallenge()
        {
            return _challenge;
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