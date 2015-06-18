using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlissBase.Model;
using BlissBase.BLL;

namespace BlissBase.Controllers
{
    /// <summary>
    /// The SymbolController is used to return views
    /// related to Symbols, be it Editing, deleting or viewing.
    /// </summary>
    public class SymbolController : Controller
    {
        /// <summary>
        /// Returns the index-view together with a list of all symbols
        /// stored in ViewData["symb"], and a message if it exists.
        /// </summary>
        /// <returns>~/Symbol/Index.cshtml</returns>
        public ActionResult Index()
        {
            SymbolBLL symb = new SymbolBLL();
            SymbolTypeBLL types = new SymbolTypeBLL();
            ViewData["types"] = types.GetAllNonStandard();
            ViewData["symb"] = symb.GetAllSymbols();
            if (TempData["msg"] != null)
            {
                ViewBag.Message = TempData["msg"].ToString();
            }
            return View();
        }

        /// <summary>
        /// Finds the Symbol with the same id as the parameter id,
        /// and stores it in ViewData["symb"].
        /// Returns the Edit-view.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>~/Symbol/Edit.cshtml</returns>
        public ActionResult Edit(int id)
        {
            if (Session["logged_in"] == null)
                return RedirectToAction("Index", "Index");

            if(id != 0)
            {
                SymbolBLL symbols = new SymbolBLL();
                Symbol toEdit = symbols.GetExactByID(id);


                if(toEdit == null)
                {
                    TempData["msg"] = "Could not find symbol with id " + id;
                }
                else
                {
                    SymbolTypeBLL types = new SymbolTypeBLL();
                    try
                    {
                        ViewData["symbol"] = toEdit;
                        TypeCodes type = types.GetExactBySymID(id).typeIndicator;
                        ViewData["type"] = type;
                        return View();
                    }
                    catch(NullReferenceException)
                    {
                        ViewData["type"] = null;
                        return View();
                    }
                    
                }
            }
            else
            {
                TempData["msg"] = "Invalid Id.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(int symbolId, int symbolType)
        {
            if (Session["logged_in"] == null)
                return RedirectToAction("Index", "Index");

            if(symbolId != 0)
            {
                SymbolBLL symbols = new SymbolBLL();
                Symbol toEdit = symbols.GetExactByID(symbolId);

                if(toEdit != null)
                {
                    if(symbolType == 0)
                    {
                        SymbolTypeBLL types = new SymbolTypeBLL();
                        types.SetStandardForSymID(toEdit.symId);
                    }
                    else
                    {
                        TypeCodes type = new TypeCodes();

                        switch (symbolType)
                        {
                            case 1: type = TypeCodes.INDICATOR;
                                break;
                            case 2: type = TypeCodes.NUMERICAL;
                                break;
                            case 3: type = TypeCodes.LATIN;
                                break;
                        }

                        SymbolTypeBLL types = new SymbolTypeBLL();
                        if(types.GetExactBySymID(toEdit.symId) != null)
                        {
                            types.UpdateTypeForSymID(toEdit.symId, type);
                        }
                        else
                        {
                            types.SetLanguageForSymID(toEdit.symId, type);
                        }
                    }
                    TempData["msg"] = "Symbol with id " + symbolId + " was successfully edited!";
                }
                else
                {
                    TempData["msg"] = "Could find and edit symbol with id " + symbolId;
                }
            }
            else
            {
                TempData["msg"] = "SymbolId is not valid.";
            }
            return RedirectToAction("Index");

        }

        /// <summary>
        /// Deletes the Symbol with the same id as parameter id.
        /// Redirects to the index-method of SymbolController,
        /// together with a message of error or success.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>~/Symbol/Index.cshtml</returns>
        public ActionResult Delete(int id)
        {
            if (Session["logged_in"] == null)
                return RedirectToAction("Index", "Index");

            SymbolBLL symb = new SymbolBLL();
            Symbol toDelete = symb.GetExactByID(id);

            var name = toDelete.symName;

            if (toDelete != null)
            {
                var ok = symb.DeleteExact(id);
                if (ok)
                {
                    TempData["msg"] = "Symbol '" + name + "' was successfully deleted.";
                }
                else
                {
                    TempData["msg"] = "Could not delete symbol '" + name + "'.";
                }
            }
            else
            {
                TempData["msg"] = "Symbol with id: '" + id + "' was not found.";
            }

            return RedirectToAction("Index");
        }

    }
}