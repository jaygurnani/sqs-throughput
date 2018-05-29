using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQSConsumer;
using SQSProducer;

namespace SQSEvents
{
    class Program
    {
        static string accessKey = "";
        static string secretKey = "";
        private static int producerCounter;

        static void Main(string[] args)
        {
            Console.WriteLine("Enter number of producer threads");
            var producerThreadsString = Console.ReadLine();

            Console.WriteLine("Enter number of consumer threads");
            var consumerThreadsString = Console.ReadLine();

            // Setup the Consumers
            SetupConsumers(Int32.Parse(consumerThreadsString));

            // Setup the Producers
            SetupProducers(Int32.Parse(producerThreadsString));

            // Print out some stats
            ReadStats();

            Console.ReadKey();
        }

        static void SetupProducers(int threads)
        {
            for (int i = 0; i < threads; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    var sqsProducer = new SQSProducer.SQSProducer(accessKey, secretKey, "https://sqs.eu-west-1.amazonaws.com/093471706084/sqs-events.fifo");
                    sqsProducer.SendMessage();
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

        static void ReadStats()
        {
            Task.Factory.StartNew(() =>
            {
                int currentSeconds = 1;
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"Total Messages Produced {Stats.Stats.ProducerCounter}");
                    Console.WriteLine($"Producer Throughput (msg/second): {Stats.Stats.ProducerCounter / currentSeconds}");
                    Console.WriteLine();
                    Console.WriteLine($"Total Messages Consumed {Stats.Stats.ConsumerCounter}");
                    Console.WriteLine($"Consumer Throughput (msg/second): {Stats.Stats.ConsumerCounter / currentSeconds}");
                    Console.WriteLine();
                    Console.WriteLine($"Total Time: {currentSeconds}");
                    currentSeconds++;
                    Thread.Sleep(1000);
                }
            }); 
        }
    }
}
