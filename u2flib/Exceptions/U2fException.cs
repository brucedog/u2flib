/*
 * Copyright 2014 Yubico.
 * Copyright 2014 Google Inc. All rights reserved.
 *
 * Use of this source code is governed by a BSD-style
 * license that can be found in the LICENSE file or at
 * https://developers.google.com/open-source/licenses/bsd
 */

using System;
using Org.BouncyCastle.Security;

namespace u2flib.Exceptions
{
    public class U2fException : Exception
    {
        public U2fException(string message)
        {
            Console.WriteLine("U2f exception:{0}", message);
        }

        public U2fException(string errorWhenVerifyingSignature, InvalidKeyException invalidKeyException)
        {
            Console.WriteLine("Error verifying signature:{0} invalid key exception:{1}", errorWhenVerifyingSignature, invalidKeyException);
        }

        public U2fException(string couldNotParseUserPublicKey, Exception invalidKeyException)
        {
            Console.WriteLine("Could not parse:{0} invalid key exception:{1}", couldNotParseUserPublicKey,
                              invalidKeyException);
        }
    }
}