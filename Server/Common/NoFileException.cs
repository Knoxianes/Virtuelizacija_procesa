using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    [DataContract]
    public class NoFileException
    {
            [DataMember]
            public string Message { get; set; }

            public NoFileException(string message)
            {
                Message = message;
            }
    }
}
