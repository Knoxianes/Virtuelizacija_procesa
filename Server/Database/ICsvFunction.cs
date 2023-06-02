using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    [ServiceContract]
    public interface ICsvFunction
    {
        [OperationContract]
        string ParseFile(MemoryStream csv, out List<Audit> greske);

        [OperationContract]
        IFileHandle OpenFile(string path);
    }
}
