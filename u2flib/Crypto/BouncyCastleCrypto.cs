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
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using u2flib.Exceptions;

namespace u2flib.Crypto
{
    public class BouncyCastleCrypto : ICrypto
    {
        private readonly SHA256Managed _sha256Managed = new SHA256Managed();
        private readonly ISigner _signer = SignerUtilities.GetSigner("SHA-256withECDSA");
        public const string SignatureError = "Error when verifying signature";
        public const string ErrorDecodingPublicKey = "Error when decoding public key";

        public bool CheckSignature(X509Certificate attestationCertificate, byte[] signedBytes, byte[] signature)
        {
            return CheckSignature(attestationCertificate.GetPublicKey(), signedBytes, signature);
        }

        public bool CheckSignature(ICipherParameters getPublicKey, byte[] signedBytes, byte[] signature)
        {
            try
            {
                _signer.Init(false, getPublicKey);
                _signer.BlockUpdate(signedBytes, 0, signedBytes.Length);

                return _signer.VerifySignature(signature);
            }
            catch (InvalidKeyException e)
            {
                throw new U2fException(SignatureError, e);
            }
            catch (SignatureException e)
            {
                throw new U2fException(SignatureError, e);
            }
        }

        public ECPublicKeyParameters DecodePublicKey(byte[] encodedPublicKey)
        {
            try
            {
                X9ECParameters curve = SecNamedCurves.GetByName("secp256r1");
                ECPoint point;
                try
                {
                    point = curve.Curve.DecodePoint(encodedPublicKey);
                }
                catch (Exception e)
                {
                    throw new U2fException("Could not parse user public key", e);
                }

                ECPublicKeyParameters xxpk = new ECPublicKeyParameters("ECDSA", point,
                                                                       SecObjectIdentifiers.SecP256r1);

                return xxpk;
            }
            catch (InvalidKeySpecException e)
            {
                throw new U2fException(ErrorDecodingPublicKey, e);
            }
        }

        public byte[] Hash(byte[] bytes)
        {
            try
            {
                return _sha256Managed.ComputeHash(bytes);
            }
            catch (Exception e)
            {
                throw new UnsupportedOperationException("Error when computing SHA-256", e);
            }
        }

        public byte[] Hash(string str)
        {
            byte[] bytes = new byte[str.Length*sizeof (char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);

            return Hash(bytes);
        }
    }
}