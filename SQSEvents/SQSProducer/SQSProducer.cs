using System;
using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using NLog;
using SQSProducer.Models;
using Stats;

namespace SQSProducer
{
    public class SQSProducer
    {
        private readonly AmazonSQSClient _client;
        private readonly string _queueUrl;
        private readonly Logger _logger;
        public const string MessageGroupId = "Testing";

        public SQSProducer(string accessKey, string secretKey, string queueUrl)
        {
            var awsCreds = new BasicAWSCredentials(accessKey, secretKey);
            _client = new AmazonSQSClient(awsCreds, RegionEndpoint.EUWest1);
            _queueUrl = queueUrl;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void SendMessage()
        {
            while (true)
            {
                var messageData = new SampleData
                {
                    MessageId = Guid.NewGuid(),
                    MessageBody = DateTime.Now.Millisecond.ToString()
                };

                var messageString = JsonConvert.SerializeObject(messageData);

                var sendRequest = new SendMessageRequest
                {
                    QueueUrl = _queueUrl,
                    MessageBody = messageString,
                    MessageGroupId = MessageGroupId,
                    MessageDeduplicationId = Guid.NewGuid().ToString()
                };
                var result = _client.SendMessageAsync(sendRequest).Result;

                if (result.HttpStatusCode != HttpStatusCode.OK)
                {
                    _logger.Error("Error in sending SQS Request");
                }
                else
                {
                    Stats.Stats.ProducerCounter++;
                }
            }
           
        }
    }
}
