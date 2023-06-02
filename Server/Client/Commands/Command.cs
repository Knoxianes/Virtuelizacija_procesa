using Common;
using Database;
using Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client.Commands
{
    public class Command : ICommand
    {
        public void GetCommand(bool min = false, bool max = false, bool stand = false)
        {

            ChannelFactory<ICalculations> channelCalc = new ChannelFactory<ICalculations>("Server");
            ICalculations proxy = channelCalc.CreateChannel();

            IFileHandle file = new FileHandle(proxy.Calculations(min, max, stand), GetFileName());
            IReport rep = new SaveReport();
            string created = (rep as SaveReport).CreateCalcFile((file as FileHandle).MemoryStream, (file as FileHandle).FileName);
            if (!created.Equals(string.Empty))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("CLIENT: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(created);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("CLIENT: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Creation unsuccessful");
            }
            file.Dispose();

        }

        public string SendCommand(out List<Audit> errors)
        {
            string ret = "";
            MemoryStream stream = new MemoryStream();
            string csvPath = ConfigurationManager.AppSettings["csv"];
            errors = new List<Audit>();
            ChannelFactory<ICalculations> channelCalc = new ChannelFactory<ICalculations>("Server");
            ICalculations proxy = channelCalc.CreateChannel();

            string[] allFiles = Directory.GetFiles(csvPath);
            if (allFiles.Length == 0)
            {
                throw new FaultException<NoFileException>(new NoFileException("No file available, please add the CSV file."));
                
            }

            using (FileStream csvStream = new FileStream(allFiles[0], FileMode.Open, FileAccess.Read))
            {
                csvStream.CopyTo(stream);
                csvStream.Dispose();
            }

            stream.Position = 0;

            IFileHandle file = new FileHandle(stream, "csvData");          
            ret = proxy.SaveCsv((file as FileHandle).MemoryStream, out errors);
            file.Dispose();

            if (!ret.Equals("Send failed") && File.Exists(allFiles[0]))
            {
                //File.Delete(allFiles[0]);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("INFO: ");
                Console.ForegroundColor= ConsoleColor.White;
                Console.WriteLine("\t" + DateTime.Now  + "\t" + allFiles[0].Split('\\')[allFiles[0].Split('\\').Length - 1] + " successfully deleted.");
            }

            return ret;
        }

        private string GetFileName()
        {
            DateTime now = DateTime.Now;
            string fileName = "Result_" + now.ToString("dd_MM_yyyy___HH_mm_ss");
            return fileName;
        }
    }
}
