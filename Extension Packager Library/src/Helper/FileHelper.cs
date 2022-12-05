// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using log4net;
using System.IO;
using System.Reflection;

namespace Extension_Packager_Library.src.Helper
{
    public class FileHelper
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static bool FileExists(string filePath)
        {
            bool exists = File.Exists(filePath);
            if (!exists)
            {
                _log.Warn($"The file \"{filePath}\" does not exist.");
            }
            return exists;
        }


        public static bool DirectoryExists(string directoryPath)
        {
            bool exists = Directory.Exists(directoryPath);
            if (!exists)
            {
                _log.Warn($"The directory \"{directoryPath}\" does not exist.");
            }
            return exists;
        }


        public static bool DirectoryNotExistsOrEmpty(string directoryPath)
        {
            bool exists = Directory.Exists(directoryPath);
            if(string.IsNullOrWhiteSpace(directoryPath) || !exists)
            {
                _log.Error($"The directory \"{directoryPath}\" is invalide.");
                return false;
            }

            bool isDirectoryEmpty = Directory.GetFiles(directoryPath).Length == 0;
            if (exists && !isDirectoryEmpty)
            {
                _log.Warn($"The directory \"{directoryPath}\" exists and is not empty.");
                return false;
            }
            return true;
        }


        public static string GetRandomDirectory(string destinationDirectory = null)
        {
            _log.Debug("Request a random directory.");
            string randomFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            string path = destinationDirectory ?? Path.GetTempPath();
            return Path.Combine(path, randomFileName);
        }


        public static string CreateRandomDirectory(string destinationDirectory = null)
        {
            _log.Debug("Create a random directory.");
            string path = GetRandomDirectory(destinationDirectory);
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
