// DAL "Database Access Layer
// All classes in DAL should be only access to DB
// All classes in DAL should only be accessibele from BLL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using BlissBase.Model;


namespace BlissBase.DAL
{
    /// <summary>
    /// All composite symbols that do not exist in the
    /// Languages table is considered "international"
    /// All other comp symbols uses a language code from
    /// the BlissBase.Model.LanguageCodes enum in the
    /// BlissBase.Model.Language.cs file
    /// </summary>
    public class LanguageDAL
    {
        bool testing;
        public LanguageDAL()
        {
            testing = false;
        }
        public LanguageDAL(bool testing_)
        {
            testing = testing_;
        }

        /// <summary>
        /// Returns a Language model for all composite symbols that has a
        /// entry in Languages table "aka non international"
        /// </summary>
        /// <returns>List of BlissBase.Model.Language or null</returns>
        public List<Language> GetAllNonInternational()
        {
            using (var db = new BlissBaseContext(testing))
            {
                return db.Languages.Select(l => new Language
                {
                    compId = l.CompID,
                    langCode = l.LangCode
                }).ToList();
            }
        }

        /// <summary>
        /// Finds exact row from Languages table based on CompID
        /// </summary>
        /// <param name="compId"></param>
        /// <returns>BlissBase.DAL.Languages row or null</returns>
        internal Languages GetExactRowByCompID(int compId)
        {
            using (var db = new BlissBaseContext(testing))
            {
                return db.Languages.Find(compId);
            }
        }

        /// <summary>
        /// Finds exact model of Language based on CompID
        /// note: langCode is type LanguageCodes (enum)
        /// </summary>
        /// <param name="compId"></param>
        /// <returns>BlissBase.Model.Language or null</returns>
        public Language GetExactByCompID(int compId)
        {
            var toReturn = GetExactRowByCompID(compId);
            return new Language
            {
                compId = toReturn.CompID,
                langCode = toReturn.LangCode
            };
        }

        /// <summary>
        /// Updates a CompID's language code. Does not set language for a CompID
        /// only updates.
        /// </summary>
        /// <param name="compId"></param>
        /// <param name="code"></param>
        /// <returns>true or false depending on success</returns>
        public bool UpdateLanguageForCompID(int compId, LanguageCodes code)
        {
            var toUpdate = GetExactRowByCompID(compId);
            try
            {
                using (var db = new BlissBaseContext(testing))
                {
                    toUpdate.LangCode = code;
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("A Exception was thrown when trying to update language code for " +
                    "CompId: " + compId + ". Might be that composite sybol is defined as international");
                Debug.WriteLine(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Sets a non international language for a CompID
        /// </summary>
        /// <param name="compId"></param>
        /// <param name="code"></param>
        /// <returns>true or false depending on success</returns>
        public bool SetLanguageForCompID(int compId, LanguageCodes code)
        {
            var toAdd = new Languages
            {
                CompID = compId,
                LangCode = code
            };

            try
            {
                using (var db = new BlissBaseContext(testing))
                {
                    db.Languages.Add(toAdd);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("An exception was thrown when defining composite symbol: " + compId +
                    " as non international, with LanguageCodes: " + code);
                Debug.WriteLine(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Deletes a CompID from Languages table and effectively
        /// sets it as international
        /// </summary>
        /// <param name="compId"></param>
        /// <returns></returns>
        public bool SetInternationalForCompID(int compId)
        {
            var toRemove = GetExactRowByCompID(compId);
            try
            {
                using (var db = new BlissBaseContext(testing))
                {
                    db.Languages.Remove(toRemove);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("An exception was thrown when defining composite symbol: " + compId +
                    " as international");
                Debug.WriteLine(e.StackTrace);
                return false;
            }
        }
    }
}
