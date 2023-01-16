// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.Settings;
using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        public static void OpenDirectory(string directory)
        {
            if(!DirectoryExists(directory)) return;

            try
            {
                Process.Start("explorer.exe", directory);
            }
            catch (Exception exception)
            {
                _log.Warn(StringResources.Get("FileHelper", 4, directory), exception);
            }
        }

        public static string[] FindFiles(string searchDirectory, string fileExtension)
        {
            if (!Directory.Exists(searchDirectory))
            {
                _log.Warn(StringResources.Get("FileHelper", 1, searchDirectory));
                return Array.Empty<string>();
            }

            if (string.IsNullOrWhiteSpace(fileExtension) || ContainsInvalidChars(fileExtension))
            {
                _log.Warn(StringResources.Get("FileHelper", 2));
                return Array.Empty<string>();
            }

            if (fileExtension[0] != '.')
            {
                fileExtension = fileExtension.Prepend('.').ToString();
            }
            try
            {
                return Directory.GetFiles(searchDirectory, $"*{fileExtension}");
            }
            catch
            {
                _log.Warn(StringResources.Get("FileHelper", 3));
                return Array.Empty<string>();
            }
        }

        private static bool ContainsInvalidChars(string path)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return path.Any(c => invalidChars.Contains(c));
        }
    }
}
