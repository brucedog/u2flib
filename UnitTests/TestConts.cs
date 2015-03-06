﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using u2flib.Crypto;
using u2flib.Util;

namespace UnitTests
{
    public static class TestConts
    {
        private static readonly ICrypto crypto = new BouncyCastleCrypto();

        //Test vectors from FIDO U2F: Raw Message Formats - Draft 4
        public static int COUNTER_VALUE = 1;
        public static HashSet<String> TRUSTED_DOMAINS = new HashSet<string>() {"http://example.com"};
        public static String APP_ID_ENROLL = "http://example.com";
        public static byte[] APP_ID_ENROLL_SHA256 = crypto.Hash(APP_ID_ENROLL);
        public static String APP_ID_SIGN = "https://gstatic.com/securitykey/a/example.com";
        public static byte[] APP_ID_SIGN_SHA256 = crypto.Hash(APP_ID_SIGN);
        public static String ORIGIN = "http://example.com";

        public static String SERVER_CHALLENGE_REGISTER_BASE64 =
            "vqrS6WXDe1JUs5_c3i4-LkKIHRr-3XVb3azuA5TifHo";

        public static String SERVER_CHALLENGE_SIGN_BASE64 = "opsXqUifDriAAmWclinfbS0e-USY0CgyJHe_Otd7z8o";

        public static String CHANNEL_ID_STRING =
            "{\"kty\":\"EC\","
            + "\"crv\":\"P-256\","
            + "\"x\":\"HzQwlfXX7Q4S5MtCCnZUNBw3RMzPO9tOyWjBqRl4tJ8\","
            + "\"y\":\"XVguGFLIZx1fXg3wNqfdbn75hi4-_7-BxhMljw42Ht4\"}";            

        public static String CLIENT_DATA_REGISTER = "{\"typ\":\"navigator.id.finishEnrollment\"," +"\"challenge\":\"" + SERVER_CHALLENGE_REGISTER_BASE64
            + "\"," + "\"cid_pubkey\":" + CHANNEL_ID_STRING + "," + "\"origin\":\"" + ORIGIN + "\"}";

        public static String CLIENT_DATA_REGISTER_BASE64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(CLIENT_DATA_REGISTER));
        
        public static byte[] CLIENT_DATA_ENROLL_SHA256 = crypto.Hash(Encoding.UTF8.GetBytes(CLIENT_DATA_REGISTER));

        public static String CLIENT_DATA_AUTHENTICATE = "{\"typ\":\"navigator.id.getAssertion\"," + "\"challenge\":\"" + SERVER_CHALLENGE_SIGN_BASE64
            + "\"," + "\"cid_pubkey\":" + CHANNEL_ID_STRING + "," + "\"origin\":\"" + ORIGIN + "\"}";

        public static String CLIENT_DATA_AUTHENTICATE_BASE64 =
            "eyJ0eXAiOiJuYXZpZ2F0b3IuaWQuZ2V0QXNzZXJ0aW9uIiwiY2hhbGxlbmdlIjoib3BzWHFVaWZEcmlBQW1XY2xpbmZiUzBlLVVTWTBDZ3lKSGVfT3RkN3o4byIsImNpZF9wdWJrZXkiOnsia3R5IjoiRUMiLCJjcnYiOiJQLTI1NiIsIngiOiJIelF3bGZYWDdRNFM1TXRDQ25aVU5CdzNSTXpQTzl0T3lXakJxUmw0dEo4IiwieSI6IlhWZ3VHRkxJWngxZlhnM3dOcWZkYm43NWhpNC1fNy1CeGhNbGp3NDJIdDQifSwib3JpZ2luIjoiaHR0cDovL2V4YW1wbGUuY29tIn0";

        public static byte[] CLIENT_DATA_AUTHENTICATE_SHA256 = StringToByteArray(
            "ccd6ee2e47baef244d49a222db496bad0ef5b6f93aa7cc4d30c4821b3b9dbc57");

        public static byte[] REGISTRATION_REQUEST_DATA = StringToByteArray(
            "4142d21c00d94ffb9d504ada8f99b721f4b191ae4e37ca0140f696b6983cfacbf0e6a6a97042a4f1f1c87f5f7d44315b2d852c2df5c7991cc66241bf7072d1c4");

        public static byte[] REGISTRATION_RESPONSE_DATA = StringToByteArray(
            "0504b174bc49c7ca254b70d2e5c207cee9cf174820ebd77ea3c65508c26da51b"
            + "657c1cc6b952f8621697936482da0a6d3d3826a59095daf6cd7c03e2e60385d2f6d9402a552dfdb7477ed65fd84133f86196010b2215b57da75d315b7b9e8fe2"
            + "e3925a6019551bab61d16591659cbaf00b4950f7abfe6660e2e006f76868b772d70c253082013c3081e4a003020102020a47901280001155957352300a06082a"
            + "8648ce3d0403023017311530130603550403130c476e756262792050696c6f74301e170d3132303831343138323933325a170d3133303831343138323933325a"
            + "3031312f302d0603550403132650696c6f74476e756262792d302e342e312d34373930313238303030313135353935373335323059301306072a8648ce3d0201"
            + "06082a8648ce3d030107034200048d617e65c9508e64bcc5673ac82a6799da3c1446682c258c463fffdf58dfd2fa3e6c378b53d795c4a4dffb4199edd7862f23"
            + "abaf0203b4b8911ba0569994e101300a06082a8648ce3d0403020347003044022060cdb6061e9c22262d1aac1d96d8c70829b2366531dda268832cb836bcd30d"
            + "fa0220631b1459f09e6330055722c8d89b7f48883b9089b88d60d1d9795902b30410df304502201471899bcc3987e62e8202c9b39c33c19033f7340352dba80f"
            + "cab017db9230e402210082677d673d891933ade6f617e5dbde2e247e70423fd5ad7804a6d3d3961ef871");

        public static String REGISTRATION_RESPONSE_DATA_BASE64 = Convert.ToBase64String(REGISTRATION_RESPONSE_DATA);

        public static byte[] KEY_HANDLE = StringToByteArray(
            "2a552dfdb7477ed65fd84133f86196010b2215b57da75d315b7b9e8fe2e3925a6019551bab61d16591659cbaf00b4950f7abfe6660e2e006f76868b772d70c25");

        public static string KEY_HANDLE_BASE64 = "KlUt_bdHftZf2EEz-GGWAQsiFbV9p10xW3uej-LjklpgGVUbq2HRZZFlnLrwC0lQ96v-ZmDi4Ab3aGi3ctcMJQ";

        public static byte[] KEY_HANDLE_BASE64_BYTE = Utils.Base64StringToByteArray("KlUt_bdHftZf2EEz-GGWAQsiFbV9p10xW3uej-LjklpgGVUbq2HRZZFlnLrwC0lQ96v-ZmDi4Ab3aGi3ctcMJQ");

        public static byte[] USER_PUBLIC_KEY_REGISTER_HEX = StringToByteArray(
            "04b174bc49c7ca254b70d2e5c207cee9cf174820ebd77ea3c65508c26da51b657c1cc6b952f8621697936482da0a6d3d3826a59095daf6cd7c03e2e60385d2f6d9");

        public static byte[] USER_PUBLIC_KEY_AUTHENTICATE_HEX = Utils.Base64StringToByteArray("BNNo8bZlut48M6IPHkKcd1DVAzZgwBkRnSmqS6erwEqnyApGu-EcqMtWdNdPMfipA_a60QX7ardK7-9NuLACXh0");

        public static byte[] AUTHENTICATE_RESPONSE_DATA = StringToByteArray(
            "0100000001304402204b5f0cd17534cedd8c34ee09570ef542a353df4436030ce43d406de870b847780220267bb998fac9b7266eb60e7cb0b5eabdfd5ba9614f53c7b22272ec10047a923f");

        public static String SIGN_RESPONSE_DATA_BASE64 = "AQAAAAEwRAIgS18M0XU0zt2MNO4JVw71QqNT30Q2AwzkPUBt6HC4R3gCICZ7uZj6ybcmbrYOfLC16r39W6lhT1PHsiJy7BAEepI_";

        public static byte[] EXPECTED_REGISTER_SIGNED_BYTES = StringToByteArray(
            "00f0e6a6a97042a4f1f1c87f5f7d44315b2d852c2df5c7991cc66241bf7072d1c44142d21c00d94ffb9d504ada8f99b721f4b191ae4e37ca0140f696b6983cfa"
            +"cb2a552dfdb7477ed65fd84133f86196010b2215b57da75d315b7b9e8fe2e3925a6019551bab61d16591659cbaf00b4950f7abfe6660e2e006f76868b772d70c"
            +"2504b174bc49c7ca254b70d2e5c207cee9cf174820ebd77ea3c65508c26da51b657c1cc6b952f8621697936482da0a6d3d3826a59095daf6cd7c03e2e60385d2f6d9");

        public static byte[] EXPECTED_AUTHENTICATE_SIGNED_BYTES = StringToByteArray(
            "4b0be934baebb5d12d26011b69227fa5e86df94e7d94aa2949a89f2d493992ca0100000001ccd6ee2e47baef244d49a222db496bad0ef5b6f93aa7cc4d30c4821b3b9dbc57");

        public static byte[] SIGNATURE_REGISTER = StringToByteArray(
            "304502201471899bcc3987e62e8202c9b39c33c19033f7340352dba80fcab017db9230e402210082677d673d891933ade6f617e5dbde2e247e70423fd5ad7804a6d3d3961ef871");

        public static byte[] SIGNATURE_AUTHENTICATE = StringToByteArray(
            "304402204b5f0cd17534cedd8c34ee09570ef542a353df4436030ce43d406de870b847780220267bb998fac9b7266eb60e7cb0b5eabdfd5ba9614f53c7b22272ec10047a923f");

        public static byte[] ATTESTATION_CERTIFICATE = Utils.Base64StringToByteArray("MIIBPDCB5KADAgECAgpHkBKAABFVlXNSMAoGCCqGSM49BAMCMBcxFTATBgNVBAMTDEdudWJieSBQaWxvdDAeFw0xMjA4MTQxODI5MzJaFw0xMzA4MTQxODI5MzJaMDExLzAtBgNVBAMTJlBpbG90R251YmJ5LTAuNC4xLTQ3OTAxMjgwMDAxMTU1OTU3MzUyMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEjWF-ZclQjmS8xWc6yCpnmdo8FEZoLCWMRj__31jf0vo-bDeLU9eVxKTf-0GZ7deGLyOrrwIDtLiRG6BWmZThATAKBggqhkjOPQQDAgNHADBEAiBgzbYGHpwiJi0arB2W2McIKbI2ZTHdomiDLLg2vNMN-gIgYxsUWfCeYzAFVyLI2Jt_SIg7kIm4jWDR2XlZArMEEN8");

  
        public static byte[] StringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}