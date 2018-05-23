using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQSConsumer;
using SQSEvents.Models;
using SQSProducer;

namespace SQSEvents
{
    class Program
    {
        static string accessKey = "AKIAIZEJJ7VBN2RMIYXQ";
        static string secretKey = "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";

        static void Main(string[] args)
        {
            Console.WriteLine("Enter number of producer threads");
            var producerThreadsString = Console.ReadLine();

            Console.WriteLine("Enter number of consumer threads");
            var consumerThreadsString = Console.ReadLine();

            // Setup the Producers
            SetupProducers(Int32.Parse(producerThreadsString));
            
            // Setup the Consumers
            SetupConsumers(Int32.Parse(consumerThreadsString));

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void SetupProducers(int threads)
        {
            for (int i = 0; i < threads; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    var sqsProducer = new SQSProducer.SQSProducer(accessKey, secretKey, "https://sqs.eu-west-1.amazonaws.com/093471706084/sqs-events.fifo");
                    var messageData = new SampleData
                    {
                        MessageId = Guid.NewGuid(),
                        MessageBody = DateTime.Now.ToLongDateString()
                    };

                    var messageString = JsonConvert.SerializeObject(messageData);
                    sqsProducer.SendMessage(messageString);
                });
            }
        }

        static void SetupConsumers(int threads)
        {
            for (int i = 0; i < threads; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    var sqsConsumer = new SQSConsumer.SQSConsumer(accessKey, secretKey, "https://sqs.eu-west-1.amazonaws.com/093471706084/sqs-events.fifo");
                    sqsConsumer.ReceieveMessage();
                });
            }
        }
    }
}
