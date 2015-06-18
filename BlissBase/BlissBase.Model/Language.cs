// Model 
// All classes in Model should be accessable from all other .dll's
// Model should not have access to any of the other layers

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlissBase.Model
{
    /// <summary>
    /// Language Enum.
    /// Use ISO 639-1 Language Codes
    /// http://www.w3schools.com/tags/ref_language_codes.asp
    /// </summary>
    public enum LanguageCodes
    {
        NO = 1,
        SV = 2,
        EN = 3
    }

    public class Language
    {
        public int compId { get; set; }
        public LanguageCodes langCode { get; set; }
    }
}
