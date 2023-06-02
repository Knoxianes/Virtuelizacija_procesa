using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [ServiceContract]
    public interface ICalculations
    {
        [OperationContract]
        MemoryStream Calculations(bool min, bool max, bool stand);

        [OperationContract]
        string SaveCsv(MemoryStream csv, out List<Audit> errors);
    }
}
