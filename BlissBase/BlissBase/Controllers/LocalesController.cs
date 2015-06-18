using System;
using System.Web;
using System.Web.Mvc;
using Insya.Localization;

namespace BlissBase.Controllers
{
    /// <summary>
    /// This class manages the CacheLang cookie to
    /// set and update the current locale
    /// </summary>
    public class LocalesController : Controller
    {
        /// <summary>
        /// This method changes the website language by updating 
        /// the cookie ChacheLang, and redirects to the referring page. 
        /// Default en_US.
        /// </summary>
        /// <param name="lang"></param>
        /// <returns>Index-view</returns>
        public ActionResult NewLocale(string lang = "en_US")
        {
            HttpCookie cookie = Request.Cookies.Get("CacheLang");
            HttpCookie newCookie;
            if (cookie == null)
                newCookie = new HttpCookie("CacheLang");
            else
                newCookie = cookie;

            newCookie.Value = lang;
            Response.SetCookie(newCookie);

            if (Request.UrlReferrer != null)
                Response.Redirect(Request.UrlReferrer.ToString());
             
			var message = Localization.Get("changedlng");
    
			return Content(message);
        }

        /// <summary>
        /// This method creates a new cookie CacheLang and sets
        /// default language en_US without redirecting.
        /// Does nothing if the cookie allready exists
        /// </summary>
        public void DefaultLocale( )
        {
            HttpCookie cookie = Request.Cookies.Get("CacheLang");
            if ( cookie == null)
            {
                HttpCookie newCookie = new HttpCookie("CacheLang");
                newCookie.Value = "en_US";
                Response.Cookies.Add(newCookie);
            }
        }
    }
}