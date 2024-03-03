using Amazon.SQS;
using Amazon.SQS.Model;
using SqsPublisher;
using System.Text.Json;

AmazonSQSClient sqsClient = new ();

CustomerCreated customer = new ()
{
    Id = Guid.NewGuid(),
    Email = "evgeni@tsonev.com",
    FullName = "Evgeni Tsonev",
    DateOfBirth = new DateTime(1999, 2, 23),
    GitHubUsername = "evgenitsonev"
};

GetQueueUrlResponse queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

SendMessageRequest sendMessageRequest = new ()
{
    QueueUrl = queueUrlResponse.QueueUrl,
    MessageBody = JsonSerializer.Serialize(customer),
    MessageAttributes = new Dictionary<string, MessageAttributeValue>
    {
        {
            "MessageType", new MessageAttributeValue
            {
                DataType = "String",
                StringValue = nameof(CustomerCreated)
            }
        }
    },

};

SendMessageResponse response = await sqsClient.SendMessageAsync(sendMessageRequest);

Console.WriteLine();