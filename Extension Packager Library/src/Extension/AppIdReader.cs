/**
 * (c) 2013 Rob Wu <rob@robwu.nl>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Extension_Packager_Library.src.Extension
{
    internal interface IAppIdReader
    {
        internal string GetAppIdByPrivateKey(string privateKeyFile);
    }

    internal class AppIdReader : IAppIdReader
    {
        public string GetAppIdByPrivateKey(string privateKeyFile)
        {
            if (!File.Exists(privateKeyFile)) return null;

            string privateKeyBase64 = File.ReadAllText(privateKeyFile);
            privateKeyBase64 = privateKeyBase64.Replace("-----BEGIN PRIVATE KEY-----", "");
            privateKeyBase64 = privateKeyBase64.Replace("-----END PRIVATE KEY-----", "");

            RSA rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKeyBase64), out _);

            byte[] publicKey = rsa.ExportSubjectPublicKeyInfo();

            string hashStr = ComputeHash(publicKey);

            return ComputeAppId(hashStr);
        }

        private string ComputeAppId(string hashStr)
        {
            StringBuilder stringBuilder = new();
            int indexOfA = (byte)'a';
            for (int i = 0; i < 32; ++i)
            {
                int charOffset = int.Parse(hashStr[i].ToString(), System.Globalization.NumberStyles.HexNumber);
                stringBuilder.Append((char)(charOffset + indexOfA));
            }

            return stringBuilder.ToString();
        }

        private string ComputeHash(byte[] latin1Bytes)
        {
            SHA256 hasher = SHA256.Create();
            byte[] hash = hasher.ComputeHash(latin1Bytes);

            string hashStr = StringifyToHexDigits(hash);
            return hashStr;
        }

        private string StringifyToHexDigits(byte[] buf)
        {
            StringBuilder stringBuilder = new();
            foreach (byte item in buf)
            {
                stringBuilder.Append(item.ToString("x2"));
            }
            return stringBuilder.ToString();
        }
    }
}
