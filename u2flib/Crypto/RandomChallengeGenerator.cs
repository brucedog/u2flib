/*
 * Copyright 2014 Yubico.
 * Copyright 2014 Google Inc. All rights reserved.
 *
 * Use of this source code is governed by a BSD-style
 * license that can be found in the LICENSE file or at
 * https://developers.google.com/open-source/licenses/bsd
 */

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;

namespace u2flib.Crypto
{
    public class RandomChallengeGenerator : IChallengeGenerator
    {
        // version 1.7 of bouncy castle library uses sha1 which the security industry seems to moving away from 
        // so manual setting generator to sha256 which is the default in the upcoming 1.8 bouncy castle library.
        private static readonly IRandomGenerator Generator = new DigestRandomGenerator(new Sha256Digest());
        private static readonly SecureRandom Random = new SecureRandom(Generator);

        public byte[] GenerateChallenge()
        {
            byte[] randomBytes = new byte[32];
            Random.NextBytes(randomBytes);

            return randomBytes;
        }
    }
}