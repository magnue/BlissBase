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
    public class RawSymbolImport
    {
        public int rawId { get; set; }
        public string rawName { get; set; }
        public byte[] rawJpeg { get; set; }
    }
}
