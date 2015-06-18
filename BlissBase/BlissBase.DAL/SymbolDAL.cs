// DAL "Database Access Layer"
// All classes in DAL should be only access to DB
// All classes in DAL should only be accessibele from BLL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using BlissBase.Model;
using System.Data.Entity;

namespace BlissBase.DAL
{
    /// <summary>
    /// SymbolDAL: Provides access to the database's Symbols table.
    /// These symbols are the buildingblocks for all composite sybols
    /// both "words" and "synonyms"
    /// </summary>
    public class SymbolDAL
    {
        bool testing;
        public SymbolDAL()
        {
            testing = false;
        }
        public SymbolDAL(bool testing_)
        {
            testing = testing_;
        }

        /// <summary>
        /// Query db for all base Symbols
        /// </summary>
        /// <returns>List of BlissBase.Model.Symbol</returns>
        public List<Symbol> GetAllSymbols()
        {
            using (var db = new BlissBaseContext(testing))
            {

                List<Symbol> symbols = db.Symbols.Select(sym => new Symbol()
                    {
                        symId = sym.SymID,
                        symName = sym.SymName,
                        symJpeg = sym.SymJPEG
                    }).ToList();

                return symbols;
            }
        }

        /// <summary>
        /// Returns Symbols Row of exact symbol by id
        /// If no symbol exists, then null is returned
        /// </summary>
        /// <param name="id"></param>
        /// <returns>BlissBase.DAL.Symbols or null</returns>
        internal Symbols GetExactRowByID(int id)
        {
            using (var db = new BlissBaseContext(testing))
            {
                Symbols sym;
                try
                {
                    sym = db.Symbols.Find(id);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("An Exception occored when trying to query db for exact symbol");
                    Debug.WriteLine(e.StackTrace);
                    return null;
                }
                return sym;
            }
        }

        /// <summary>
        /// Returns Symbol Model of exact symbol by id
        /// If no symbol exists, then null is returned
        /// </summary>
        /// <param name="id"></param>
        /// <returns>BlissBase.Model.Symbol or null</returns>
        public Symbol GetExactByID(int id)
        {
            Symbols row = GetExactRowByID(id);
            if (row != null)
                return new Symbol
                {
                    symId = row.SymID,
                    symName = row.SymName,
                    symJpeg = row.SymJPEG
                };
            return null;
        }

        /// <summary>
        /// Inserts new base symbol by name and jpeg
        /// </summary>
        /// <param name="name"></param>
        /// <param name="jpeg"></param>
        /// <returns>BlissBase.DAL.Symbols.SymID or -1 depending on success</returns>
        public int Insert(String name, byte[] jpeg)
        {
            var idToReturn = 0;
            using (var db = new BlissBaseContext(testing))
            {
                try
                {
                    Symbols newSymbol = new Symbols
                    {
                        SymName = name,
                        SymJPEG = jpeg
                    };
                    db.Symbols.Add(newSymbol);
                    db.SaveChanges();
                    idToReturn = newSymbol.SymID;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception occured when adding new symbol with name: " + name);
                    Debug.WriteLine(e.StackTrace);
                    return -1;
                }
            }
            return idToReturn;
        }

        /// <summary>
        /// Updates a base symbol's name by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns>true or false depending on success</returns>
        public bool UpdateName(int id, String name)
        {
            using (var db = new BlissBaseContext(testing))
            {
                Symbols sym = db.Symbols.Find(id);
                sym.SymName = name;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception when updating symbol name of symbol: " + id);
                    Debug.WriteLine(e.StackTrace);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Updates a base symbol's jpeg data by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="jpeg"></param>
        /// <returns>returns true or false depending on success</returns>
        public bool UpdateJPEG(int id, byte[] jpeg)
        {
            using (var db = new BlissBaseContext(testing))
            {
                Symbols sym = db.Symbols.Find(id);
                sym.SymJPEG = jpeg;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception when updating symbol jpeg of symbol: " + id);
                    Debug.WriteLine(e.StackTrace);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Deletes exacly one Symbol by id in Symbols table,
        /// deletes all the entries of the symbol in CompositesOf table
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true false depending on success</returns>
        public bool DeleteExact(int id)
        {
            using (var db = new BlissBaseContext(testing))
            {
                Symbols sym = db.Symbols.Find(id);
                try
                {
                    // Deletes the relations where the symbol is a component of
                    // a composite symbol
                    var compOf = new CompositeOfDAL();
                    compOf.DeleteBySymbolID(id);
                    // Sets the symbol as a standard type (Deletes it from 
                    // SymbolTypes table)
                    var type = new SymbolTypeDAL();
                    type.SetStandardForSymID(id);
                    // Deletes the symbol from Symbols table
                    db.Symbols.Remove(sym);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception when remowing symbol: " + id);
                    Debug.WriteLine(e.StackTrace);
                    return false;
                }
            }
            return true;
        }
    }
}
