using System;
using System.Collections.Generic;
// Model 
// All classes in Model should be accessable from all other .dll's
// Model should not have access to any of the other layers

/* Comented using statements gives compiler error
 * using System;
 * using System.Collections.Generic;*/
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlissBase.Model
{
    public class Symbol
    {
        public int symId { get; set; }
        public String symName { get; set; }
        public byte[] symJpeg { get; set; }
    }
}
