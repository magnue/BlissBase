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
using System.Data.Entity;

namespace BlissBase.DAL
{
    /// <summary>
    /// All Symbols that does not have a entry in the Types
    /// table is considered standard symbols. All Symbols in
    /// Types has a TypeCodes (enum) value
    /// </summary>
    public class SymbolTypeDAL
    {
        bool testing;
        public SymbolTypeDAL()
        {
            testing = false;
        }
        public SymbolTypeDAL(bool testing_)
        {
            testing = testing_;
        }

        /// <summary>
        /// Gets all entries in SymbolTypes
        /// </summary>
        /// <returns>List of BlissBase.Model.SymbolType or null</returns>
        public List<SymbolType> GetAllNonStandard()
        {
            try
            {
                using (var db = new BlissBaseContext(testing))
                {
                    return db.SymbolTypes.Select(t => new SymbolType
                    {
                        symId = t.SymID,
                        typeIndicator = t.TypeIndicator
                    }).ToList();
                }
            } catch (Exception e)
            {
                Debug.WriteLine("An exception was thrown when trying to get all entries in SymbolTypes.");
                Debug.WriteLine(e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Returns a row from SymbolTypes table based on SymID
        /// </summary>
        /// <param name="symId"></param>
        /// <returns>BlissBase.DAL.SymbolTypes or null</returns>
        internal SymbolTypes GetExactRowBySymID(int symId)
        {
            try
            {
                using (var db = new BlissBaseContext(testing))
                {
                    return db.SymbolTypes.Find(symId);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("An exception was thrown when trying to GetExactRowBySymID from SymbolTypes " +
                    "for symId: " + symId);
                Debug.WriteLine(e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Returns a SymbolType model by SymID or null
        /// </summary>
        /// <param name="symId"></param>
        /// <returns>BlissBase.Model.SymbolType or null</returns>
        public SymbolType GetExactBySymID(int symId)
        {
            var toReturn = GetExactRowBySymID(symId);
            if (toReturn == null)
                return null;
            return new SymbolType
            {
                symId = toReturn.SymID,
                typeIndicator = toReturn.TypeIndicator
            };
        }

        /// <summary>
        /// Updates a SymID's TypeIndicator code. Does not set type for a SymID
        /// only updates.
        /// </summary>
        /// <param name="symId"></param>
        /// <param name="code"></param>
        /// <returns>true or false depending on success</returns>
        public bool UpdateTypeForSymID(int symId, TypeCodes code)
        {
            var toUpdate = GetExactRowBySymID(symId);
            try
            {
                using (var db = new BlissBaseContext(testing))
                {
                    toUpdate.TypeIndicator = code;
                    db.Entry(toUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("An exception was thrown when trying to update TypeIndicator code for " +
                    "symId: " + symId);
                Debug.WriteLine(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Sets a non standard type: "TypeCodes code" for a SymID symId
        /// </summary>
        /// <param name="symId"></param>
        /// <param name="code"></param>
        /// <returns>true or false depending on success</returns>
        public bool SetLanguageForSymID(int symId, TypeCodes code)
        {
            var toAdd = new SymbolTypes
            {
                SymID = symId,
                TypeIndicator = code
            };

            try
            {
                using (var db = new BlissBaseContext(testing))
                {
                    db.SymbolTypes.Add(toAdd);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("An exception was thrown when trying to set TypeCode: " + code +
                    " for symId: " + symId);
                Debug.WriteLine(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Deletes a SymID from SymbolTypes table and effectively
        /// sets it as standard
        /// </summary>
        /// <param name="symId"></param>
        /// <returns>true or false depending on success</returns>
        public bool SetStandardForSymID(int symId)
        {
            var toRemove = GetExactRowBySymID(symId);
            try
            {
                using (var db = new BlissBaseContext(testing))
                {
                    db.SymbolTypes.Attach(toRemove);
                    db.Entry(toRemove).State = EntityState.Deleted;
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception thrown when trying to set symId: " + symId +
                    " as standard SymbolType");
                Debug.WriteLine(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Checks if symbol with id as parameter symbolId
        /// has a type that exists in the SymbolType DB.
        /// </summary>
        /// <param name="symbolId"></param>
        /// <returns>true if symbol has a type, false otherwise</returns>
        public bool HasType(int symbolId)
        {
            if(symbolId != 0)
            {
                using (var db = new BlissBaseContext(testing))
                {
                    var hasType = db.SymbolTypes.Where(t => t.SymID == symbolId).First();
                    if (hasType != null)
                        return true;
                    else
                        return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
