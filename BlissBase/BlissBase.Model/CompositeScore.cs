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
    public class CompositeScore
    {
        public int compId { get; set; }
        public int compScore { get; set; }
    }
}
