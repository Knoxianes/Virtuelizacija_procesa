using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    [ServiceContract]
    public interface IDatabase
    {
        [OperationContract]
        int WriteIntoBase(List<Load> data, List<Audit> errors);

        [OperationContract]
        bool ReadFromBase(out List<Load> data);
    }
}
