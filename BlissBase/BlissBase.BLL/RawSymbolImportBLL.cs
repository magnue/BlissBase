// BLL "Business Logic Layer"
// All the classes in the BLL layer should handle the application's
// use of the databaselayer "DAL". The BLL layer should only be accessed 
// from the BlissBase "web application" and not the DAL or Models layer.
// The BLL layer should be the only connection down to the DAL

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using BlissBase.DAL;
using BlissBase.Model;

namespace BlissBase.BLL
{
    public class RawSymbolImportBLL
    {
        bool testing;
        public RawSymbolImportBLL()
        {
            testing = false;
        }
        public RawSymbolImportBLL(bool testing_)
        {
            testing = testing_;
        }
        public bool ImportFromPath(String path)
        {
            var RawSymbolsImportDAL = new RawSymbolImportDAL(testing);
            return RawSymbolsImportDAL.ImportFromPath(path);
        }

        public List<RawSymbolImport> GetAll()
        {
            var RawSymbolsImportDAL = new RawSymbolImportDAL(testing);
            return RawSymbolsImportDAL.GetAll();
        }

        public RawSymbolImport GetExact(int id)
        {
            var RawSymbolsImportDAL = new RawSymbolImportDAL(testing);
            return RawSymbolsImportDAL.GetExact(id);
        }

        public bool DeleteAll()
        {
            var RawSymbolsImportDAL = new RawSymbolImportDAL(testing);
            return RawSymbolsImportDAL.DeleteAll();
        }

        public bool DeleteExact(int id)
        {
            var RawSymbolsImportDAL = new RawSymbolImportDAL(testing);
            return RawSymbolsImportDAL.DeleteExact(id);
        }
    }
}
