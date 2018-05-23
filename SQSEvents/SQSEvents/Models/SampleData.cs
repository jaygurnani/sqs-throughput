using System;
using System.Collections.Generic;
using System.Text;

namespace SQSEvents.Models
{
    public class SampleData
    {
        public Guid MessageId { get; set; }
        public string MessageBody { get; set; }
    }
}
