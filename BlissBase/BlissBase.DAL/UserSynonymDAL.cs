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
    /// UserSynonymDAL: Serves as a intermediate table for the
    /// many to many relations between "words" and "synonyms" 
    /// "CompositeSymbols" and the "UserSynonymsList".
    /// </summary>
    public class UserSynonymDAL
    {
        bool testing;
        public UserSynonymDAL()
        {
            testing = false;
        }
        public UserSynonymDAL(bool testing_)
        {
            testing = testing_;
        }

        /// <summary>
        /// Returns all composite symbols that is in UserSynonyms table
        /// </summary>
        /// <returns>List of BlissBase.Model.UserSynonym</returns>
        public List<UserSynonym> GetAll()
        {
            using (var db = new BlissBaseContext(testing))
            {

                List<UserSynonym> allUserSynonyms = db.UserSynonyms.Select(u => new UserSynonym()
                {
                    uSynId = u.USynID,
                    uSynSynonym = u.USynSynonym,
                    uSynWord = u.USynWord,
                    uListUserId = u.UserID,
                    uSynApproved = u.USynApproved
                }).ToList();
                return (allUserSynonyms.Count == 0) ? allUserSynonyms : null;
            }
        }

        /// <summary>
        /// Returns all UserSynonyms for a CompositeSymbol "word"
        /// </summary>
        /// <param name="word"></param>
        /// <returns>List of BlissBase.Model.UserSynonym or null</returns>
        public List<UserSynonym> GetSynonymForWord(CompositeSymbol word)
        {
            List<UserSynonym> allUserSynonymsForWord = new List<UserSynonym>();

            using (var db = new BlissBaseContext(testing))
            {
                allUserSynonymsForWord = db.UserSynonyms.Where(uS => uS.USynWord == word.compId).Select(
                    uS => new UserSynonym
                    {
                        uSynId = uS.USynID,
                        uSynWord = uS.USynWord,
                        uSynSynonym = uS.USynSynonym,
                        uListUserId = uS.UserID,
                        uSynApproved = uS.USynApproved
                    }).ToList();
            }
            return allUserSynonymsForWord.Count == 0 ? null : allUserSynonymsForWord;
        }

        /// <summary>
        /// Return all User synonyms for a user's list id "uListUserId"
        /// </summary>
        /// <param name="user"></param>
        /// <returns>List of BlissBase.Model.UserSynonym or null</returns>
        public List<UserSynonym> GetSynonymForUser(UserSynonymList user)
        {
            List<UserSynonym> allSynonymsForUser = new List<UserSynonym>();

            using (var db = new BlissBaseContext(testing))
            {
                allSynonymsForUser = db.UserSynonyms.Where(uS => uS.UserID == user.uListUserId).Select(
                    uS => new UserSynonym
                    {
                        uSynId = uS.USynID,
                        uSynWord = uS.USynWord,
                        uSynSynonym = uS.USynSynonym,
                        uListUserId = uS.UserID,
                        uSynApproved = uS.USynApproved
                    }).ToList();
            }
            return allSynonymsForUser.Count == 0 ? null : allSynonymsForUser;
        }

        /// <summary>
        /// Returns exacly one UserSynonym by id
        /// or null if none exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>BlissBase.Model.UserSynonym or null</returns>
        public UserSynonym GetExactSynonym(int id)
        {
            using (var db = new BlissBaseContext(testing))
            {
                UserSynonyms row = db.UserSynonyms.Find(id);
                if (row != null)
                    return new UserSynonym()
                    {
                        uSynId = row.USynID,
                        uSynWord = row.USynWord,
                        uSynSynonym = row.USynSynonym,
                        uListUserId = row.UserID,
                        uSynApproved = row.USynApproved
                    };
                return null;
            }
        }

        /// <summary>
        /// Creates and inserts a synonym to the CompositeSymblos, 
        /// and defines it as a UserSynonym
        /// </summary>
        /// <param name="word"></param>
        /// <param name="name"></param>
        /// <param name="components"></param>
        /// <returns>true or false depending on success</returns>
        public bool InsertSynonym(CompositeSymbol word, string name, List<Symbol> components)
        {
            var composite = new CompositeSymbolDAL();

            CompositeSymbol synonym = composite.CreateCompositeByComponents(name, components);
            int synonymId = composite.Insert(synonym, components);

            using (var db = new BlissBaseContext(testing))
            {
                try
                {
                    db.UserSynonyms.Add(new UserSynonyms { USynWord = word.compId, USynSynonym = synonymId });
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception when trying to link new user synonym: " + synonymId + " with word: " + word.compId);
                    Debug.WriteLine(e.StackTrace);
                    return false;
                }
            }
        }

        /// <summary>
        /// Deletes row in UserSynonyms where UsynSynonym == id
        /// For the synonym found, the CompositeSymbol with CompId == id
        /// will be deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true or false</returns>
        public bool DeleteSynonym(int id)
        {
            using (var db = new BlissBaseContext(testing))
            {
                try
                {
                    UserSynonyms toRemove = db.UserSynonyms.Find(id);
                    db.UserSynonyms.Remove(toRemove);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception when deleting user synonym: " + id);
                    Debug.WriteLine(e.StackTrace);
                    return false;
                }
            }
        }


        /// <summary>
        /// Sets a user synonym to approved or not
        /// </summary>
        /// <param name="bit"></param>
        /// <returns>true or false depending on successfulness</returns>
        public bool SetUserSynonymApproved(int uSynId, bool bit)
        {
            using (var db = new BlissBaseContext(testing))
            {
                UserSynonyms toEdit = db.UserSynonyms.Find(uSynId);
                if (toEdit != null)
                {
                    try
                    {
                        toEdit.USynApproved = bit;
                        db.SaveChanges();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Exception when approving user synonym: " + uSynId);
                        Debug.WriteLine(e.StackTrace);
                    }
                }
            }
            return false;
        }
    }
}
