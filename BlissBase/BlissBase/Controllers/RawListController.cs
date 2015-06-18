using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlissBase.Model;
using BlissBase.BLL;

namespace BlissBase.Controllers
{
    /// <summary>
    /// RawListController is used to create RawListImports, convert them to either 
    /// CompositeSymbols or Symbols, delete single RawListImports or all. 
    /// </summary>
    public class RawListController : Controller
    {
        /// <summary>
        /// Returns the Index-view together with the list of
        /// all the RawImportSymbols in the DB. Sends a message aswell, if it exists
        /// </summary>
        /// <returns>~/RawList/Index.cshtml</returns>
        public ActionResult Index()
        {
            RawSymbolImportBLL raw = new RawSymbolImportBLL();
            ViewData["raw"] = raw.GetAll();
            if(TempData["msg"] != null)
            {
                ViewBag.Message = TempData["msg"].ToString();
            }

            return View();
        }

        /// <summary>
        /// Returns the byte-array paramater raw
        /// as a jpeg image file.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns>jpeg image file</returns>
        public ActionResult Image(byte[] raw)
        {
            if (raw != null)
                return File(raw, "image/jpeg");
            else
            {
                Debug.WriteLine("Raw Symbol byte[] was null");
                return null;
            }
        }

        /// <summary>
        /// Takes an int id as parameter and finds the corresponding
        /// RawSymbolImport. Saves it in ViewData["raw"] and returns
        /// the AddAsBaseSymbol-view.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>~/RawList/AddAsBaseSymbol.cshtml</returns>
        public ActionResult AddAsBaseSymbol(int id)
        {
            if (Session["logged_in"] == null)
                return RedirectToAction("Index", "Index");

            RawSymbolImportBLL rawImport = new RawSymbolImportBLL();
            RawSymbolImport toConvert = rawImport.GetExact(id);

            ViewData["raw"] = toConvert;

            return View();
        }

        /// <summary>
        /// Takes an int rawId and converts the corresponding RawSymbolImport to 
        /// a Symbol with the same type as parameter symbolType.
        /// Redirects to Index-view of SymbolController, together with a message.
        /// </summary>
        /// <param name="rawId"></param>
        /// <param name="symbolType"></param>
        /// <returns>~/Symbol/Index.cshtml</returns>
        [HttpPost]
        public ActionResult AddAsBaseSymbol(int rawId, int symbolType)
        {
            if (Session["logged_in"] == null)
                return RedirectToAction("Index", "Index");

            RawSymbolImportBLL rawImport = new RawSymbolImportBLL();
            RawSymbolImport toConvert = rawImport.GetExact(rawId);

            SymbolBLL symbols = new SymbolBLL();
            var returnedId = symbols.Insert(toConvert.rawName, toConvert.rawJpeg);
            if (returnedId != -1)
            {
                TempData["msg"] = "Success";

                RawSymbolImportBLL raws = new RawSymbolImportBLL();
                var ok = raws.DeleteExact(rawId);
                if (!ok)
                {
                    TempData["msg"] += ", but could not delete symbol from raw list.";
                }

                if(symbolType != 0)
                {
                    SymbolTypeBLL symbolTypes = new SymbolTypeBLL();
                    TypeCodes type;
                    
                    if (symbolType == 1)
                    {
                        type = TypeCodes.INDICATOR;
                    }
                    else if (symbolType == 2)
                    {
                        type = TypeCodes.NUMERICAL;
                    }
                    else
                    {
                        type = TypeCodes.LATIN;
                    }

                    symbolTypes.SetLanguageForSymID(returnedId, type);
                }
            }
            else
            {
                TempData["msg"] = "Could not add symbol as base.";
            }

            return RedirectToAction("Index", "Symbol");
        }

        /// <summary>
        /// Takes int id as parameter and finds the corresponding RawSymbolImport.
        /// Saves the RawSymbolImport and a list of all Symbols in Viewdata["raw"] and
        /// Viewdata["symbList"] respectively, and returns the ConvertToComposite-view.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>~/RawList/ConvertToComposite.cshtml</returns>
        public ActionResult ConvertToComposite(int id)
        {
            if (Session["logged_in"] == null)
                return RedirectToAction("Index", "Index");

            RawSymbolImportBLL raw = new RawSymbolImportBLL();
            RawSymbolImport find = raw.GetExact(id);

            SymbolBLL symb = new SymbolBLL();

            SymbolTypeBLL types = new SymbolTypeBLL();

            ViewData["raw"] = find;
            ViewData["symbList"] = symb.GetAllSymbols();
            ViewData["types"] = types.GetAllNonStandard();
            return View();
        }

        /// <summary>
        /// Converts the RawSymbolImport with id = rawId to a CompositeSymbol 
        /// made of Symbols with id's stored in the isPartOf-array.
        /// Redirects to the Index-method in the CompositeSymbolController,
        /// together with a message.
        /// </summary>
        /// <param name="isPartOf"></param>
        /// <param name="rawId"></param>
        /// <returns>~/CompositeSymbol/Index.cshtml</returns>
        [HttpPost]
        public ActionResult ConvertToComposite(Int32[] isPartOf, int rawId)
        {
            if (Session["logged_in"] == null)
                return RedirectToAction("Index", "Index");

            RawSymbolImportBLL raw = new RawSymbolImportBLL();
            SymbolBLL symbols = new SymbolBLL();
            CompositeSymbolBLL comp = new CompositeSymbolBLL();

            RawSymbolImport rawBecomingComp = raw.GetExact(rawId);
            List<Symbol> partOf = new List<Symbol>();

            foreach (Int32 part in isPartOf)
            {
                var temp = symbols.GetExactByID(part);
                if (temp != null)
                {
                    partOf.Add(temp);
                }
            }

            CompositeSymbol compOfRaw = new CompositeSymbol()
            {
                compId = rawBecomingComp.rawId,
                compName = rawBecomingComp.rawName,
                compJpeg = rawBecomingComp.rawJpeg
            };

            var ok = comp.Insert(compOfRaw, partOf);
            if (ok != -1)
            {
                TempData["msg"] = "Raw symbol '" + rawBecomingComp.rawName + "' is now a composite symbol";
                var successfullyDeleted = raw.DeleteExact(rawId);
                if(!successfullyDeleted)
                {
                    TempData["msg"] += ", but was not deleted from raw list.";
                }
            }
            return RedirectToAction("Index", "CompositeSymbol");
        }

        /// <summary>
        /// Deletes the RawSymbolImport with id as parameter id.
        /// Redirects to the Index-method in RawListController together with
        /// a message of error or success.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>~/RawList/Index.cshtml</returns>
        public ActionResult Delete(int id)
        {
            if (Session["logged_in"] == null)
                return RedirectToAction("Index", "Index");

            RawSymbolImportBLL raw = new RawSymbolImportBLL();
            RawSymbolImport toDelete = raw.GetExact(id);
            
            var name = toDelete.rawName;

            if(toDelete != null)
            {
                var ok = raw.DeleteExact(id);
                if (ok)
                {
                    TempData["msg"] = "Raw Symbol '" + name + "' was successfully deleted.";
                }
                else
                {
                    TempData["msg"] = "Could not delete symbol '" + name + "'.";
                }
            }
            else
            {
                TempData["msg"] = "Raw Symbol with id: '" + id + "' was not found.";
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Deletes all RawSymbolImports from the database.
        /// Redirects to Index-method in RawListController, together
        /// with a message of error or success.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteAll()
        {
            if (Session["logged_in"] == null)
                return RedirectToAction("Index", "Index");

            RawSymbolImportBLL rawImport = new RawSymbolImportBLL();
            TempData["msg"] = (rawImport.DeleteAll()) ? "All raw symbols deleted" : "Error trying to delete files.";
            return RedirectToAction("Index");
        }
    }
}