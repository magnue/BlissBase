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
    public class CompositeOfBLL
    {
        bool testing;
        public CompositeOfBLL()
        {
            testing = false;
        }
        public CompositeOfBLL(bool testing_)
        {
            testing = testing_;
        }
        public List<CompositeOf> getAllRelations()
        {
            var CompositeOfDAL = new CompositeOfDAL(testing);
            return CompositeOfDAL.getAllRelations();
        }

        public List<Symbol> GetComponentsOf(CompositeSymbol innCompSymbol)
        {
            var CompositeOfDAL = new CompositeOfDAL(testing);
            return CompositeOfDAL.GetComponentsOf(innCompSymbol);
        }

        public bool SetCompositeOfSymbol(CompositeSymbol compositeModel, List<Symbol> composedOf)
        {
            var CompositeOfDAL = new CompositeOfDAL(testing);
            return CompositeOfDAL.SetCompositeOfSymbol(compositeModel, composedOf);
        }

        public bool DeleteByCompositeSymbol(CompositeSymbol composit)
        {
            var CompositeOfDAL = new CompositeOfDAL(testing);
            return CompositeOfDAL.DeleteByCompositeSymbol(composit);
        }
    }
}
