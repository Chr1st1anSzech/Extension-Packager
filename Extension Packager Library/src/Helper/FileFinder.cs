using log4net;
using Microsoft.UI.Xaml.Shapes;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Helper
{
    public class FileFinder
    {
        #region Static Fields

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion


        public string[] FindFiles(string searchDirectory, string fileExtension)
        {
            if (!Directory.Exists(searchDirectory))
            {
                _log.Warn(StringResources.Get(this, 1, searchDirectory));
                return Array.Empty<string>();
            }

            if (string.IsNullOrWhiteSpace(fileExtension) || ContainsInvalidChars(fileExtension))
            {
                _log.Warn(StringResources.Get(this, 2));
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
                _log.Warn(StringResources.Get(this, 3));
                return Array.Empty<string>();
            }
        }

        private bool ContainsInvalidChars(string path)
        {
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
            return path.Any(c => invalidChars.Contains(c));
        }
    }
}
