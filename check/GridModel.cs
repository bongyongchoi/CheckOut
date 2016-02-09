using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace check
{
    
    class GridModel
    {
        public string name { get; set; }
        public string ymd { get; set; }

        public TimeSpan hms { get; set; }
        public int termId { get; set; }
        public int roomNum { get; set; }
    }
}
