// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using log4net;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Extension
{
    public class ExtensionPostProcess
    {
        internal static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        internal readonly bool _isUpdate = false;

        internal ExtensionPostProcess(bool isUpdate = false)
        {
            _isUpdate = isUpdate;
        }

        internal void CopyFile(string destinationFile, string sourceFile)
        {
            if (destinationFile.Equals(sourceFile)) return;

            _log.Debug($"Copy the CRX file \"{sourceFile}\" to destination \"{destinationFile}\".");

            if (File.Exists(destinationFile) && !_isUpdate)
            {
                _log.Warn($"The CRX file {destinationFile} already exists.");
                throw new ArgumentException($"The CRX file {destinationFile} already exists.");
            }
            try
            {
                File.Copy(sourceFile, destinationFile, true);
            }
            catch
            {
                _log.Error($"Could not copy file \"{sourceFile}\" to destination \"{destinationFile}\".");
                throw;
            }
        }

        internal string CreateExtensionDirectory(string destinationDirectory, string extensionName)
        {
            string newDirectory = Path.Combine(destinationDirectory, extensionName);

            _log.Debug($"Create extension directory \"{newDirectory}\".");

            if (Directory.Exists(newDirectory) && Directory.GetFiles(newDirectory).Length != 0 && !_isUpdate)
            {
                _log.Error($"The directory \"{newDirectory}\" already exists and is not empty.");
                throw new ArgumentException($"The directory \"{newDirectory}\" already exists and is not empty.");
            }

            try
            {
                Directory.CreateDirectory(newDirectory);
            }
            catch (Exception ex)
            {
                _log.Error($"The directory \"{newDirectory}\" could not be created.", ex);
                throw;
            }

            return newDirectory;
        }

        internal async Task<string> WriteXmlManifestAsync(string xmlManifest, string xmlManifestFile, string destinationDirectory)
        {
            string destinationFile = Path.Combine(destinationDirectory, xmlManifestFile);

            _log.Debug($"Write the XML manifest to the file \"{destinationFile}\".");

            try
            {
                await File.WriteAllTextAsync(destinationFile, xmlManifest);
                return destinationFile;
            }
            catch (Exception ex)
            {
                _log.Error($"The XML manifest could not be written to the file \"{destinationFile}\".", ex);
                throw;
            }
        }
    }
}
