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
    public class UserSynonym
    {
        public int uSynId { get; set; }
        public int uSynWord { get; set; }
        public int uSynSynonym { get; set; }
        public int uListUserId { get; set; }
        public bool uSynApproved { get; set; }
    }
}
