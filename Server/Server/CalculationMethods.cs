using Common;
using Database;
using Server.DelegateClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class CalculationMethods : CustomEventBase
    {
        List<Load> data = new List<Load>();
        public void ReadFromDataBase()
        {
            try
            {
            ChannelFactory<IDatabase> database_factory = new ChannelFactory<IDatabase>("Database");
            IDatabase database_channel = database_factory.CreateChannel();
            database_channel.ReadFromBase(out data);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void CalculateMin(object sender, CustomEventArgs a)
        {
            ReadFromDataBase();
            a.Results.Add("Min Load",data.Min(load=>load.MeasuredValue1));
        }

        public void CalculateMax(object sender, CustomEventArgs a)
        {
            ReadFromDataBase();
            a.Results.Add("Max Load",data.Max(load => load.MeasuredValue1));
        }

        public void CalculateDev(object sender, CustomEventArgs a)
        {
            ReadFromDataBase();
            var all_mesurments = data.Select(load => load.MeasuredValue1);
            double sumOfSquares = 0;
            double mean = all_mesurments.Average();

            foreach (double tmp in all_mesurments)
            {
                double deviation = tmp - mean;
                sumOfSquares += deviation*deviation;
            }
            double variance = sumOfSquares / all_mesurments.Count();

            a.Results.Add("Standard deviation",Math.Round(Math.Sqrt(variance),5));
        }
    }
}
