/*
 * Copyright 2014 Yubico.
 * Copyright 2014 Google Inc. All rights reserved.
 *
 * Use of this source code is governed by a BSD-style
 * license that can be found in the LICENSE file or at
 * https://developers.google.com/open-source/licenses/bsd
 */

using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;

namespace u2flib.Crypto
{
    public interface ICrypto
    {
        /// <summary>
        /// Checks the signature.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="src">The source.</param>
        /// <param name="signature">The signature.</param>
        /// <returns></returns>
        bool CheckSignature(ICipherParameters key, byte[] src, byte[] signature);

        /// <summary>
        /// Checks the signature.
        /// </summary>
        /// <param name="attestationCertificate">The attestation certificate.</param>
        /// <param name="signedBytes">The signed bytes.</param>
        /// <param name="signature">The signature.</param>
        /// <returns></returns>
        bool CheckSignature(X509Certificate attestationCertificate, byte[] signedBytes, byte[] signature);

        /// <summary>
        /// Decodes the public key.
        /// </summary>
        /// <param name="encodedPublicKey">The encoded public key.</param>
        /// <returns></returns>
        ICipherParameters DecodePublicKey(byte[] encodedPublicKey);

        /// <summary>
        /// Hashes the specified bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        byte[] Hash(byte[] bytes);

        /// <summary>
        /// Hashes the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        byte[] Hash(String str);
    }
}