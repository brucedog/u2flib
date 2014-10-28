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
using u2flib.Crypto;
using u2flib.Data;
using u2flib.Data.Messages;

namespace u2flib
{
    public class U2F
    {
        public const String U2FVersion = "U2F_V2";
        private static readonly IChallengeGenerator _challengeGenerator = new RandomChallengeGenerator();
        public static readonly ICrypto Crypto = new BouncyCastleCrypto();
        public const String AuthenticateTyp = "navigator.id.getAssertion";
        public const String RegisterType = "navigator.id.finishEnrollment";


        /**
         * Initiates the registration of a device.
         *
         * @param appId the U2F AppID. Set this to the Web Origin of the login page, unless you need to
         * support logging in from multiple Web Origins.
         * @return a StartedRegistration, which should be sent to the client and temporary saved by the
         * server.
         */
        public static StartedRegistration StartRegistration(String appId)
        {
            byte[] challenge = _challengeGenerator.GenerateChallenge();
            String challengeBase64 = Convert.ToBase64String(challenge);

            return new StartedRegistration(challengeBase64, appId);
        }

        /**
       * Finishes a previously started registration.
       *
       * @param startedRegistration
       * @param tokenResponse the response from the token/client.
       * @return a DeviceRegistration object, holding information about the registered device. Servers should
       * persist this.
       */
        public static DeviceRegistration FinishRegistration(StartedRegistration startedRegistration,
                                                            RegisterResponse tokenResponse)
        {
            return FinishRegistration(startedRegistration, tokenResponse, new HashSet<string>());
        }

        public static DeviceRegistration FinishRegistration(StartedRegistration startedRegistration,
                                                            RegisterResponse tokenResponse, HashSet<String> facets)
        {
            ClientData clientData = tokenResponse.GetClientData();
            clientData.CheckContent(RegisterType, startedRegistration.Challenge, facets);

            RawRegisterResponse rawRegisterResponse = RawRegisterResponse.FromBase64(tokenResponse.RegistrationData);
            rawRegisterResponse.CheckSignature(startedRegistration.AppId, clientData.AsJson());
            return rawRegisterResponse.CreateDevice();
        }

        /**
         * Initiates the authentication process.
         *
         * @param appId the U2F AppID. Set this to the Web Origin of the login page, unless you need to
         * support logging in from multiple Web Origins.
         * @param deviceRegistration the DeviceRegistration for which to initiate authentication.
         * @return a StartedAuthentication which should be sent to the client and temporary saved by
         * the server.
         */
        public static StartedAuthentication StartAuthentication(String appId, DeviceRegistration deviceRegistration)
        {
            byte[] challenge = _challengeGenerator.GenerateChallenge();
            return new StartedAuthentication(
                Convert.ToBase64String(challenge),
                appId,
                Convert.ToBase64String(deviceRegistration.KeyHandle)
                );
        }

        /**
        * Finishes a previously started authentication.
        *
        * @param startedAuthentication
        * @param response the response from the token/client.
        * @return the new value of the DeviceRegistration's counter.
        */
        public static DeviceRegistration FinishAuthentication(StartedRegistration startedRegistration,
                                                              RegisterResponse tokenResponse)
        {
            return FinishAuthentication(startedRegistration, tokenResponse, null);
        }

        public static DeviceRegistration FinishAuthentication(StartedRegistration startedRegistration,
                                                              RegisterResponse response,
                                                              HashSet<String> facets)
        {
            ClientData clientData = response.GetClientData();
            clientData.CheckContent(RegisterType, startedRegistration.Challenge, facets);
            RawRegisterResponse rawAuthenticateResponse = RawRegisterResponse.FromBase64(response.RegistrationData);
            rawAuthenticateResponse.CheckSignature(startedRegistration.AppId, clientData.AsJson());

            return rawAuthenticateResponse.CreateDevice();
        }
    }
}