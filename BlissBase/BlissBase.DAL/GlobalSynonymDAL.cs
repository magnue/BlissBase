// DAL "Database Access Layer
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
    /// GlobalSynonymDAL: creates access to the GlobalSynonyms table
    /// This table contains all the synonyms that are globally approved.
    /// Unlike the UserSynonyms, these global synonyms are available
    /// to all users.
    /// </summary>
    public class GlobalSynonymDAL
    {
        bool testing;
        
        public GlobalSynonymDAL()
        {
            testing = false;
        }
        public GlobalSynonymDAL(bool testing_)
        {
            testing = testing_;
        }

        /// <summary>
        /// Returns all composite symbols that is GlobalSynonyms
        /// </summary>
        /// <returns>List of BlissBase.Model.GlobalSynonym or null</returns>
        public List<GlobalSynonym> GetAll()
        {
            using (var db = new BlissBaseContext(testing))
            {

                List<GlobalSynonym> allGlobalSynonyms = db.GlobalSynonyms.Select(g => new GlobalSynonym()
                {
                    gSynId = g.GSynID,
                    gSynSynonym = g.GSynSynonym,
                    gSynWord = g.GSynWord
                }
                    ).ToList();
                return (allGlobalSynonyms.Count == 0) ? allGlobalSynonyms : null;
            }
        }

        /// <summary>
        /// Returns all Global synonyms for a word
        /// </summary>
        /// <param name="word"></param>
        /// <returns>List of BlissBase.Model.GlobalSynonym or null</returns>
        public List<GlobalSynonym> GetSynonymForWord(CompositeSymbol word)
        {
            List<GlobalSynonym> allSynonymsForWord = new List<GlobalSynonym>();

            using (var db = new BlissBaseContext(testing))
            {
                allSynonymsForWord = db.GlobalSynonyms.Where(gS => gS.GSynWord == word.compId).Select(
                    gS => new GlobalSynonym
                    {
                        gSynId = gS.GSynID,
                        gSynWord = gS.GSynWord,
                        gSynSynonym = gS.GSynSynonym
                    }).ToList();
            }
            return allSynonymsForWord.Count == 0 ? null : allSynonymsForWord;
        }


        /// <summary>
        /// Returns exacly one GlobalSynonym Model by id
        /// or null if none exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>BlissBase.Model.GlobalSynonym or null</returns>
        public GlobalSynonym GetExactSynonym(int id)
        {
            using (var db = new BlissBaseContext(testing))
            {
                GlobalSynonyms row = db.GlobalSynonyms.Find(id);
                if (row != null)
                    return new GlobalSynonym()
                    {
                        gSynId = row.GSynID,
                        gSynSynonym = row.GSynSynonym,
                        gSynWord = row.GSynWord
                    };
                return null;
            }
        }

        /// <summary>
        /// Creates and inserts a synonym to the CompositeSymblos by name and components, 
        /// and defines it as a global synonym in GlobalSynonyms table with a link to a 
        /// word in CompositeSymbols table
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
                    db.GlobalSynonyms.Add(new GlobalSynonyms { GSynWord = word.compId, GSynSynonym = synonymId });
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception when trying to link new global synonym: " + synonymId + " with word: " + word.compId);
                    Debug.WriteLine(e.StackTrace);
                    return false;
                }
            }
        }

        /// <summary>
        /// Deletes row in GlobalSynonyms where GsynSynonym == id
        /// For the synonym found, the CompositeSymbol with CompId == id
        /// will be deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true or false depending on success</returns>
        public bool DeleteSynonym(int id)
        {
            using (var db = new BlissBaseContext(testing))
            {
                try
                {
                    GlobalSynonyms toRemove = db.GlobalSynonyms.Find(id);
                    db.GlobalSynonyms.Remove(toRemove);
                    CompositeSymbols compToRemove = db.CompositeSymbols.Find(id);
                    db.CompositeSymbols.Remove(compToRemove);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception when deleting global synonym: " + id);
                    Debug.WriteLine(e.StackTrace);
                    return false;
                }
            }
        }
    }
}
