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
    public class CompositeSymbolBLL
    {
        bool testing;
        public CompositeSymbolBLL()
        {
            testing = false;
        }
        public CompositeSymbolBLL(bool testing_)
        {
            testing = testing_;
        }
        public List<CompositeSymbol> GetAll()
        {
            var CompositeSymbolDAL = new CompositeSymbolDAL(testing);
            return CompositeSymbolDAL.GetAll();
        }

        public CompositeSymbol GetExaxtComositeSymbolByID(int id)
        {
            var CompositeSymbolDAL = new CompositeSymbolDAL(testing);
            return CompositeSymbolDAL.GetExactCompositeSymbolByID(id);
        }

        public CompositeSymbol GetExaxtComositeSymbolByName(string name)
        {
            var CompositeSymbolDAL = new CompositeSymbolDAL(testing);
            return CompositeSymbolDAL.GetExactCompositeSymbolByName(name);
        }

        public CompositeSymbol CreateCompositeByComponents(String compositeName, List<Symbol> components)
        {
            var CompositeSymbolDAL = new CompositeSymbolDAL(testing);
            return CompositeSymbolDAL.CreateCompositeByComponents(compositeName, components);
        }

        public int Insert(CompositeSymbol innCompSymbol, List<Symbol> components)
        {
            var CompositeSymbolDAL = new CompositeSymbolDAL(testing);
            return CompositeSymbolDAL.Insert(innCompSymbol, components);
        }

        public bool UpdateCompositeName(int id, String name)
        {
            var CompositeSymbolDAL = new CompositeSymbolDAL(testing);
            return CompositeSymbolDAL.UpdateCompositeName(id, name);
        }

        public bool UpdateCompositeJPEG(int id, byte[] jpeg)
        {
            var CompositeSymbolDAL = new CompositeSymbolDAL(testing);
            return CompositeSymbolDAL.UpdateCompositeJPEG(id, jpeg);
        }

        public bool DeleteCompositeSymbol(CompositeSymbol compSymbol)
        {
            var CompositeSymbolDAL = new CompositeSymbolDAL(testing);
            return CompositeSymbolDAL.DeleteCompositeSymbol(compSymbol);
        }
    }
}
