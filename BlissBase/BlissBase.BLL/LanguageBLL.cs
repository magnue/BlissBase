// BLL "Business Logic Layer"
// All the classes in the BLL layer should handle the application's
// use of the databaselayer "DAL". The BLL layer should only be accessed 
// from the BlissBase "web application" and not the DAL or Models layer.
// The BLL layer should be the only connection down to the DAL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlissBase.DAL;
using BlissBase.Model;

namespace BlissBase.BLL
{
    public class LanguageBLL
    {
        bool testing;
        public LanguageBLL()
        {
            testing = false;
        }
        public LanguageBLL(bool testing_)
        {
            testing = testing_;
        }
        public List<Language> GetAllNonInternational()
        {
            var LanguageDAL = new LanguageDAL();
            return LanguageDAL.GetAllNonInternational();
        }
        public Language GetExactByCompID(int compId)
        {
            var LanguageDAL = new LanguageDAL(testing);
            return LanguageDAL.GetExactByCompID(compId);
        }
        public bool UpdateLanguageForCompID(int compId, LanguageCodes code)
        {
            var LanguageDAL = new LanguageDAL(testing);
            return LanguageDAL.UpdateLanguageForCompID(compId, code);
        }
        public bool SetLanguageForCompID(int compId, LanguageCodes code)
        {
            var LanguageDAL = new LanguageDAL(testing);
            return LanguageDAL.SetLanguageForCompID(compId, code);
        }
        public bool SetInternationalForCompID(int compId)
        {
            var LanguageDAL = new LanguageDAL(testing);
            return LanguageDAL.SetInternationalForCompID(compId);
        }
    }
}
