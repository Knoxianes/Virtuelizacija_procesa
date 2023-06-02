using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class FileHandle : IFileHandle
    {
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public MemoryStream MemoryStream { get; set; }

        public FileHandle(MemoryStream memoryStream, string fileName)
        {
            MemoryStream = memoryStream;
            FileName = fileName;
        }

        public void Dispose()
        {
            if (MemoryStream != null)
            {
                try
                {
                    MemoryStream.Dispose();
                    MemoryStream.Close();
                    MemoryStream = null;
                }
                catch (Exception)
                {
                    Console.WriteLine("Dispose error!!!");
                }
            }
        }
    }
}
