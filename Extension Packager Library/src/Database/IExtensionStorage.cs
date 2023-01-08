using Extension_Packager_Library.src.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Database
{
    public interface IExtensionStorage
    {
        public void SaveExtension(DataModels.Extension extension);

        public void AddToLastUsedExtension(DataModels.Extension extension);

        public List<DataModels.Extension> ReadExtensions();

        public List<DataModels.Extension> ReadLastUsedExtensions();

        public DataModels.Extension ReadExtension(string id);
    }
}
