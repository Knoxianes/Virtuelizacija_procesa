using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class Load
    {
        private int Id;
        private DateTime Timestamp;
        private double MeasuredValue;

        public Load()
        {

        }

        public Load(int id, DateTime timestamp, double measuredValue)
        {
            Id = id;
            Timestamp = timestamp;
            MeasuredValue = measuredValue;
        }

        [DataMember]
        public int Id1 { get => Id; set => Id = value; }
        [DataMember]
        public DateTime Timestamp1 { get => Timestamp; set => Timestamp = value; }
        [DataMember]
        public double MeasuredValue1 { get => MeasuredValue; set => MeasuredValue = value; }
    }
}
