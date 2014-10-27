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
using Org.BouncyCastle.Utilities;
using u2flib.Exceptions;
using u2flib.Util;

namespace u2flib.Data.Messages
{
    public class RawAuthenticateResponse
    {
        public static byte UserPresentFlag = 0x01;
        private readonly byte _userPresence;
        private readonly int _counter;
        private readonly byte[] _signature;

        public RawAuthenticateResponse(byte userPresence, int counter, byte[] signature)
        {
            _userPresence = userPresence;
            _counter = counter;
            _signature = signature;
        }

        public static RawAuthenticateResponse FromBase64(String rawDataBase64)
        {
            byte[] bytes = Convert.FromBase64String(rawDataBase64);
            Stream stream = new MemoryStream(bytes);
            var binaryReader = new BinaryReader(stream);
            try
            {
                return new RawAuthenticateResponse(
                    binaryReader.ReadByte(),
                    binaryReader.ReadInt32(), // TODO figure out
                    Utils.ReadAllBytes(binaryReader)
                    );
            }
            finally
            {
                stream.Dispose();
                binaryReader.Dispose();
            }
        }

        public void CheckSignature(String appId, String clientData, byte[] publicKey)
        {
            byte[] signedBytes = PackBytesToSign(
                U2F.Crypto.Hash(appId),
                _userPresence,
                _counter,
                U2F.Crypto.Hash(clientData)
                );
            U2F.Crypto.CheckSignature(
                U2F.Crypto.DecodePublicKey(publicKey),
                signedBytes,
                _signature
                );
        }

        public static byte[] PackBytesToSign(byte[] appIdHash, byte userPresence, int counter, byte[] challengeHash)
        {
            List<byte> someBytes = new List<byte>();
            someBytes.AddRange(appIdHash);
            someBytes.Add(userPresence);
            someBytes.Add(Convert.ToByte(counter));
            someBytes.AddRange(challengeHash);

            return someBytes.ToArray();
        }

        /**
         * Bit 0 is set to 1, which means that user presence was verified. (This
         * version of the protocol doesn't specify a way to request authentication
         * responses without requiring user presence.) A different value of bit 0, as
         * well as bits 1 through 7, are reserved for future use. The values of bit 1
         * through 7 SHOULD be 0
         */

        public byte GetUserPresence()
        {
            return _userPresence;
        }

        /**
         * This is the big-endian representation of a counter value that the U2F device
         * increments every time it performs an authentication operation.
         */

        public int GetCounter()
        {
            return _counter;
        }

        /** This is a ECDSA signature (on P-256) */

        public byte[] GetSignature()
        {
            return _signature;
        }

        public override int GetHashCode()
        {
            return 23 + _signature.Sum(b => b + 31 + _counter + _userPresence);
        }

        public override bool Equals(Object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (this.GetType() != obj.GetType())
                return false;
            RawAuthenticateResponse other = (RawAuthenticateResponse) obj;
            if (_counter != other._counter)
                return false;
            if (!Arrays.AreEqual(_signature, other._signature))
                return false;
            return _userPresence == other._userPresence;
        }

        public void CheckUserPresence()
        {
            if (_userPresence != UserPresentFlag)
            {
                throw new U2fException("User presence invalid during authentication");
            }
        }
    }
}