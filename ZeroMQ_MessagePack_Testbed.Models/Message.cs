using System;

namespace ZeroMQ_MessagePack_Testbed.Models
{
    public class Message
    {
        public Guid Id;

        public Priority Priority;

        public string Subject;

        public string Body;

        public DateTime Timestamp;
    }

    public enum Priority
    {
        High,
        Medium,
        Low
    }
}
