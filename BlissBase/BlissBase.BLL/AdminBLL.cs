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
    public class AdminBLL
    {
        bool testing;
        public AdminBLL()
        {
            testing = false;
        }
        public AdminBLL(bool testing_)
        {
            testing = testing_;
        }
        public List<Admin> getAllAdmins()
        {
            var AdminDAL = new AdminDAL(testing);
            return AdminDAL.getAllAdmins();
        }
        public bool CheckIfUserAdmin(int id)
        {
            var AdminDAL = new AdminDAL(testing);
            return AdminDAL.CheckIfUserAdmin(id);
        }
        public bool SetUserAdmin(int id)
        {
            var AdminDAL = new AdminDAL(testing);
            return AdminDAL.SetUserAdmin(id);
        }
        public bool DisableUserAdmin(int id)
        {
            var AdminDAL = new AdminDAL(testing);
            return AdminDAL.DisableUserAdmin(id);
        }
    }
}
