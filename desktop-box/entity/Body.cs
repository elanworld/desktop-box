using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop_box.entity
{
    public class Body
    {
        public int? X { get; set; } 
        public int? Y { get; set; }    
        public int? X1 { get; set; } 
        public int? Y1 { get; set; }


        public Double duration { get; set; }    

        public Double? transparency { get; set; }

        public String showPath { get; set; }
        public int? delay { get; set; }
        public int? width { get; set; }
    }
}
