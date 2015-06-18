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
    public class UserSynonymBLL
    {
         bool testing;
        public UserSynonymBLL()
        {
            testing = false;
        }
        public UserSynonymBLL(bool testing_)
        {
            testing = testing_;
        }
        public List<UserSynonym> GetAll()
        {
            var UserSynonymDAL = new UserSynonymDAL(testing);
            return UserSynonymDAL.GetAll();
        }

        public List<UserSynonym> GetSynonymForWord(CompositeSymbol word)
        {
            var UserSynonymDAL = new UserSynonymDAL(testing);
            return UserSynonymDAL.GetSynonymForWord(word);
        }

        public List<UserSynonym> GetSynonymForUser(UserSynonymList user)
        {
            var UserSynonymDAL = new UserSynonymDAL(testing);
            return UserSynonymDAL.GetSynonymForUser(user);
        }

        public UserSynonym GetExactSynonym(int id)
        {
            var UserSynonymDAL = new UserSynonymDAL(testing);
            return UserSynonymDAL.GetExactSynonym(id);
        }

        public bool InsertSynonym(CompositeSymbol word, string name, List<Symbol> components)
        {
            var UserSynonymDAL = new UserSynonymDAL(testing);
            return UserSynonymDAL.InsertSynonym(word, name, components);
        }

        public bool DeleteSynonym(int id)
        {
            var UserSynonymDAL = new UserSynonymDAL(testing);
            return UserSynonymDAL.DeleteSynonym(id);
        }

        public bool SetUserSynonymApproved(int uSynId, bool bit)
        {
            var UserSynonymDAL = new UserSynonymDAL();
            return UserSynonymDAL.SetUserSynonymApproved(uSynId, bit);
        }
    }
}
