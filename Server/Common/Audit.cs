using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class Audit
    {
        private int Id;
        private DateTime Timestamp;
        private MessageType MessageType;
        private string Message;

        public Audit(int id, DateTime timestamp, MessageType messageType, string message)
        {
            Id = id;
            Timestamp = timestamp;
            MessageType = messageType;
            Message = message;
        }

        [DataMember]
        public int Id1 { get => Id; set => Id = value; }
        [DataMember]
        public DateTime Timestamp1 { get => Timestamp; set => Timestamp = value; }
        [DataMember]
        public MessageType MessageType1 { get => MessageType; set => MessageType = value; }
        [DataMember]
        public string Message1 { get => Message; set => Message = value; }

        public override string ToString()
        {
            return MessageType1.ToString() + "\t" + Timestamp.ToString() + "\t" + Message;
        }
    }
}
