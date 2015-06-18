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
    public class GlobalSynonymBLL
    {
        bool testing;
        public GlobalSynonymBLL()
        {
            testing = false;
        }
        public GlobalSynonymBLL(bool testing_)
        {
            testing = testing_;
        }
        public List<GlobalSynonym> GetAll()
        {
            var GlobalSynonymDAL = new GlobalSynonymDAL(testing);
            return GlobalSynonymDAL.GetAll();
        }

        public List<GlobalSynonym> GetSynonymForWord(CompositeSymbol word)
        {
            var GlobalSynonymDAL = new GlobalSynonymDAL(testing);
            return GlobalSynonymDAL.GetSynonymForWord(word);
        }

        public GlobalSynonym GetExactSynonym(int id)
        {
            var GlobalSynonymDAL = new GlobalSynonymDAL(testing);
            return GlobalSynonymDAL.GetExactSynonym(id);
        }

        public bool InsertSynonym(CompositeSymbol word, string name, List<Symbol> components)
        {
            var GlobalSynonymDAL = new GlobalSynonymDAL(testing);
            return GlobalSynonymDAL.InsertSynonym(word, name, components);
        }

        public bool DeleteSynonym(int id)
        {
            var GlobalSynonymDAL = new GlobalSynonymDAL(testing);
            return GlobalSynonymDAL.DeleteSynonym(id);
        }
    }
}
