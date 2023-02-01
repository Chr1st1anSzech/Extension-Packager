/**
 * (c) 2013 Rob Wu <rob@robwu.nl>
 * This Source Code Form is subject to the terms of the Mozilla internal
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Linq;
using System.Text;

namespace Extension_Packager_Library.src.Extension
{
    internal class CrxInfo
    {
        #region Properties

        internal string Version
        {
            get
            {
                return _buf[4].ToString();
            }
        }

        internal int PublicKeyLength
        {
            get
            {
                return CalcLength(_buf[8], _buf[9], _buf[10], _buf[11]);
            }
        }

        internal byte[] PublicKey
        {
            get
            {
                return GetPublicKey();
            }
        }

        internal int HeaderLength
        {
            get
            {
                return 12 + (Version == "2" ? 4 : 0);
            }
        }

        internal int SignatureLength
        {
            get
            {
                if (Version != "2") return 0;

                return CalcLength(_buf[12], _buf[13], _buf[14], _buf[15]);
            }
        }

        internal bool IsZipArchive
        {
            get
            {
                if (MagicNumber == null) return false;
                return MagicNumber.StartsWith("PK$&".Replace('$', (char)3).Replace('&', (char)4));
            }
        }

        internal string MagicNumber
        {
            get
            {
                byte[] bytes = new byte[4] { 67, 114, 50, 52 };
                StringBuilder stringBuilder = new();
                foreach (byte item in bytes)
                {
                    stringBuilder.Append((char)item);
                }
                return stringBuilder.ToString();
            }
        }

        #endregion

        private readonly byte[] _buf;

        internal CrxInfo(byte[] buf)
        {
            if (buf == null) throw new ArgumentNullException(nameof(buf));
            if (buf.Length < 16) throw new ArgumentException(nameof(buf), "Invalid header");
            _buf = buf;
        }

        private byte[] GetPublicKey()
        {
            if (Version == null) return null;

            int offset = 12 + SignatureLength;
            return _buf.Skip(offset).Take(PublicKeyLength).ToArray();
        }

        private int CalcLength(int a, int b, int c, int d)
        {
            int length = 0;

            length += a << 0;
            length += b << 8;
            length += c << 16;
            length += d << 24;
            return length;
        }
    }
}
