using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BussPassAPI.Models
{
    public class _User
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AadharNo { get; set; }
        public string MobileNo { get; set; }
        public string Passcode { get; set; }
    }
}