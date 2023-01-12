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
        public void Set(DataModels.Extension extension);

        public void SetLastModified(DataModels.Extension extension);

        public List<DataModels.Extension> GetAll();

        public List<DataModels.Extension> GetAllLastModified();

        public DataModels.Extension Get(string id);
    }
}
