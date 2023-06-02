using Common;
using Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Service : ICalculations
    {
        CalculationMethods obj = new CalculationMethods();
        [OperationBehavior(AutoDisposeParameters =true)]
        public MemoryStream Calculations(bool min, bool max, bool stand)
        {
            if (min)
                obj.CustomEvent += obj.CalculateMin;
            if (max)
                obj.CustomEvent += obj.CalculateMax;
            if (stand)
                obj.CustomEvent += obj.CalculateDev;

            var results = new Dictionary<string,double>();
            obj.RaiseEvent(results);
            
            string fileName = GetFileName();
            string filePath = ConfigurationManager.AppSettings["report"] + fileName;

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach(var key in results.Keys)
                {
                    writer.WriteLine(String.Format("{0}: {1}", key, results[key]));
                }
            }

            MemoryStream memoryStream = new MemoryStream();
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fileStream.CopyTo(memoryStream);
            }

            memoryStream.Position = 0;
            string[] allFiles = Directory.GetFiles(ConfigurationManager.AppSettings["report"]);
            if (File.Exists(allFiles[1]))
            {
                File.Delete(allFiles[1]);
            }
            return memoryStream;
        }

        public string SaveCsv(MemoryStream csv, out List<Audit> errors)
        {
            string ret = "";
            ChannelFactory<ICsvFunction> database_factory = new ChannelFactory<ICsvFunction>("CsvParser");
            ICsvFunction database_channel = database_factory.CreateChannel();
            //errors = new List<Audit>();

            ret = database_channel.ParseFile(csv, out errors);
            return ret;
        }

        private string GetFileName()
        {
            DateTime now = DateTime.Now;
            string fileName = "Result_" + now.ToString("dd_MM_yyyy___HH_mm_ss") + ".txt";
            return fileName;
        }
    }
}
