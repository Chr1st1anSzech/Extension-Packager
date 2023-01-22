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
        internal readonly bool _canOverwrite = false;

        internal ExtensionPostProcess(bool canOverwrite = false)
        {
            _canOverwrite = canOverwrite;
        }

        internal void CopyFile(string destinationFile, string sourceFile)
        {
            if (destinationFile.Equals(sourceFile)) return;

            _log.Debug($"Copy the CRX file \"{sourceFile}\" to destination \"{destinationFile}\".");

            if (File.Exists(destinationFile) && !_canOverwrite)
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

        internal void CreateExtensionDirectory(string directoryToCreate)
        {
            _log.Debug($"Create extension directory \"{directoryToCreate}\".");

            if (Directory.Exists(directoryToCreate) && Directory.GetFiles(directoryToCreate).Length != 0 && !_canOverwrite)
            {
                _log.Error($"The directory \"{directoryToCreate}\" already exists and is not empty.");
                throw new ArgumentException($"The directory \"{directoryToCreate}\" already exists and is not empty.");
            }

            try
            {
                Directory.CreateDirectory(directoryToCreate);
            }
            catch (Exception ex)
            {
                _log.Error($"The directory \"{directoryToCreate}\" could not be created.", ex);
                throw;
            }
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
