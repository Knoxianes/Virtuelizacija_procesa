using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client.Commands
{
    [ServiceContract]
    public interface ICommand
    {
        [OperationContract]
        string SendCommand(out List<Audit> errors);

        [OperationContract]
        void GetCommand(bool min = false, bool max = false, bool stand = false);
    }
}
