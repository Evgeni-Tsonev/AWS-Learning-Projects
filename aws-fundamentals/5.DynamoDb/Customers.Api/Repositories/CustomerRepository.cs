using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Customers.Api.Contracts.Data;
using System.Net;
using System.Text.Json;

namespace Customers.Api.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IAmazonDynamoDB _dynamoDB;
    private readonly string _tableIName = "customers";

    public CustomerRepository(IAmazonDynamoDB dynamoDB)
    {
        _dynamoDB = dynamoDB;
    }

    public async Task<bool> CreateAsync(CustomerDto customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        var customerAsJson = JsonSerializer.Serialize(customer);
        var customerAsAttributes = Document.FromJson(customerAsJson).ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = _tableIName,
            Item = customerAsAttributes,
            ConditionExpression = "attribute_not_exists(pk) and attribute_not_exists(sk)"
        };

        var response = await _dynamoDB.PutItemAsync(createItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<CustomerDto?> GetAsync(Guid id)
    {
        var getItemRequest = new GetItemRequest
        {
            TableName = _tableIName,
            Key = new Dictionary<string, AttributeValue>
             {
                 { "pk", new AttributeValue{ S = id.ToString() } },
                 { "sk", new AttributeValue{ S = id.ToString() } }
             }
        };

        var response = await _dynamoDB.GetItemAsync(getItemRequest);

        if (response.Item.Count == 0)
        {
            return null;
        }

        var itemAsDocument = Document.FromAttributeMap(response.Item);
        return JsonSerializer.Deserialize<CustomerDto>(itemAsDocument.ToJson());
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        //Avoid scan request because is exponentially slower and insanely expensive
        var scanRequest = new ScanRequest
        {
            TableName = _tableIName
        };

        var response = await _dynamoDB.ScanAsync(scanRequest);

        return response.Items.Select(x =>
        {
            var json = Document.FromAttributeMap(x).ToJson();
            return JsonSerializer.Deserialize<CustomerDto>(json);
        })!;
    }

    public async Task<bool> UpdateAsync(CustomerDto customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        var customerAsJson = JsonSerializer.Serialize(customer);
        var customerAsAttributes = Document.FromJson(customerAsJson).ToAttributeMap();

        var updateItemRequest = new PutItemRequest
        {
            TableName = _tableIName,
            Item = customerAsAttributes,
        };

        var response = await _dynamoDB.PutItemAsync(updateItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var deleteItemRequest = new DeleteItemRequest
        {
            TableName = _tableIName,
            Key = new Dictionary<string, AttributeValue>
            {
                 { "pk", new AttributeValue{ S = id.ToString() } },
                 { "sk", new AttributeValue{ S = id.ToString() } }
            }
        };

        var response = await _dynamoDB.DeleteItemAsync(deleteItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }
}
