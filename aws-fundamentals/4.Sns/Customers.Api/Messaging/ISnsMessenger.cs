namespace Customers.Api.Messaging;

using Amazon.SimpleNotificationService.Model;

public interface ISnsMessenger
{
    Task<PublishResponse> PublishMessageAsync<T>(T message);
}
