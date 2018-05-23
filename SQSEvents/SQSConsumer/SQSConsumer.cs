using System;
using System.Linq;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using NLog;

namespace SQSConsumer
{
    public class SQSConsumer
    {
        private readonly AmazonSQSClient _client;
        private readonly string _queueUrl;
        private readonly Logger _logger;
        public const string MessageGroupId = "Testing";

        public SQSConsumer(string accessKey, string secretKey, string queueUrl)
        {
            var awsCreds = new BasicAWSCredentials(accessKey, secretKey);
            _client = new AmazonSQSClient(awsCreds, RegionEndpoint.EUWest1);
            _queueUrl = queueUrl;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void ReceieveMessage()
        {
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = _queueUrl
            };

            while (true)
            {
                var response = _client.ReceiveMessageAsync(receiveMessageRequest).Result;
                if (response.Messages.Any())
                {
                    foreach (var message in response.Messages)
                    {
                        var deleteMessageRequest = new DeleteMessageRequest
                        {
                            QueueUrl = _queueUrl,
                            ReceiptHandle = message.ReceiptHandle
                        };
                        var result = _client.DeleteMessageAsync(deleteMessageRequest).Result;
                    }
                }
            }
        }
    }
}
