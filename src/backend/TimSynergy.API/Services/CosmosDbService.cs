using Microsoft.Azure.Cosmos;
using TimSynergy.API.Models;

namespace TimSynergy.API.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private readonly Container _customerContainer;
        private readonly Container _interactionContainer;

        public CosmosDbService(CosmosClient cosmosClient, string databaseName)
        {
            _customerContainer = cosmosClient.GetContainer(databaseName, "Customers");
            _interactionContainer = cosmosClient.GetContainer(databaseName, "Interactions");
        }

        // Customer operations
        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            var query = _customerContainer.GetItemQueryIterator<Customer>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<Customer>();
            
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            
            return results;
        }

        public async Task<Customer> GetCustomerAsync(string id)
        {
            try
            {
                var response = await _customerContainer.ReadItemAsync<Customer>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            var response = await _customerContainer.CreateItemAsync(customer, new PartitionKey(customer.Id));
            return response.Resource;
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            customer.UpdatedAt = DateTime.UtcNow;
            var response = await _customerContainer.UpsertItemAsync(customer, new PartitionKey(customer.Id));
            return response.Resource;
        }

        public async Task DeleteCustomerAsync(string id)
        {
            await _customerContainer.DeleteItemAsync<Customer>(id, new PartitionKey(id));
        }

        // Interaction operations
        public async Task<IEnumerable<Interaction>> GetInteractionsAsync(string customerId)
        {
            var query = _interactionContainer.GetItemQueryIterator<Interaction>(
                new QueryDefinition("SELECT * FROM c WHERE c.CustomerId = @customerId")
                .WithParameter("@customerId", customerId));
            
            var results = new List<Interaction>();
            
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            
            return results;
        }

        public async Task<Interaction> GetInteractionAsync(string id)
        {
            try
            {
                var response = await _interactionContainer.ReadItemAsync<Interaction>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<Interaction> AddInteractionAsync(Interaction interaction)
        {
            var response = await _interactionContainer.CreateItemAsync(interaction, new PartitionKey(interaction.Id));
            return response.Resource;
        }

        public async Task<Interaction> UpdateInteractionAsync(Interaction interaction)
        {
            interaction.UpdatedAt = DateTime.UtcNow;
            var response = await _interactionContainer.UpsertItemAsync(interaction, new PartitionKey(interaction.Id));
            return response.Resource;
        }

        public async Task DeleteInteractionAsync(string id)
        {
            await _interactionContainer.DeleteItemAsync<Interaction>(id, new PartitionKey(id));
        }
    }
}
