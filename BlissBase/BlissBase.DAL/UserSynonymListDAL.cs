// DAL "Database Access Layer"
// All classes in DAL should be only access to DB
// All classes in DAL should only be accessibele from BLL

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlissBase.Model;

namespace BlissBase.DAL
{
    /// <summary>
    /// UserSynonymListDAL: Creates acces to tables containing "list"
    /// over all users self defined synonyms. These symbols "synonyms" 
    /// are not avalible for use by other users. The synonyms must 
    /// be approved by a "administrative" user before they can be used
    /// freely, and moved to GlobalSynonyms to be used by all users
    /// </summary>
    public class UserSynonymListDAL
    {
        bool testing;
        public UserSynonymListDAL()
        {
            testing = false;
        }
        public UserSynonymListDAL(bool testing_)
        {
            testing = testing_;
        }

        /// <summary>
        /// Inserts a new user and returns a UserSynonymsList model
        /// for the new user
        /// </summary>
        /// <param name="id"></param>
        /// <returns>BlissBase.Model.UserSynonymList or null</returns>
        public UserSynonymList InsertUser(int userId)
        {
            using (var db = new BlissBaseContext(testing))
            {
                try
                {
                    UsersSynonymsList toInsert = new UsersSynonymsList { UserID = userId };
                    db.UsersSynonymsList.Add(toInsert);
                    db.SaveChanges();
                    return new UserSynonymList { uListUserId = toInsert.UserID };
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception Adding user id: " + userId + " to UserSynonymsList");
                    Debug.WriteLine(e.StackTrace);
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns all synonyms for a user by id as a
        /// List of UserSynonym
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of BlissBase.Model.UserSynonym or null</returns>
        public List<UserSynonym> GetSynonymForUser(int userId)
        {
            var uList = FindUser(userId);
            UserSynonymDAL uSynDal = new UserSynonymDAL();
            List<UserSynonym> uSynList = uSynDal.GetSynonymForUser(uList);
            return uSynList.Count == 0 ? null : uSynList;
        }

        /// <summary>
        /// Returns a users UserSynonymlist Model by id if it exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns>BlissBase.Model.UserSynonymList or null</returns>
        public UserSynonymList FindUser(int userId)
        {
            using (var db = new BlissBaseContext(testing))
            {
                return db.UsersSynonymsList.Where(uSL => uSL.UserID == userId).Select(uSL => new UserSynonymList
                {
                    uListUserId = uSL.UserID
                }).First();
            }
        }

        /// <summary>
        /// Deletes all of the synonyms for a user by id
        /// </summary>
        /// <returns>true or false depending on success</returns>
        public bool DeleteAllSynonymsForUser(int userId)
        {
            List<UserSynonym> uSynModels = GetSynonymForUser(userId);

            using (var db = new BlissBaseContext(testing))
            {
                foreach (UserSynonym model in uSynModels)
                {
                    db.UserSynonyms.Remove(new UserSynonyms
                    {
                        USynID = model.uSynId,
                        USynApproved = model.uSynApproved,
                        USynSynonym = model.uSynSynonym,
                        USynWord = model.uSynWord
                    });
                }
                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception when deleting all synonyms for user: " + userId);
                    Debug.WriteLine(e.StackTrace);
                    return false;
                }
            }
        }

        /// <summary>
        /// Delete user from UsersSynonymsList table by users Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>true or false depending on success</returns>
        public bool DeleteUser(int userId)
        {
            using (var db = new BlissBaseContext(testing))
            {
                try
                {
                    UserSynonymList toRemove = FindUser(userId);
                    if (toRemove != null)
                    {
                        db.UsersSynonymsList.Remove(new UsersSynonymsList
                        {
                            UserID = toRemove.uListUserId
                        });
                    }
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception removing user: " + userId + " synonym list");
                    Debug.WriteLine(e.StackTrace);
                    return false;
                }
            }
        }
    }
}
