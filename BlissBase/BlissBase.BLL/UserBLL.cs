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
    public class UserBLL
    {
        bool testing;
        public UserBLL()
        {
            testing = false;
        }
        public UserBLL(bool testing_)
        {
            testing = testing_;
        }
        public List<User> GetAllWithoutPasswd()
        {
            var UserDAL = new UserDAL(testing);
            return UserDAL.GetAllWithoutPasswd();
        }

        public User GetExact(int id)
        {
            var UserDAL = new UserDAL(testing);
            return UserDAL.GetExact(id);
        }

        public bool CheckIfUsernameExists(string username)
        {
            var UserDAL = new UserDAL(testing);
            return UserDAL.CheckIfUsernameExists(username);
        }

        public bool InsertUser(User user)
        {
            var UserDAL = new UserDAL(testing);
            return UserDAL.InsertUser(user);
        }

        public User LogInnUser(string passwd, string username)
        {
            var UserDAL = new UserDAL(testing);
            return UserDAL.LogInnUser(passwd, username);
        }

        public bool UpdateUserApproved(int id, bool status)
        {
            var UserDAL = new UserDAL(testing);
            return UserDAL.UpdateUserApproved(id, status);
        }

        public bool DeleteUser(User user)
        {
            var UserDAL = new UserDAL(testing);
            return UserDAL.DeleteUser(user);
        }
    }
}
