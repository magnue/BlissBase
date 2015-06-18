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
    /// The WriteController is used to return views and methods
    /// that allow the user to write with Symbols and CompositeSymbols.
    /// </summary>
    public class WriteController : Controller
    {
        /// <summary>
        /// Stores all Symbols in ViewData["symbols"],
        /// all symbols with symboltype other than normal in ViewDate["types"],
        /// and all CompositeSymbols in ViewData["comp"], 
        /// and returns the index-view of WriteController.
        /// </summary>
        /// <returns>~/Write/Inex.cshtml</returns>
        public ActionResult Index()
        {
            SymbolBLL symbols = new SymbolBLL();
            ViewData["symbols"] = symbols.GetAllSymbols();

            SymbolTypeBLL types = new SymbolTypeBLL();
            ViewData["types"] = types.GetAllNonStandard();

            CompositeSymbolBLL comp = new CompositeSymbolBLL();
            ViewData["comp"] = comp.GetAll();

            CompositeOfBLL compOf = new CompositeOfBLL();
            ViewData["compOf"] = compOf.getAllRelations();

            return View();
        }

        /// <summary>
        /// Takes an instance of SuggestionsModel called myModel as paramater.
        /// Uses myModel.symbolId to find a Symbol with that id,
        /// and then create a list of CompositeSymbols called suggestions.
        /// These suggestions include all CompositeSymbols containing the
        /// Symbol with id myModel.symbolId.
        /// If myModel.currentSuggestions already exists, 
        /// the method will filter that list with the new myModel.symbolId.
        /// </summary>
        /// <param name="myModel"></param>
        /// <returns>Json of List of CompositeSymbols</returns>
        [HttpPost]
        public ActionResult Suggestions(SuggestionsModel myModel)
        {
            var symbolId = myModel.symbolId;
            var currentSuggestions = myModel.currentSuggestions;
            var first = myModel.first;

            System.Diagnostics.Debug.WriteLine(first + " AND ID IS " + symbolId + " SIR");
            var listOfComp = new List<CompositeSymbol>();
            if(first == "true")
            {
                CompositeSymbolBLL comp = new CompositeSymbolBLL();
                listOfComp = comp.GetAll();
                System.Diagnostics.Debug.WriteLine("FILLED LIST WITH ALL COMP SYMBOLS FROM BLL SIR");
            }
            else
            {
                foreach(CompositeSymbol co in currentSuggestions)
                {
                    CompositeSymbolBLL comp = new CompositeSymbolBLL();
                    listOfComp.Add(comp.GetExaxtComositeSymbolByID(co.compId));
                }
                //listOfComp = currentSuggestions;
                System.Diagnostics.Debug.WriteLine("NUMBER IS: " + currentSuggestions.Count + ", FILLING LIST WITH LIST IN PARAMETER SIR");
            }
            
            if(listOfComp.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("VERY EMPTY THIS LIST SIR");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("LIST HAS VALUES SIR");
            }

            
            CompositeOfBLL compOf = new CompositeOfBLL();
            CompositeScoreBLL score = new CompositeScoreBLL();
            List<CompositeSymbol> suggestions = new List<CompositeSymbol>();

            var lastScore = 0;
            foreach(CompositeSymbol c in listOfComp)
            {
                if (c != null)
                    System.Diagnostics.Debug.WriteLine("COMP SYMBOL DOES EXCIST SIR");
                else
                    System.Diagnostics.Debug.WriteLine("COMP SYMBOL DOES NOT EXIST SIR");

                var temp = compOf.GetComponentsOf(c);
                foreach(Symbol s in temp)
                {
                    if(s.symId == symbolId)
                    {
                        if(score.GetScoreFor(c.compId) >= lastScore)
                        {
                            suggestions.Insert(0, c);
                            lastScore = score.GetScoreFor(c.compId);
                        }
                        else
                            suggestions.Add(c);
                    }
                }
            }

            return Json(suggestions);
        }

        /// <summary>
        /// Increases the score of the CompositeSymbol
        /// with id as parameter id.
        /// Returns an int with the new score.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>int with new score</returns>
        [HttpPost]
        public int IncreaseCompSymbolScore(int id)
        {
            CompositeScoreBLL compScore = new CompositeScoreBLL();

            return compScore.UpdateScoreFor(id, 1);
        }
    }
}