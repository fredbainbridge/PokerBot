using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerMavensAPI
{
    /// <summary>  call without any parameters to retrieve a list of dates and table names where a hand history file was created.  </summary>
    public class LogsHandHistory
    {
        /// <summary>  Used to get Result from JSON reply</summary>
        public string Result { get; set; }

        /// <summary>  get the JSON response for Result </summary>
        public string Error { get; set; }

        /// <summary>  parameter in the format of yyyy-mm-dd  </summary>
        public List<string> Data {get; set;}

    }
}
