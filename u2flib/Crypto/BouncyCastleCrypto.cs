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
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using u2flib.Exceptions;

namespace u2flib.Crypto
{
    public class BouncyCastleCrypto : ICrypto
    {
        private readonly SHA256Managed _sha256Managed = new SHA256Managed();
        private readonly ISigner _signer = SignerUtilities.GetSigner("SHA-256withECDSA");
        private readonly DerObjectIdentifier _curve = SecObjectIdentifiers.SecP256r1;
        private const string SignatureError = "Error when verifying signature";
        private const string ErrorDecodingPublicKey = "Error when decoding public key";

        public bool CheckSignature(X509Certificate2 attestationCertificate, byte[] signedBytes, byte[] signature)
        {
            var x509 = DotNetUtilities.FromX509Certificate(attestationCertificate);

            return CheckSignature(x509.GetPublicKey(), signedBytes, signature);
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

        public ICipherParameters DecodePublicKey(byte[] encodedPublicKey)
        {
            try
            {
                try
                {
                    X9ECParameters curve = SecNamedCurves.GetByOid(_curve);
                    ECPoint point = curve.Curve.DecodePoint(encodedPublicKey);
                    ECDomainParameters ecP = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

                    return new ECPublicKeyParameters(point, ecP);
                }
                catch (Exception e)
                {
                    throw new U2fException("Could not parse user public key", e);
                }
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