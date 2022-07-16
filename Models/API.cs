using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbionOnlineCraftingCalculator.Models
{
    public class RegisterModel
    {
        public string Username { get; set; }

        public string Password { get; set;   }
    }


    public class RegisterReturn
    {
        public string Username { get; set; }
        public string passwordHash { get; set; }

        public string passwordSalt { get; set; }
    }

    public class LoginReturn
    {
        public string JWTToken { get; set; }
    }
}
