using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DelegateClasses
{
    public class CustomEventBase
    {
        public delegate void CustomEventHandler(object sender, CustomEventArgs args);

        public event CustomEventHandler CustomEvent;

        public void RaiseEvent(Dictionary<string,double> results)
        {
            CustomEvent?.Invoke(this, new CustomEventArgs(results));
        }
    }
}
