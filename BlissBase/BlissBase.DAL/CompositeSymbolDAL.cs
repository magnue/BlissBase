// DAL "Database Access Layer
// All classes in DAL should be only access to DB
// All classes in DAL should only be accessibele from BLL

using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using BlissBase.Model;

namespace BlissBase.DAL
{
    /// <summary>
    /// CompositeSymbolsDAL creates access to the CompositeSymbols
    /// table. They have a many to many relation to the Symbols table 
    /// trough a intermediate table "CompositesOf".
    /// CompositeSymbols is all the "words" and "synonyms" that are
    /// composed of more than one individual symbol
    /// </summary>
    public class CompositeSymbolDAL
    {
        bool testing;
        public CompositeSymbolDAL()
        {
            testing = false;
        }
        public CompositeSymbolDAL(bool testing_)
        {
            testing = testing_;
        }

        /// <summary>
        /// Returns all composite symbols as list of CompositeSymbol Models
        /// </summary>
        /// <returns>List of BlissBase.Model.CompositeSymbol</returns>
        public List<CompositeSymbol> GetAll()
        {
            using (var db = new BlissBaseContext(testing))
            {

                List<CompositeSymbol> allCompSymbols = db.CompositeSymbols.Select(cS => new CompositeSymbol()
                    {
                        compId = cS.CompID,
                        compName = cS.CompName,
                        compJpeg = cS.CompJPEG
                    }
                    ).ToList();
                return (allCompSymbols.Count == 0) ? null : allCompSymbols;
            }
        }

        /// <summary>
        /// Takes an id and returns exact symbol as 
        /// CompositeSymbol Model or null if not found
        /// </summary>
        /// <param name="id"></param>
        /// <returns>BlissBase.Model.CompositeSymbol or null if not found</returns>
        public CompositeSymbol GetExactCompositeSymbolByID(int id)
        {
            CompositeSymbols row = GetExactCompositeRowByID(id);
            return new CompositeSymbol()
            {
                compId = row.CompID,
                compName = row.CompName,
                compJpeg = row.CompJPEG
            };
        }

        /// <summary>
        /// Takes an id and returns exact CompoiteSymbol as an row from 
        /// CompositeSymbols table. Or null if none exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>BlissBase.DAL.CompositeSymbols or null if not found</returns>
        internal CompositeSymbols GetExactCompositeRowByID(int id)
        {
            using (var db = new BlissBaseContext(testing))
            {
                return db.CompositeSymbols.Where(c => c.CompID == id).First();
            }
        }

        /// <summary>
        /// Takes a composite symbol's name and returns exact 
        /// CompositeSymbol as a model, or null if not exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns>BlissBase.Model.CompositeSymbol or null</returns>
        public CompositeSymbol GetExactCompositeSymbolByName(string name)
        {
            CompositeSymbols row = GetExactCompositeRowByName(name);
            return new CompositeSymbol()
            {
                compId = row.CompID,
                compName = row.CompName,
                compJpeg = row.CompJPEG
            };
        }

        /// <summary>
        /// Takes a CompositeSymbol name and returns exact symbol as
        /// row, or null if not exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns>BlissBase.DAL.CompositeSymbols or null</returns>
        internal CompositeSymbols GetExactCompositeRowByName(string name)
        {
            using (var db = new BlissBaseContext(testing))
            {
                try
                {
                    return db.CompositeSymbols.Where(c => c.CompName == name).First();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("A exception was thrown when finding CompositeSymbol with name: " + name +
                        "symbol might not exist! check stacktrace");
                    Debug.WriteLine(e.StackTrace);
                    return null;
                }
            }
        }

        /// <summary>
        /// Takes a list of Symbol Models and a name.
        /// Creates a CompositeSymbol by merging components JPEG's
        /// and adding name. Returns null if merging jpegs failed, or
        /// if identical CompositeSymbol exists. The returned CompositeSymbol
        /// should be passed to CompositeSymbolDAL.Insert if the sybol is intended 
        /// to be saved in the database table
        /// </summary>
        /// <param name="CompositeName"></param>
        /// <param name="components"></param>
        /// <returns>BlissBase.Model.CompositeSymbol or null</returns>
        public CompositeSymbol CreateCompositeByComponents(String compositeName,
            List<Symbol> components)
        {
            List<Symbol> sortedList = components.OrderBy(s => s.symId).ToList();

            CompositeSymbol nameExists = GetExactCompositeSymbolByName(compositeName);
            if (nameExists != null)
            {
                CompositeOfDAL composites = new CompositeOfDAL();
                List<Symbol> hasComponents = composites.GetComponentsOf(nameExists);
                if (Enumerable.SequenceEqual(sortedList, hasComponents))
                    return null;
            }
            byte[] newCompJpeg = null;
            newCompJpeg = CombineJpegFromComponents(sortedList);

            CompositeSymbol newCompSymbol = null;
            if (newCompJpeg != null)
            {
                newCompSymbol = new CompositeSymbol()
                {
                    compName = compositeName,
                    compJpeg = newCompJpeg
                };
            }

            return newCompSymbol;
        }

        /// <summary>
        /// Takes a CompositeSymbol Model and a list of Symbol Models.
        /// Inserts composite symbol and links it to the list of Symbols 
        /// using CompositesOf
        /// </summary>
        /// <param name="innCompSymbol"></param>
        /// <param name="components"></param>
        /// <returns>Id of new BlissBase.DAL.CompositeSymbols as int, or -1 if insert failed</returns>
        public int Insert(CompositeSymbol innCompSymbol, List<Symbol> components)
        {
            var newCompSymbol = new CompositeSymbols()
            {
                CompName = innCompSymbol.compName,
                CompJPEG = innCompSymbol.compJpeg
            };

            using (var db = new BlissBaseContext(testing))
            {

                try
                {
                    db.CompositeSymbols.Add(newCompSymbol);
                    db.SaveChanges();

                    CompositeOfDAL compOf = new CompositeOfDAL();
                    compOf.SetCompositeOfRow(newCompSymbol, components);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error Inserting Composite symbol: " + innCompSymbol.compId);
                    Debug.WriteLine(e.StackTrace);
                    return -1;
                }
                return newCompSymbol.CompID;
            }
        }


        /// <summary>
        /// Updates a composite symbol's name by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns>true, false depending on success</returns>
        public bool UpdateCompositeName(int id, String name)
        {
            if (String.IsNullOrEmpty(name))
            {
                return false;
            }
            else
            {
                using (var db = new BlissBaseContext(testing))
                {
                    try
                    {
                        CompositeSymbols toEdit = db.CompositeSymbols.Find(id);
                        toEdit.CompName = name;
                        db.SaveChanges();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("An exception was thrown when updating name for CompID: " + id);
                        Debug.WriteLine(e.StackTrace);
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Updates a composite symbol's jpeg data by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="jpeg"></param>
        /// <returns>true or false depending on success</returns>
        public bool UpdateCompositeJPEG(int id, byte[] jpeg)
        {
            if (jpeg == null)
            {
                return false;
            }
            else
            {
                using (var db = new BlissBaseContext(testing))
                {
                    try
                    {
                        CompositeSymbols toEdit = db.CompositeSymbols.Find(id);
                        toEdit.CompJPEG = jpeg;
                        db.SaveChanges();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("An exception was thrown when updating JPEG for CompID: " + id);
                        Debug.WriteLine(e.StackTrace);
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Deletes a composite symbol and it's rows in
        /// CompositeOf table. Takes a CompositeSymbols 
        /// row as inn parameter
        /// </summary>
        /// <param name="compSymbol"></param>
        /// <returns>true or false depending on success</returns>
        internal bool DeleteCompositeSymbolByRow(CompositeSymbols compSymbol)
        {
            if (compSymbol == null)
            {
                return false;
            }
            else
            {
                using (var db = new BlissBaseContext(testing))
                {
                    try
                    {
                        db.CompositeSymbols.Attach(compSymbol);
                        db.Entry(compSymbol).State = EntityState.Deleted;
                        db.SaveChanges();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Exception when removing symbol: " + compSymbol.CompID);
                        Debug.WriteLine(e.StackTrace);
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Deletes a composite symbol and it's rows in
        /// CompositeOf table. Takes a CompositeSymbol 
        /// model as inn parameter
        /// </summary>
        /// <param name="compModel"></param>
        /// <returns>true or false depending on success</returns>
        public bool DeleteCompositeSymbol(CompositeSymbol compModel)
        {
            if (compModel == null)
                return false;

            CompositeOfDAL compOf = new CompositeOfDAL();
            var ok = compOf.DeleteByCompositeSymbol(compModel);
            if (ok)
            {
                CompositeSymbols compSymbol = GetExactCompositeRowByID(compModel.compId);
                return DeleteCompositeSymbolByRow(compSymbol);
            }
            return false;
        }

        /// <summary>
        /// Creates a new Jpeg that is all the Jpeg's in components combined.
        /// The heght of the jpeg will be equal to the tallest jpeg in components
        /// and the width of the new jpeg will be the total width of the jpegs in 
        /// the List of components
        /// </summary>
        /// <param name="components"></param>
        /// <returns>byte[] or null</returns>
        private byte[] CombineJpegFromComponents(List<Symbol> components)
        {
            /* References used: 
             * http://tech.pro/tutorial/921/combining-images-with-csharp
             * http://stackoverflow.com/questions/3801275/how-to-convert-image-in-byte-array
             */
            List<Bitmap> images = new List<Bitmap>();
            Bitmap finalImage = null;

            try
            {
                int width = 0;
                int height = 0;

                foreach (var sym in components)
                {
                    using (var memStream = new MemoryStream(sym.symJpeg))
                    {
                        Bitmap img = (Bitmap)Image.FromStream(memStream);

                        width += img.Width;
                        height = (img.Height > height) ? img.Height : height;

                        images.Add(img);
                    }
                }

                finalImage = new Bitmap(width, height);

                using (Graphics g = Graphics.FromImage(finalImage))
                {
                    g.Clear(Color.White);

                    int offset = 0;

                    foreach (Bitmap image in images)
                    {
                        g.DrawImage(image,
                            new Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }

                MemoryStream ms = new MemoryStream();
                finalImage.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
            catch (Exception e)
            {
                if (finalImage != null)
                    finalImage.Dispose();

                Debug.WriteLine("There was an Exception when combining component bitmaps");
                Debug.WriteLine(e.StackTrace);
                return null;
            }
            finally
            {
                foreach (var image in images)
                    image.Dispose();
            }
        }
    }
}
