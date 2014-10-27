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
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using u2flib.Exceptions;

namespace u2flib.Data
{
    public class DeviceRegistration : DataObject, ISerializable
    {
        private long serialVersionUID = -142942195464329902L;
        public static int INITIAL_COUNTER_VALUE = 0;

        private readonly byte[] _keyHandle;
        private readonly byte[] _publicKey;
        private readonly byte[] _attestationCert;
        private int _counter;

        public DeviceRegistration(byte[] keyHandle, byte[] publicKey, X509Certificate attestationCert, int counter)
        {
            _keyHandle = keyHandle;
            _publicKey = publicKey;
            _counter = counter;

            try
            {
                _attestationCert = attestationCert.GetEncoded();
            }
            catch (CertificateEncodingException e)
            {
                throw new U2fException("Invalid attestation certificate", e);
            }
        }

        public byte[] GetKeyHandle()
        {
            return _keyHandle;
        }

        public byte[] GetPublicKey()
        {
            return _publicKey;
        }

        public X509Certificate GetAttestationCertificate()
        {
            X509CertificateParser test = new X509CertificateParser();

            return test.ReadCertificate(_attestationCert);
        }

        public int GetCounter()
        {
            return _counter;
        }

        public override int GetHashCode()
        {
            int hash = _publicKey.Sum(b => b + 31);
            hash += _attestationCert.Sum(b => b + 31);
            hash += _keyHandle.Sum(b => b + 31);

            return hash;
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is DeviceRegistration))
            {
                return false;
            }
            DeviceRegistration that = (DeviceRegistration) obj;
            return Arrays.AreEqual(_keyHandle, that._keyHandle)
                   && Arrays.AreEqual(_publicKey, that._publicKey)
                   && Arrays.AreEqual(_attestationCert, that._attestationCert);
        }

        public override String ToString()
        {
            return base.ToJson();
        }

        public static DeviceRegistration FromJson(String json)
        {
            return JsonConvert.DeserializeObject<DeviceRegistration>(json);
        }

        public String toJson()
        {
            return JsonConvert.SerializeObject(new DeviceWithoutCertificate(_keyHandle, _publicKey, _counter));
        }

        public String ToJsonWithAttestationCert()
        {
            return ToJson();
        }

        public void CheckAndIncrementCounter(int clientCounter)
        {
            if (clientCounter <= _counter++)
            {
                throw new U2fException("Counter value smaller than expected!");
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class DeviceWithoutCertificate
    {
        private byte[] _keyHandle;
        private byte[] _publicKey;
        private int _counter;

        internal DeviceWithoutCertificate(byte[] keyHandle, byte[] publicKey, int counter)
        {
            _keyHandle = keyHandle;
            _publicKey = publicKey;
            _counter = counter;
        }
    }
}