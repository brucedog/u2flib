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
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using u2flib.Exceptions;
using u2flib.Util;

namespace u2flib.Data.Messages
{
    public class RawRegisterResponse
    {
        public const byte RegistrationReservedByteValue = 0x05;
        public const byte RegistrationSignedReservedByteValue = 0x00;

        /**
         * The (uncompressed) x,y-representation of a curve point on the P-256
         * NIST elliptic curve.
         */
        private readonly byte[] _userPublicKey;

        /**
         * A handle that allows the U2F token to identify the generated key pair.
         */
        private readonly byte[] _keyHandle;
        
        private readonly byte[] _attestationCertificate;

        /** A ECDSA signature (on P-256) */
        private readonly byte[] _signature;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawRegisterResponse"/> class.
        /// </summary>
        /// <param name="userPublicKey">The user public key.</param>
        /// <param name="keyHandle">The key handle.</param>
        /// <param name="attestationCertificate">The attestation certificate.</param>
        /// <param name="signature">The signature.</param>
        public RawRegisterResponse(byte[] userPublicKey, byte[] keyHandle,
                                   byte[] attestationCertificate, byte[] signature)
        {
            _userPublicKey = userPublicKey;
            _keyHandle = keyHandle;
            _attestationCertificate = attestationCertificate;
            _signature = signature;
        }

        public static RawRegisterResponse FromBase64(String rawDataBase64)
        {
            byte[] bytes = Utils.Base64StringToByteArray(rawDataBase64); 

            Stream stream = new MemoryStream(bytes);
            BinaryReader binaryReader = new BinaryReader(stream);

            byte reservedByte = binaryReader.ReadByte();
            if (reservedByte != RegistrationReservedByteValue)
            {
                throw new U2fException(String.Format(
                    "Incorrect value of reserved byte. Expected: {0}. Was: {1}",
                    RegistrationReservedByteValue, reservedByte));
            }

            try
            {
                byte[] publicKey = binaryReader.ReadBytes(65);
                byte[] keyHandle = binaryReader.ReadBytes(binaryReader.ReadByte());
                long certificatePosition = binaryReader.BaseStream.Position;
                int size = (int)(binaryReader.BaseStream.Length - binaryReader.BaseStream.Position);
                byte[] certBytes = binaryReader.ReadBytes(size);
                X509Certificate2 attestationCertificate = new X509Certificate2(certBytes);

                binaryReader.BaseStream.Position = certificatePosition + attestationCertificate.Export(X509ContentType.Cert).Length;

                size = (int)(binaryReader.BaseStream.Length - binaryReader.BaseStream.Position);

                byte[] signature = binaryReader.ReadBytes(size);

                RawRegisterResponse rawRegisterResponse = new RawRegisterResponse(
                    publicKey,
                    keyHandle,
                    attestationCertificate.RawData,
                    signature);

                return rawRegisterResponse;
            }
            catch (CertificateException e)
            {
                throw new U2fException("Error when parsing attestation certificate", e);
            }
            finally
            {
                stream.Dispose();
                binaryReader.Dispose();
            }
        }

        public void CheckSignature(String appId, String clientData)
        {
            byte[] signedBytes = PackBytesToSign(
                U2F.Crypto.Hash(Encoding.ASCII.GetBytes(appId)), 
                U2F.Crypto.Hash(Encoding.ASCII.GetBytes(clientData)), 
                _keyHandle,
                _userPublicKey);

            U2F.Crypto.CheckSignature(new X509Certificate2(_attestationCertificate), signedBytes, _signature);
        }

        public static byte[] PackBytesToSign(byte[] appIdHash, byte[] clientDataHash, byte[] keyHandle,
                                             byte[] userPublicKey)
        {
            List<byte> someBytes = new List<byte>();
            someBytes.Add(RegistrationSignedReservedByteValue);
            someBytes.AddRange(appIdHash);
            someBytes.AddRange(clientDataHash);
            someBytes.AddRange(keyHandle);
            someBytes.AddRange(userPublicKey);

            return someBytes.ToArray();
        }

        public DeviceRegistration CreateDevice()
        {
            return new DeviceRegistration(
                _keyHandle,
                _userPublicKey,
                _attestationCertificate,
                DeviceRegistration.INITIAL_COUNTER_VALUE
                );
        }

        public override int GetHashCode()
        {
            int hash = 23 + _userPublicKey.Sum(b => b + 31);
            hash += _keyHandle.Sum(b => b + 31);
            hash += _signature.Sum(b => b + 31);

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
            RawRegisterResponse other = (RawRegisterResponse) obj;
            if (!Arrays.AreEqual(_attestationCertificate, other._attestationCertificate))
                return false;
            if (!Arrays.AreEqual(_keyHandle, other._keyHandle))
                return false;
            if (!Arrays.AreEqual(_signature, other._signature))
                return false;
            return Arrays.AreEqual(_userPublicKey, other._userPublicKey);
        }
    }
}