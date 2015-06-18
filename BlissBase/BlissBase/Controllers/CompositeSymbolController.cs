using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlissBase.BLL;
using BlissBase.Model;

namespace BlissBase.Controllers
{
    /// <summary>
    /// CompositeSymbolController is used to keep
    /// track of CompositeSymbols. The methods here edit and delete the 
    /// CompositeSymbols, and also return appropriate views.
    /// </summary>
    public class CompositeSymbolController : Controller
    {
        /// <summary>
        /// Generates a list of all the CompositeSymbols and 
        /// saves it in ViewData, and then returns the Index-view.
        /// It also sends an error message to the view if it exists.
        /// </summary>
        /// <param></param>
        /// <returns>Index-view</returns>
        public ActionResult Index()
        {
            CompositeSymbolBLL comp = new CompositeSymbolBLL();
            ViewData["comp"] = comp.GetAll();
            if (TempData["msg"] != null)
            {
                ViewBag.Message = TempData["msg"].ToString();
            }
            return View();
        }

        /// <summary>
        /// Gets the id of the CompositeSymbol you want to edit, 
        /// the symbols it is made of, and a list of all Symbols,
        /// saves them in ViewData and then returns the Edit-view.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Edit-view</returns>
        public ActionResult Edit(int id)
        {
            if (Session["logged_in"] == null)
                return RedirectToAction("Index", "Index");

            CompositeSymbolBLL comp = new CompositeSymbolBLL();
            CompositeSymbol find = comp.GetExaxtComositeSymbolByID(id);

            CompositeOfBLL compOf = new CompositeOfBLL();

            SymbolBLL symb = new SymbolBLL();

            List<Symbol> symbols = compOf.GetComponentsOf(find);

            SymbolTypeBLL types = new SymbolTypeBLL();

            ViewData["comp"] = find;
            ViewData["symbList"] = symbols;
            ViewData["otherSymbols"] = symb.GetAllSymbols();
            ViewData["Types"] = types.GetAllNonStandard();
            return View();
        }

        /// <summary>
        /// This method takes an Int32-array with id's of Symbols, and a compId
        /// representing the recently changed CompositeSymbol.
        /// Redirects you to the Index-method in CompositeSymbolController,
        /// together with a message of error or success.
        /// </summary>
        /// <param name="isPartOf"></param>
        /// <param name="compId"></param>
        /// <returns>Index-view</returns>
        [HttpPost]
        public ActionResult Edit(Int32[] isPartOf, int compId)
        {
            if (Session["logged_in"] == null)
                return RedirectToAction("Index", "Index");

            CompositeSymbolBLL comp = new CompositeSymbolBLL();
            SymbolBLL symbols = new SymbolBLL();

            CompositeSymbol editedComp = comp.GetExaxtComositeSymbolByID(compId);
            List<Symbol> partOf = new List<Symbol>();

            foreach(Int32 part in isPartOf)
            {
                var temp = symbols.GetExactByID(part);
                if(temp != null)
                {
                    partOf.Add(temp);
                }
            }

            CompositeOfBLL compOf = new CompositeOfBLL();
            var ok = compOf.DeleteByCompositeSymbol(editedComp);
            if(ok)
            {
                ok = compOf.SetCompositeOfSymbol(editedComp, partOf);
                if(ok)
                {
                    TempData["msg"] = "Composite Symbol '" + editedComp.compName + "' was successfully edited.";
                }
                else
                {
                    TempData["msg"] = "Error when editing.";
                }
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Takes an id and deletes the corresponding CompositeSymbol.
        /// Returns you to the Index-method of the CompositeSymbolController
        /// together with a message of error or success.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Index-view</returns>
        public ActionResult Delete(int id)
        {
            if (Session["logged_in"] == null)
                return RedirectToAction("Index", "Index");

            CompositeSymbolBLL comp = new CompositeSymbolBLL();
            CompositeSymbol toDelete = comp.GetExaxtComositeSymbolByID(id);

            var name = toDelete.compName;

            if (toDelete != null)
            {
                var ok = comp.DeleteCompositeSymbol(toDelete);
                if (ok)
                {
                    TempData["msg"] = "Composite Symbol '" + name + "' was successfully deleted.";
                }
                else
                {
                    TempData["msg"] = "Could not delete symbol '" + name + "'.";
                }
            }
            else
            {
                TempData["msg"] = "Composite Symbol with id: '" + id + "' was not found.";
            }

            return RedirectToAction("Index");
        }
    }
}