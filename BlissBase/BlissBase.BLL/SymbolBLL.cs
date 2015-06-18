// BLL "Business Logic Layer"
// All the classes in the BLL layer should handle the application's
// use of the databaselayer "DAL". The BLL layer should only be accessed 
// from the BlissBase "web application" and not the DAL or Models layer.
// The BLL layer should be the only connection down to the DAL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlissBase.DAL;
using BlissBase.Model;

namespace BlissBase.BLL
{
    public class SymbolBLL
    {
        bool testing;
        public SymbolBLL()
        {
            testing = false;
        }
        public SymbolBLL(bool testing_)
        {
            testing = testing_;
        }
        public List<Symbol> GetAllSymbols()
        {
            var SymbolDAL = new SymbolDAL(testing);
            return SymbolDAL.GetAllSymbols();
        }

        public Symbol GetExactByID(int id)
        {
            var SymbolDAL = new SymbolDAL(testing);
            return SymbolDAL.GetExactByID(id);
        }

        public int Insert(String name, byte[] jpeg)
        {
            var SymbolDAL = new SymbolDAL(testing);
            return SymbolDAL.Insert(name, jpeg);
        }

        public bool UpdateName(int id, String name)
        {
            var SymbolDAL = new SymbolDAL(testing);
            return SymbolDAL.UpdateName(id, name);
        }

        public bool UpdateJPEG(int id, byte[] jpeg)
        {
            var SymbolDAL = new SymbolDAL(testing);
            return SymbolDAL.UpdateJPEG(id, jpeg);
        }

        public bool DeleteExact(int id)
        {
            var SymbolDAL = new SymbolDAL(testing);
            return SymbolDAL.DeleteExact(id);
        }
    }
}
