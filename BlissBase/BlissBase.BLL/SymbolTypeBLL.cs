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
    public class SymbolTypeBLL
    {
        bool testing;
        public SymbolTypeBLL()
        {
            testing = false;
        }
        public SymbolTypeBLL(bool testing_)
        {
            testing = testing_;
        }
        public List<SymbolType> GetAllNonStandard()
        {
            var SymbolTypeDAL = new SymbolTypeDAL(testing);
            return SymbolTypeDAL.GetAllNonStandard();
        }

        public SymbolType GetExactBySymID(int symId)
        {
            var SymbolTypeDAL = new SymbolTypeDAL(testing);
            return SymbolTypeDAL.GetExactBySymID(symId);
        }

        public bool UpdateTypeForSymID(int symId, TypeCodes code)
        {
            var SymbolTypeDAL = new SymbolTypeDAL(testing);
            return SymbolTypeDAL.UpdateTypeForSymID(symId, code);
        }

        public bool SetLanguageForSymID(int symId, TypeCodes code)
        {
            var SymbolTypeDAL = new SymbolTypeDAL(testing);
            return SymbolTypeDAL.SetLanguageForSymID(symId, code);
        }

        public bool SetStandardForSymID(int symId)
        {
            var SymbolTypeDAL = new SymbolTypeDAL(testing);
            return SymbolTypeDAL.SetStandardForSymID(symId);
        }
    }
}
