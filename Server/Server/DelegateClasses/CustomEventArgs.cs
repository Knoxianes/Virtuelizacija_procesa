using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DelegateClasses
{
    public class CustomEventArgs
    {
        public  Dictionary<string,double> Results { get; set; }
        public CustomEventArgs(Dictionary<string,double> results)
        {
            Results = results;
        }
    }
}
