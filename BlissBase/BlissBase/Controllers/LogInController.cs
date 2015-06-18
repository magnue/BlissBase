using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using Insya.Localization;
using BlissBase.BLL;
using BlissBase.Model;

namespace BlissBase.Controllers
{
    /// <summary>
    /// The LogInController returns views and runs methods related
    /// to the login-process.
    /// </summary>
    public class LogInController : Controller
    {
        /// <summary>
        /// This method returns the index-view containing the login-form.
        /// </summary>
        /// <returns>../Views/Login/Index.cshtml</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// This method takes the paramteres username and password from a HTTP-Post.
        /// The paramteres are used to retrieve the user from database by using the
        /// LogInnUser(passwrd, username)-method from UserDAL.
        /// It then creates a Session called logged_in, containing the returned user, aswell as a 
        /// messsage of error or success. Redirects you to the Index-method of MyPageController if 
        /// successfull, or the Index-method in the IndexController if failed. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>~/MyPage/Index.cshtml or ~/Index/Index.cshtml</returns>
        [HttpPost]
        public ActionResult Index(string username, string password)
        {
            UserBLL users = new UserBLL();

            var user = users.LogInnUser(password, username);
            if(user == null)
            {
                TempData["error"] = Localization.Get("errorLogIn");
                return RedirectToAction("Index", "Index");
            }
            else
            {
                TempData["success"] = Localization.Get("welcomeBack") + user.username;

                Session["logged_in"] = user;

                return RedirectToAction("Index", "MyPage");
            }
        }

        /// <summary>
        /// Logs out currently logged in user by deleting the session and retruning the user to 
        /// the IndexControllers Index-Method.
        /// </summary>
        /// <returns>~/Index/Index.cshtml</returns>
        public ActionResult LogOut()
        {
            Session.Remove("logged_in");
            if(Session["logged_in"] == null)
                Debug.WriteLine("Deleted");
            return RedirectToAction("Index", "Index");
        }

        /// <summary>
        /// Returns the subscribe-view.
        /// Returns appropriate errors to the view if the exist. 
        /// </summary>
        /// <returns>~/Login/Subscribe.cshtml</returns>
        public ActionResult Subscribe()
        {
            if (TempData["username_error"] != null)
            {
                ViewData["username_error"] = TempData["username_error"].ToString();
            }

            if (TempData["password_error"] != null)
            {
                ViewData["password_error"] = TempData["password_error"].ToString();
            }

            return View();
        }

        /// <summary>
        /// This method gets the POST'ed values from the subscribe-view,
        /// and creates an instance of SubscribeViewModel. It then
        /// validates the fields in the model and then creates a User in 
        /// the database, and creates a Session where the user is logged in.
        /// Returns ~/MyPage/Index.cshtml or ~/Login/Subscribe.cshtml with error-messages.
        /// </summary>
        /// <param name="subscribe"></param>
        /// <returns>~/MyPage/Index.cshtml or ~/Login/Subscribe.cshtml</returns>
        [HttpPost]
        public ActionResult Subscribe(SubscribeViewModel subscribe)
        {
            var firstName = subscribe.firstname;
            var lastName = subscribe.lastname;
            var username = subscribe.username;
            var password = subscribe.password;
            var confirmPassword = subscribe.confirm_password;

            var users = new UserBLL();
            var ok = true;

            //does username exist?
            if(users.CheckIfUsernameExists(username))
            {
                TempData["username_error"] = Localization.Get("errorUserName");
                ok = false;
                Debug.WriteLine("username error");
            }

            //are the password-fields identical?
            if(password != confirmPassword)
            {
                TempData["password_error"] = Localization.Get("errorPasswd");
                ok = false;
                Debug.WriteLine("password error");
            }

            if(ok)
            {
                User newUser = new User
                {
                    userFirstName = firstName,
                    userLastName = lastName,
                    username = username,
                    userPasswd = password
                };

                ok = users.InsertUser(newUser);
                if(ok)
                {
                    Debug.WriteLine("it worked");
                    TempData["success"] = Localization.Get("successSignUp") + username
                        + Localization.Get("approval");
                    
                    Session["logged_in"] = null;

                    return RedirectToAction("Index", "MyPage");
                }
            }
            Debug.WriteLine("didnt work too bad");
            return RedirectToAction("Subscribe");
        }
    }
}