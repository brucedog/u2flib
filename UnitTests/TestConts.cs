using System;
using System.Collections.Generic;
using System.Text;
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
        public static String ORIGIN = "http://example.com";

        public static String SERVER_CHALLENGE_REGISTER_BASE64 = "vqrS6WXDe1JUs5_c3i4-LkKIHRr-3XVb3azuA5TifHo";

        public static String SERVER_CHALLENGE_SIGN_BASE64 = "opsXqUifDriAAmWclinfbS0e-USY0CgyJHe_Otd7z8o";

        public static String CHANNEL_ID_STRING =
            "{\"kty\":\"EC\","
            + "\"crv\":\"P-256\","
            + "\"x\":\"HzQwlfXX7Q4S5MtCCnZUNBw3RMzPO9tOyWjBqRl4tJ8\","
            + "\"y\":\"XVguGFLIZx1fXg3wNqfdbn75hi4-_7-BxhMljw42Ht4\"}";            

        public static String CLIENT_DATA_REGISTER = "{\"typ\":\"navigator.id.finishEnrollment\"," +"\"challenge\":\"" + SERVER_CHALLENGE_REGISTER_BASE64
            + "\"," + "\"cid_pubkey\":" + CHANNEL_ID_STRING + "," + "\"origin\":\"" + ORIGIN + "\"}";

        public static String CLIENT_DATA_REGISTER_BASE64 = Utils.ByteArrayToBase64String(Utils.GetBytes(CLIENT_DATA_REGISTER));

        public static String CLIENT_DATA_AUTHENTICATE = "{\"typ\":\"navigator.id.getAssertion\"," + "\"challenge\":\"" + SERVER_CHALLENGE_SIGN_BASE64
            + "\"," + "\"cid_pubkey\":" + CHANNEL_ID_STRING + "," + "\"origin\":\"" + ORIGIN + "\"}";

        public static String CLIENT_DATA_AUTHENTICATE_BASE64 = Utils.ByteArrayToBase64String(Utils.GetBytes(CLIENT_DATA_AUTHENTICATE));


        public static String REGISTRATION_RESPONSE_DATA_BASE64 = "BQSxdLxJx8olS3DS5cIHzunPF0gg69d+o8ZVCMJtpRtlfBzGuVL4YhaXk2SC2gptPTgmpZCV2vbNfAPi5gOF0vbZQCpVLf23R37WX9hBM/hhlgELIhW1faddMVt7no/i45JaYBlVG6th0WWRZZy68AtJUPer/mZg4uAG92hot3LXDCUwggE8MIHkoAMCAQICCkeQEoAAEVWVc1IwCgYIKoZIzj0EAwIwFzEVMBMGA1UEAxMMR251YmJ5IFBpbG90MB4XDTEyMDgxNDE4MjkzMloXDTEzMDgxNDE4MjkzMlowMTEvMC0GA1UEAxMmUGlsb3RHbnViYnktMC40LjEtNDc5MDEyODAwMDExNTU5NTczNTIwWTATBgcqhkjOPQIBBggqhkjOPQMBBwNCAASNYX5lyVCOZLzFZzrIKmeZ2jwURmgsJYxGP//fWN/S+j5sN4tT15XEpN/7QZnt14YvI6uvAgO0uJEboFaZlOEBMAoGCCqGSM49BAMCA0cAMEQCIGDNtgYenCImLRqsHZbYxwgpsjZlMd2iaIMsuDa80w36AiBjGxRZ8J5jMAVXIsjYm39IiDuQibiNYNHZeVkCswQQ3zBFAiAUcYmbzDmH5i6CAsmznDPBkDP3NANS26gPyrAX25Iw5AIhAIJnfWc9iRkzreb2F+Xb3i4kfnBCP9WteASm09OWHvhx";

        public static string KEY_HANDLE_BASE64 = "KlUt_bdHftZf2EEz-GGWAQsiFbV9p10xW3uej-LjklpgGVUbq2HRZZFlnLrwC0lQ96v-ZmDi4Ab3aGi3ctcMJQ";

        public static byte[] KEY_HANDLE_BASE64_BYTE = Utils.Base64StringToByteArray("KlUt_bdHftZf2EEz-GGWAQsiFbV9p10xW3uej-LjklpgGVUbq2HRZZFlnLrwC0lQ96v-ZmDi4Ab3aGi3ctcMJQ");


        public static byte[] USER_PUBLIC_KEY_AUTHENTICATE_HEX = Utils.Base64StringToByteArray("BNNo8bZlut48M6IPHkKcd1DVAzZgwBkRnSmqS6erwEqnyApGu-EcqMtWdNdPMfipA_a60QX7ardK7-9NuLACXh0");


        public static String SIGN_RESPONSE_DATA_BASE64 = "AQAAAAEwRAIgS18M0XU0zt2MNO4JVw71QqNT30Q2AwzkPUBt6HC4R3gCICZ7uZj6ybcmbrYOfLC16r39W6lhT1PHsiJy7BAEepI_";
    }
}