using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    [ServiceContract]
    public interface IReport
    {
        [OperationContract]
        string CreateCalcFile(MemoryStream calcStream, string fileName);
    }
}
