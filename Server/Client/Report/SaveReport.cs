using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class SaveReport : IReport
    {
        public string CreateCalcFile(MemoryStream calcStream, string fileName)
        {
            string txtPath = ConfigurationManager.AppSettings["txt"] + fileName + ".txt";
            try
            {
                using(FileStream stream = new FileStream(txtPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        using (StreamReader txtStream = new StreamReader(calcStream))
                        {
                            string txtData = txtStream.ReadToEnd();
                            string[] txtRows = txtData.Split('\n');
                            string[] rows = txtRows.Take(txtRows.Length - 1).ToArray();
                            foreach(var row in rows)
                                writer.Write(row);
                        }
                    }
                }
                return "Successfully created \n\tFilename:" + fileName + "\n\tFilepath: " + Directory.GetCurrentDirectory() + "\\" + txtPath;
            }
            catch
            {
                return "";
            }

        }
    }
}
