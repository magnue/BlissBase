// DAL "Database Access Layer
// All classes in DAL should be only access to DB
// All classes in DAL should only be accessibele from BLL

using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using BlissBase.Model;

namespace BlissBase.DAL
{
    /// <summary>
    /// UserDAL: Provides access to the database's Users table
    /// All useraccounts is defined in this table
    /// </summary>
    public class UserDAL
    {
        bool testing;
        public UserDAL()
        {
            testing = false;
        }
        public UserDAL(bool testing_)
        {
            testing = testing_;
        }

        /// <summary>
        /// Gets all the users "whithout password or salt"
        /// </summary>
        /// <returns>List of BlissBase.Model.User or null</returns>
        public List<User> GetAllWithoutPasswd()
        {
            using (var db = new BlissBaseContext(testing))
            {
                return db.Users.Select(u => new User
                {
                    userId = u.UserID,
                    username = u.UserName,
                    userFirstName = u.UserFirstName,
                    userLastName = u.UserLastName,
                    userApproved = u.UserApproved
                }).ToList();
            }
        }

        /// <summary>
        /// Gets an exact user with all information
        /// as a Users table row
        /// </summary>
        /// <param name="id"></param>
        /// <returns>BlissBase.DAL.Users or null</returns>
        internal Users GetExactRow(int id)
        {
            using (var db = new BlissBaseContext(testing))
            {
                return db.Users.Find(id);
            }
        }

        /// <summary>
        /// Gets an exact user with all information
        /// as a User model
        /// </summary>
        /// <param name="id"></param>
        /// <returns>BlissBase.Model.User or null</returns>
        public User GetExact(int id)
        {
            Users u = GetExactRow(id);
            return u == null ? null : new User
            {
                userId = u.UserID,
                username = u.UserName,
                userFirstName = u.UserFirstName,
                userLastName = u.UserLastName,
                userPasswd = u.UserPasswd,
                userSalt = u.UserSalt,
                userApproved = u.UserApproved
            };
        }

        /// <summary>
        /// Checks if username currently exists
        /// </summary>
        /// <param name="username"></param>
        /// <returns>true or false</returns>
        public bool CheckIfUsernameExists(string username)
        {
            using (var db = new BlissBaseContext(testing))
            {
                var users = db.Users.Select(u => new User
                {
                    userId = u.UserID,
                    username = u.UserName,
                    userFirstName = u.UserFirstName,
                    userLastName = u.UserLastName,
                    userApproved = u.UserApproved
                }).ToList();

                foreach (User user in users)
                {
                    if (user.username == username)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Takes a BlissBase.Model.User and inserts it
        /// to the BlissBase.DAL.Users table.
        /// The user is not approved and is awaiting approval by admin
        /// </summary>
        /// <param name="user"></param>
        /// <returns>true if successfull, else throws Exception</returns>
        public bool InsertUser(User user)
        {
            using (var db = new BlissBaseContext(testing))
            {
                string salt = generateSalt();

                Users toAdd = new Users
                {
                    UserID = user.userId,
                    UserName = user.username,
                    UserFirstName = user.userFirstName,
                    UserLastName = user.userLastName,
                    UserSalt = salt,
                    UserPasswd = hashString(user.userPasswd + salt),
                    UserApproved = false
                };
                try
                {
                    db.Users.Add(toAdd);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Takes a pasword and a username as two strings
        /// If user with username exists in BlissBase.DAL.Users table
        /// then the salt is collected and the password checked if match
        /// Returns the user as User object or null. If the user exists but
        /// is not approved null is returned
        /// </summary>
        /// <param name="passwd"></param>
        /// <param name="username"></param>
        /// <returns>BlissBase.Model.User or null</returns>
        public User LogInnUser(string passwd, string username)
        {
            using (var db = new BlissBaseContext(testing))
            {
                if (db.Users.Count() == 0)
                {
                    return null;
                }

                User toLogInn = db.Users.Where(u => username == u.UserName).Select(u => new User
                {
                    userId = u.UserID,
                    username = u.UserName,
                    userFirstName = u.UserFirstName,
                    userLastName = u.UserLastName,
                    userPasswd = u.UserPasswd,
                    userSalt = u.UserSalt,
                    userApproved = u.UserApproved
                }).First();

                if (toLogInn.userApproved &&
                        toLogInn.userPasswd == hashString(passwd + toLogInn.userSalt))
                    return toLogInn;
            }
            return null;
        }

        /// <summary>
        /// Takes a users id and bool status as parameters. Updates the given user's
        /// UserApproved status and sets it to status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns>true or false depending on success</returns>
        public bool UpdateUserApproved(int id, bool status)
        {
            try
            {
                using (var db = new BlissBaseContext(testing))
                {
                    Users toApprove = db.Users.Find(id);
                    toApprove.UserApproved = status;
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("An exception was thrown when changing userId: " + id +
                    " to approved status = " + status);
                Debug.WriteLine(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Takes an BlissBase.Model.User and delete it if
        /// it excists
        /// </summary>
        /// <param name="user"></param>
        /// <returns>return true or false depending on success, 
        /// or throws exception</returns>
        public bool DeleteUser(User user)
        {
            using (var db = new BlissBaseContext(testing))
            {
                Users toRemove = db.Users.Find(user.userId);
                if (toRemove != null)
                    try
                    {
                        db.Users.Remove(toRemove);
                        db.SaveChanges();
                        return true;
                    }
                    catch
                    {
                        throw;
                    }
                return false;
            }
        }

        /// <summary>
        /// Generates a random "salt" string with a length of 255 characters
        /// The salt is ment to "disguise" password in the db table, so it's not
        /// human readable
        /// </summary>
        /// <returns>string of salt</returns>
        private string generateSalt()
        {
            string saltbox = "1234567890qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
            string salt = "";
            Random rnd = new Random();
            for (int i = 0; i < 255; i++)
                salt += saltbox[rnd.Next(saltbox.Length - 1)];

            return salt;
        }

        /// <summary>
        /// Generates a password hash after kombining the password with salt
        /// </summary>
        /// <param name="toHash"></param>
        /// <returns>hashed password string</returns>
        private string hashString(string toHash)
        {
            var stringData = Encoding.ASCII.GetBytes(toHash);
            var hashData = new SHA256Managed().ComputeHash(stringData);
            string hash = string.Empty;

            foreach (var b in hashData)
                hash += b.ToString("X2");

            return hash;
        }
    }
}
