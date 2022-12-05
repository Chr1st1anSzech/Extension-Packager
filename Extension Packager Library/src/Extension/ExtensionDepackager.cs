/**
 * (c) 2013 Rob Wu <rob@robwu.nl>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using log4net;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Extension
{
    public interface IExtensionDepackager
    {
        /// <summary>
        /// Unpacks a CRX file to a defined output directory.
        /// </summary>
        /// <param name="sourceFilePath">The CRX file to be unpacked.</param>
        /// <param name="destinationDirectory">The output directory of the unpacked files.</param>
        public Task UnpackCrxAsync(string sourceFilePath, string destinationDirectory);
    }

    public class ExtensionDepackager : IExtensionDepackager
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Unpacks a CRX file to a defined output directory.
        /// </summary>
        /// <param name="sourceFilePath">The CRX file to be unpacked.</param>
        /// <param name="destinationDirectory">The output directory of the unpacked files.</param>
        public async Task UnpackCrxAsync(string sourceFilePath, string destinationDirectory)
        {
            _log.Debug($"Unzip the crx file \"{sourceFilePath}\" to destination \"{destinationDirectory}\".");
            byte[] buf = await ConvertToZipAsync(sourceFilePath);
            UnzipArchive(destinationDirectory, buf);
        }

        private void UnzipArchive(string destinationDirectory, byte[] buf)
        {
            _log.Debug("Load buffer into archive and unzip it.");
            try
            {
                Stream stream = new MemoryStream(buf);
                ZipArchive zip = new(stream);
                zip.ExtractToDirectory(destinationDirectory);
            }
            catch (Exception e)
            {
                _log.Error($"The archive file could not be unzipped to directory \"{destinationDirectory}\".", e);
                throw;
            }
        }

        private async Task<byte[]> ConvertToZipAsync(string sourceFilePath)
        {
            _log.Debug("Read the zip file to buffer.");
            try
            {
                byte[] buf = await File.ReadAllBytesAsync(sourceFilePath);
                buf = ExtractZipArchive(buf);
                return buf;
            }
            catch (Exception e)
            {
                _log.Error($"The CRX file \"{sourceFilePath}\" could not be read.", e);
                throw;
            }
        }


        private byte[] ExtractZipArchive(byte[] buf)
        {
            _log.Debug("Extract zip archive from crx file.");

            CrxInfo crx = new(buf);
            if (buf.Length < 16)
            {
                throw new ArgumentException("Invalid header");
            }

            if (crx.IsZipArchive)
            {
                return buf;
            }

            if (crx.MagicNumber != "Cr24")
            {
                throw new ArgumentException("Invalid header: Does not start with Cr24");
            }

            if (crx.Version != "2" && crx.Version != "3")
            {
                throw new ArgumentException("Unexpected crx format version number.");
            }

            int offset = crx.HeaderLength + crx.PublicKeyLength + crx.SignatureLength;

            return buf.Skip(offset).Take(buf.Length - offset).ToArray();
        }
    }
}
