﻿/*
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