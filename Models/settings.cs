using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbionOnlineCraftingCalculator
{


    public class Setting
    {
        public List< UserSpecInput> UserSpecInput  { get; set; }  
    }




    public class UserSpecInput
    {

 

        public string Name { get; set; }

        public List<int> Spec { get; set; } = new List<int>(); // Most of the time 7 items. Royal doesnt have a spec.
    }
}
