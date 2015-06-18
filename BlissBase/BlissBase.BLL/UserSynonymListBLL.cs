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
    public class UserSynonymListBLL
    {
        bool testing;
        public UserSynonymListBLL()
        {
            testing = false;
        }
        public UserSynonymListBLL(bool testing_)
        {
            testing = testing_;
        }
        public UserSynonymList InsertUser(int userId)
        {
            var UserSynonymListDAL = new UserSynonymListDAL(testing);
            return UserSynonymListDAL.InsertUser(userId);
        }

        public List<UserSynonym> GetSynonymForUser(int userId)
        {
            var UserSynonymListDAL = new UserSynonymListDAL(testing);
            return UserSynonymListDAL.GetSynonymForUser(userId);
        }

        public UserSynonymList FindUser(int id)
        {
            var UserSynonymListDAL = new UserSynonymListDAL(testing);
            return UserSynonymListDAL.FindUser(id);
        }

        public bool DeleteAllSynonymsForUser(int id)
        {
            var UserSynonymListDAL = new UserSynonymListDAL(testing);
            return UserSynonymListDAL.DeleteAllSynonymsForUser(id);
        }

        public bool DeleteUser(int id)
        {
            var UserSynonymListDAL = new UserSynonymListDAL(testing);
            return UserSynonymListDAL.DeleteUser(id);
        }
    }
}
