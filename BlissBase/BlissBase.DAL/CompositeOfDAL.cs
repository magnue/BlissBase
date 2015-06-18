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
    /// CompositeOfDAL provides access to the CompositeOf table.
    /// this table is a intermediate table for the many to many
    /// relation between CompositSymbols and Symbols
    /// </summary>
    public class CompositeOfDAL
    {
        bool testing;
        public CompositeOfDAL()
        {
            testing = false;
        }
        public CompositeOfDAL(bool testing_)
        {
            testing = testing_;
        }

        /// <summary>
        /// Returns all relations between symbols and composites
        /// </summary>
        /// <param name=""></param>
        /// <returns>List of BlissBase.Model.CompositeOf or null</returns>
        public List<CompositeOf> getAllRelations()
        {
            using (var db = new BlissBaseContext(testing))
            {
                List<CompositeOf> allRelations = db.CompositesOf.Select(cS => new CompositeOf()
                {
                    compOfId = cS.CompOfID,
                    compId = cS.CompID,
                    symId = cS.SymID
                }
                    ).ToList();
                return (allRelations.Count == 0) ? null : allRelations; 
            }
        }

        /// <summary>
        /// Returns all base symbols the innCompSymbol is composed of
        /// </summary>
        /// <param name="innCompSymbol"></param>
        /// <returns>List of BlissBase.Model.Symbol or null</returns>
        public List<Symbol> GetComponentsOf(CompositeSymbol innCompSymbol)
        {
            using (var db = new BlissBaseContext(testing))
            {
                List<CompositesOf> rows = db.CompositesOf.Where(e => e.CompID == innCompSymbol.compId).ToList();
                List<Symbols> symbols = new List<Symbols>();
                foreach(CompositesOf co in rows)
                {
                    Symbols temp = db.Symbols.Where(s => s.SymID == co.SymID).First();
                    symbols.Add(temp);
                }
                //List<Symbols> symbolRows = db.CompositesOf.Where(e => e.CompID == innCompSymbol.compId).Select(e => e.Symbols).ToList();
                List<Symbol> components = new List<Symbol>();

                foreach (Symbols t in symbols)
                {
                    components.Add(
                    new Symbol()
                    {
                        symId = t.SymID,
                        symName = t.SymName,
                        symJpeg = t.SymJPEG
                    });
                }
                return components;

            }
        }

        /// <summary>
        /// Method accepts a Row from CompositeSymbols table, and a 
        /// list of Symbol Models. The ComposedOf table is updated 
        /// to keep track of the relation between the composit symbol 
        /// and  it's base symbols
        /// </summary>
        /// <param name="compositeRow"></param>
        /// <param name="composedOf"></param>
        /// <returns>true or false depending on success</returns>
        internal bool SetCompositeOfRow(CompositeSymbols compositeRow, List<Symbol> composedOf)
        {

            CompositeSymbols inn = compositeRow;
            return SetCompositeOfSymbol(new CompositeSymbol() { 
                compId = inn.CompID, 
                compName = inn.CompName, 
                compJpeg = inn.CompJPEG
            }, composedOf);
        }

        /// <summary>
        /// Method accepts a Model of CompositeSymbols table, and a 
        /// list of Symbol Models. The ComposedOf table is updated 
        /// to keep track of the relation between the composit symbol 
        /// and  it's base symbols
        /// </summary>
        /// <param name="compositeModel"></param>
        /// <param name="composedOf"></param>
        /// <returns>true or false depending on success</returns>
        public bool SetCompositeOfSymbol(CompositeSymbol compositeModel, List<Symbol> composedOf)
        {
            // Reference used: http://stackoverflow.com/a/3309230
            List<Symbol> sortedList = composedOf.OrderBy(s => s.symId).ToList();

            using (var db = new BlissBaseContext(testing))
            {
                foreach (Symbol sym in composedOf)
                    try
                    {
                        db.CompositesOf.Add(new CompositesOf() { CompID = compositeModel.compId, SymID = sym.symId });
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Error setting CompositedOf: ", e.ToString());
                        Debug.WriteLine(e.StackTrace);
                        return false;
                    }

                return true;
            }
        }

        /// <summary>
        /// Deletes all rows in CompositesOf table by CompID
        /// "Same as DeleteByCompositeSymbol but takes a row from
        /// CompositeSymbols table instead of a Model of
        /// CompositeSymbol
        /// </summary>
        /// <param name="composit"></param>
        /// <returns>true or false depending on success</returns>
        internal bool DeleteByCompositeRow(CompositeSymbols composit)
        {
            return DeleteByCompositeSymbol(new CompositeSymbol()
            {
                compId = composit.CompID,
                compName = composit.CompName,
                compJpeg = composit.CompJPEG
            });
        }

        /// <summary>
        /// Deletes all rows in CompositesOf table by CompID
        /// </summary>
        /// <param name="composit"></param>
        /// <returns>true or false depending on success</returns>
        public bool DeleteByCompositeSymbol(CompositeSymbol composit)
        {
            using (var db = new BlissBaseContext(testing))
            {
                var composedOf = db.CompositesOf.Where(c => c.CompID == composit.compId);
                foreach (CompositesOf comp in composedOf)
                {
                    db.CompositesOf.Remove(comp);
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception occured when deleting composed of composite relation");
                    Debug.WriteLine(e.StackTrace);
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Deletes all rows in CompositesOf table by component id 
        /// </summary>
        /// <param name="symId"></param>
        /// <returns>true or false, depending on success</returns>
        internal bool DeleteBySymbolID(int symId)
        {
            using (var db = new BlissBaseContext(testing))
            {
                var components = db.CompositesOf.Where(c => c.SymID == symId);
                foreach (CompositesOf comp in components)
                {
                    db.CompositesOf.Remove(comp);
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception occured when deleting componet: " + symId +
                        " from CompositesOf table");
                    Debug.WriteLine(e.StackTrace);
                    return false;
                }
                return true;
            }
        }
    }
}

/* REFERENCES USED IN THIS FILE
 * http://stackoverflow.com/questions/5706437/whats-the-difference-between-inner-join-left-join-right-join-and-full-join
 * https://smehrozalam.wordpress.com/2010/06/29/entity-framework-queries-involving-many-to-many-relationship-tables/
*/