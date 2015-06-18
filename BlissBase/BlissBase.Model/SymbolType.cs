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
    /// Symbol type enum
    /// Used by onscreen keeyboard to group base symbols
    /// </summary>
    public enum TypeCodes
    {
        INDICATOR = 1,
        NUMERICAL = 2,
        LATIN = 3
    }

    public class SymbolType
    {
        public int symId { get; set; }
        public TypeCodes typeIndicator { get; set; }
    }
}