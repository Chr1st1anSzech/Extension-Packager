using log4net;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace Extension_Packager_Library.src.Settings
{
    public class SettingsFileReader : ISettingsReader
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISettingsReader _defaultSettingsReader;

        public SettingsFileReader(ISettingsReader defaultSettingsReader)
        {
            _defaultSettingsReader = defaultSettingsReader;
        }

        public DataModels.Settings ReadSettings()
        {
            if( SettingsFile.TryGetValidePath(out string path))
            {
                return JsonConvert.DeserializeObject<DataModels.Settings>(File.ReadAllText(path));
            }
            else
            {
                throw new FileNotFoundException($"{nameof(ReadSettings)}: The File was not found. Path = \"{path}\"");
            }
        }

        public DataModels.Settings ReadDefaultSettings()
        {
            return _defaultSettingsReader.ReadSettings();
        }
    }
}
