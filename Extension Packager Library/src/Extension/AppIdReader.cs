/**
 * (c) 2013 Rob Wu <rob@robwu.nl>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Extension_Packager_Library.src.Extension
{
    public interface IAppIdReader
    {
        public string GetAppId(string crxPath);
    }

    public class AppIdReader : IAppIdReader
    {
        public string GetAppId(string crxPath)
        {
            if (!File.Exists(crxPath)) return null;

            byte[] buf = File.ReadAllBytes(crxPath);
            CrxInfo crx = new(buf);

            byte[] key = FindPublicKey(crx.PublicKey);

            string hashStr = ComputeHash(key);

            return ComputeAppId(hashStr);
        }

        private byte[] FindPublicKey(byte[] publicKey)
        {
            List<byte[]> publicKeys = new();
            byte[] crxIdBin = Array.Empty<byte>();
            for (int i = 0; i < publicKey.Length;)
            {
                int key = GetVarInt(publicKey, ref i);
                int length = GetVarInt(publicKey, ref i);

                if (key == 80002)
                {
                    int sigdatakey = GetVarInt(publicKey, ref i);
                    int sigdatalen = GetVarInt(publicKey, ref i);
                    if (sigdatakey != 0xA)
                    {
                        //throw new ArgumentException("proto: Unexpected key in signed_header_data: " + sigdatakey);
                    }
                    else if (sigdatalen != 16)
                    {
                        //throw new ArgumentException("proto: Unexpected signed_header_data length " + length);
                    }
                    else if (crxIdBin.Length != 0)
                    {
                        //throw new ArgumentException("proto: Unexpected duplicate signed_header_data");
                    }
                    else
                    {
                        crxIdBin = publicKey.Skip(i).Take(16).ToArray();
                    }
                    i += sigdatalen;
                    continue;
                }
                if (key != 0x12)
                {
                    if (key != 0x1a)
                    {
                        //throw new ArgumentException("proto: Unexpected key: " + key);
                    }
                    i += length;
                    continue;
                }

                var keyproofend = i + length;
                var keyproofkey = GetVarInt(publicKey, ref i);
                var keyprooflength = GetVarInt(publicKey, ref i);
                if (keyproofkey == 0x12)
                {
                    i += keyprooflength;
                    if (i >= keyproofend)
                    {
                        continue;
                    }
                    keyproofkey = GetVarInt(publicKey, ref i);
                    keyprooflength = GetVarInt(publicKey, ref i);
                }
                if (keyproofkey != 0xA)
                {
                    i += keyprooflength;
                    //throw new ArgumentException("proto: Unexpected key in AsymmetricKeyProof: " + keyproofkey);
                }
                if (i + keyprooflength >= publicKey.Length)
                {
                    //throw new ArgumentException("proto: size of public_key field is too large");
                }
                byte[] b = publicKey.Skip(i).Take(keyprooflength).ToArray();

                publicKeys.Add(b);
                i = keyproofend;
            }

            if (publicKeys.Count == 0)
            {
                throw new ArgumentException("proto: Did not find any public key");
            }
            if (crxIdBin == null)
            {
                throw new ArgumentException("proto: Did not find crx_id");
            }

            string crxIdHex = StringifyTwoHexDigits(crxIdBin.Skip(0).Take(16).ToArray());
            for (int j = 0; j < publicKeys.Count; ++j)
            {
                string sha256sum = ComputeHash(publicKeys[j]);
                string f = sha256sum[0..32];
                if (f == crxIdHex)
                {
                    return publicKeys[j];
                }
            }

            return null;
        }

        private int GetVarInt(byte[] buf, ref int i)
        {
            var val = buf[i] & 0x7F;
            if (buf[i++] < 0x80) return val;
            val |= (buf[i] & 0x7F) << 7;
            if (buf[i++] < 0x80) return val;
            val |= (buf[i] & 0x7F) << 14;
            if (buf[i++] < 0x80) return val;
            val |= (buf[i] & 0x7F) << 21;
            if (buf[i++] < 0x80) return val;
            val = (val | (buf[i] & 0xF) << 28);
            if (buf[i++] >= 0x80) throw new ArgumentException("proto: not a uint32");
            return val;
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

            string hashStr = StringifyTwoHexDigits(hash);
            return hashStr;
        }

        private string StringifyTwoHexDigits(byte[] buf)
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
