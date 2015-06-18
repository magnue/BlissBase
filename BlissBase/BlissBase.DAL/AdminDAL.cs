// DAL "Database Access Layer
// All classes in DAL should be only access to DB
// All classes in DAL should only be accessibele from BLL

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using BlissBase.Model;

namespace BlissBase.DAL
{
    /// <summary>
    /// All users whit their UserID in BlissBase.DAL.Admins
    /// table is considered a administrator
    /// </summary>
    public class AdminDAL
    {
        bool testing;
        public AdminDAL()
        {
            testing = false;
        }
        public AdminDAL(bool testing_)
        {
            testing = testing_;
        }

        /// <summary>
        /// Returns all the admins as a list of Admin
        /// </summary>
        /// <returns>List of BlissBase.Model.Admin or null</returns>
        public List<Admin> getAllAdmins()
        {
            using (BlissBaseContext db = new BlissBaseContext(testing))
            {
                return db.Admins.Select(a => new Admin
                {
                    userId = a.UserID
                }).ToList();
            }
        }

        /// <summary>
        /// Takes a UserID as a parameter and returns a Admins table row
        /// </summary>
        /// <param name="id"></param>
        /// <returns>BlissBase.DAL.Admins row or null</returns>
        internal Admins GetExcactByUserId(int id)
        {
            using (BlissBaseContext db = new BlissBaseContext(testing))
            {
                return db.Admins.Where(a => a.UserID == id).First();
            }
        }

        /// <summary>
        /// Checks if a user is an administrator
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true or false</returns>
        public bool CheckIfUserAdmin(int id)
        {
            Admins toCheck = GetExcactByUserId(id);
            return toCheck != null ? true : false;
        }

        /// <summary>
        /// Sets a user as a administrator by UserID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true or false depending on success, 
        /// if user allready is an admin true is returned</returns>
        public bool SetUserAdmin(int id)
        {
            if (!CheckIfUserAdmin(id))
            {
                Admins toAdd = new Admins
                {
                    UserID = id
                };
                try
                {
                    using (BlissBaseContext db = new BlissBaseContext(testing))
                    {
                        db.Admins.Add(toAdd);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("A exception was thrown when setting user: " + id + " as admin");
                    Debug.WriteLine(e.StackTrace);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Removes a users admin privileges
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true or false depending on success,
        /// if user is currently not an admin true is returned</returns>
        public bool DisableUserAdmin(int id)
        {
            Admins toRemove = GetExcactByUserId(id);
            if (toRemove != null)
            {
                try
                {
                    using (BlissBaseContext db = new BlissBaseContext(testing))
                    {
                        db.Admins.Remove(toRemove);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("A exception was thrown when remowing admin permissions for user: " + id);
                    Debug.WriteLine(e.StackTrace);
                    return false;
                }
            }
            return true;
        }
    }
}
