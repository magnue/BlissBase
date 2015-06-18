using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlissBase.BLL;
using BlissBase.Model;
using System.Net.Mail;
using BlissBase.Models;
using System.Text;
namespace BlissBase.Controllers
{
    /// <summary>
    /// The IndexController is used to return the Index-view, 
    /// and also do anything else related to the index-view.
    /// </summary>
    public class IndexController : Controller
    {
        /// <summary>
        /// This method returns the index-view.
        /// </summary>
        /// <returns>Index-view</returns>
        public ActionResult Index()
        {          
            return View();
        }

        public ActionResult SymbolAdministration()
        {
            return View();
        }

        /// <summary>
        /// Saves files selected in form and saves them in "Import" directory
        /// Then calls ImportFromPath() method in RawSymbolsImportBll class to import to db
        /// </summary>
        /// <param name="files"></param>
        /// <returns>View()</returns>
        [HttpPost]
        public ActionResult SymbolAdministration(HttpPostedFileBase[] files)
        {
            /* Refrences used
             * http://www.aspdotnet-pools.com/2014/06/multiple-file-upload-with-aspnet-mvc-c.html
             */
            List<string> extensions = new List<string> { ".jpg", ".jpeg", ".png" };
            int maxFileSize = 51200;
            try
            {
                int i = 0;
                foreach (HttpPostedFileBase file in files)
                {
                    // Save files to Import directory
                    string filename = System.IO.Path.GetFileName(file.FileName).ToLower();
                    // Check extention
                    if (extensions.Contains(Path.GetExtension(filename)) && file.ContentLength < maxFileSize)
                    {
                        file.SaveAs(Server.MapPath("~/Import/" + filename));
                        i++;
                    }
                }
                ViewBag.Message = "There were: " + i + " file(s) uploaded successfully";
            }
            catch(Exception e)
            {
                ViewBag.Message = "Error trying to upload files.";
                Console.WriteLine(e.StackTrace);
            }
            // Import files to db
            RawSymbolImportBLL rawImport = new RawSymbolImportBLL();
            rawImport.ImportFromPath("Import");
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactModels c)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    MailMessage mail = new MailMessage(c.Email, "faraz.german@gmail.com");
                    SmtpClient client = new SmtpClient();
                    client.Port = 465;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Host = "smtp-relay.gmail.com";
                    mail.Subject = "this is a test email.";
                    mail.Body = "this is my test email body";
                    client.Send(mail);
                    return View("Success");
                }
                catch (Exception)
                {
                        return View("Error");
                }
           }
            
            return View();
        }
    }
}