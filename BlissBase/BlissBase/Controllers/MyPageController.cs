using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlissBase.Controllers
{
    /// <summary>
    /// MyPageController is used to return views-related to the Users accounts.
    /// </summary>
    public class MyPageController : Controller
    {
        /// <summary>
        /// Returns the index-view together with a message
        /// to the user if they just logged in.
        /// </summary>
        /// <returns>~/MyPage/Index.cshtml</returns>
        public ActionResult Index()
        {
            if (TempData["success"] != null)
            {
                ViewBag.Message = TempData["success"].ToString();
            } 
            
            if (Session["logged_in"] == null && TempData["success"] == null)
                 return RedirectToAction("Index", "Index");
            else
                return View();
        }


    }
}