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
        public DataModels.Extension GetLastModified(string id);

        public List<DataModels.Extension> GetAllLastModified();

        public void SetLastModified(DataModels.Extension extension);


        public DataModels.Extension Get(string id);

        public List<DataModels.Extension> GetAll();

        public void Set(DataModels.Extension extension);

        public void Delete(string id);
    }
}
