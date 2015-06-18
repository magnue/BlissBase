// DAL "Database Access Layer"
// All classes in DAL should be only access to DB
// All classes in DAL should only be accessibele from BLL

using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlissBase.Model;

namespace BlissBase.DAL
{
    /// <summary>
    /// RawSymbolImportDAL
    /// Creates  access to the table that handles import of raw data.
    /// The data in this table should only contain raw JPEG/PNG data in
    /// the RawJPEG field, and the RawName should be the filename of
    /// the raw file. Intended as a ImportAll to db, to import files.
    /// All images is resized to a height of 64 pixels with aspect ratio
    /// conserved. For convenience transparrent PNG's with a height of
    /// 64 px or more should be used
    /// </summary>
    public class RawSymbolImportDAL
    {
        const int dbImageHeight = 64;
        bool testing;
        public RawSymbolImportDAL()
        {
            testing = false;
        }
        public RawSymbolImportDAL(bool testing_)
        {
            testing = testing_;
        }

        /// <summary>
        /// Accepts a path variable and import all jpeg files in the directory.
        /// rawName == the filename of the image rawJpeg == imagedata as byte[]
        /// Successfully imported images is moved to Imported folder, with
        /// Day, Month, Year appended to the path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true or false depending on success</returns>
        public bool ImportFromPath(String path)
        {

            using (var db = new BlissBaseContext(testing))
            {
                try
                {
                    string[] directory = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + path);
                    foreach (string fullPath in directory)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(fullPath);

                        byte[] fullSizeRawImage = File.ReadAllBytes(fullPath);
                        byte[] imageData = ResizeImageAsBytes(fullSizeRawImage, dbImageHeight);

                        if (imageData != null)
                        {
                            db.RawSymbolsImport.Add(new RawSymbolsImport
                            {
                                RawJPEG = imageData,
                                RawName = fileName
                            });
                            db.SaveChanges();
                        }

                        // Move file to imported + /yyyy/mm/dd/ and create directory if not exists
                        string targetPath = AppDomain.CurrentDomain.BaseDirectory + path + "/Imported/" + DateTime.Now.ToString("yyyy\\/MM\\/dd");
                        String targetFile = Path.Combine(targetPath, Path.GetFileNameWithoutExtension(fullPath));
                        if (!Directory.Exists(targetPath))
                            Directory.CreateDirectory(targetPath);

                        File.Copy(fullPath, targetFile, true);
                        File.Delete(fullPath);
                    }
                }
                catch (DirectoryNotFoundException dnfe)
                {
                    Debug.WriteLine("Directory not found when importing imagefiles to db");
                    Debug.WriteLine(dnfe.ToString());
                    return false;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("There was an exception when trying to import imagefiles to db");
                    Debug.WriteLine(e.ToString());
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns a RawSymbolImport "Model" list
        // of all imported symbols, or null if none
        // exists.
        /// </summary>
        /// <returns>List of BlissBase.Model.RawSymbolImport or null</returns>
        public List<RawSymbolImport> GetAll()
        {
            using (var db = new BlissBaseContext(testing))
            {
                List<RawSymbolsImport> rawSymbolRows = null;
                try
                {
                    // queries all RawSymbolImport rows to list
                    rawSymbolRows = db.RawSymbolsImport.ToList();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception conwerting RawSympolsImport table to list");
                    Debug.WriteLine(e.StackTrace);
                    return null;
                }

                if (rawSymbolRows == null)
                    return null;
                // Converts list of rows to list of models
                List<RawSymbolImport> rawSymbolModels = new List<RawSymbolImport>();
                foreach (RawSymbolsImport t in rawSymbolRows)
                {
                    rawSymbolModels.Add(
                    new RawSymbolImport()
                    {
                        rawId = t.RawID,
                        rawName = t.RawName,
                        rawJpeg = t.RawJPEG
                    });
                }
                return rawSymbolModels;
            }
        }

        /// <summary>
        /// Returns an exact symbol by RawID if the 
        /// symbol exists. if not, null will be returned
        /// </summary>
        /// <param name="id"></param>
        /// <returns>BlissBase.Model.RawSymbolImport or null</returns>
        public RawSymbolImport GetExact(int id)
        {
            using (var db = new BlissBaseContext(testing))
            {
                RawSymbolsImport rawSymbolRow = null;
                try
                {
                    rawSymbolRow = db.RawSymbolsImport.First(e => e.RawID == id);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception querying RawSymbol by id");
                    Debug.WriteLine(e.StackTrace);
                    return null;
                }
                if (rawSymbolRow == null)
                    return null;

                return new RawSymbolImport
                {
                    rawId = rawSymbolRow.RawID,
                    rawName = rawSymbolRow.RawName,
                    rawJpeg = rawSymbolRow.RawJPEG
                };
            }
        }

        /// <summary>
        /// Deletes all the symbols in the RawSymbolsImport table
        /// </summary>
        /// <returns>true or false depending on success</returns>
        public bool DeleteAll()
        {
            using (var db = new BlissBaseContext(testing))
            {
                try
                {
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE [RawSymbolsImport]");
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception trying to TRUNCATE table RawSymbolsImport");
                    Debug.WriteLine(e.StackTrace);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// When a sybol is moved to the Symbols or CompositeSymbols
        /// table, the raw version should be deleted. This method
        /// deletes RawSymbol by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true or false depending on success</returns>
        public bool DeleteExact(int id)
        {
            using (var db = new BlissBaseContext(testing))
            {
                try
                {
                    RawSymbolsImport symbolToDelete = db.RawSymbolsImport.First(e => e.RawID == id);
                    if (symbolToDelete != null)
                    {
                        db.RawSymbolsImport.Remove(symbolToDelete);
                        db.SaveChanges();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Could not find and delete RawSymbol with id: " + id);
                    Debug.WriteLine(e.StackTrace);
                }
            }
            return false;
        }

        /// <summary>
        /// Reads byte[] fullSize to MemoryStream, then reads the stream to a 
        /// System.Drawing.Image object 'fullSizeImage'. Then a new Image object 
        /// newImage is set to a thumbnail of fullSizeImage with a height of 64px,
        /// maintaining aspect ratio, and then saved to a MemoryStream
        /// The memory stream's .ToArray is returned if te stream is not null.
        /// </summary>
        /// <param name="fullSize"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>Image as byte[] or null</returns>
        private byte[] ResizeImageAsBytes(byte[] fullSize, int newHeight)
        {
            /* Reference used: http://stackoverflow.com/a/8790466
             * http://www.stev.org/post/2011/03/01/C-Resize-an-image-by-size-of-height.aspx
             */
            // Read image from stream
            MemoryStream memStream = new MemoryStream(fullSize);
            Image fullSizeImage = Image.FromStream(memStream);

            // Get scaling ratio and set width and height
            float ratio = ((float)fullSizeImage.Height / (float)newHeight);
            int width = Convert.ToInt32(fullSizeImage.Width / ratio);
            int height = Convert.ToInt32(fullSizeImage.Height / ratio);

            // Get scaled thumbnail image and save to stream
            Image newImage = fullSizeImage.GetThumbnailImage(width, height, null, IntPtr.Zero);
            MemoryStream result = new MemoryStream();
            newImage.Save(result, System.Drawing.Imaging.ImageFormat.Png);

            // Return stream .ToArray
            return (result != null) ? result.ToArray() : null;
        }
    }
}
