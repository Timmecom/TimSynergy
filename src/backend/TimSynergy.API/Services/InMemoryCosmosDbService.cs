using TimSynergy.API.Models;

namespace TimSynergy.API.Services
{
    public class InMemoryCosmosDbService : ICosmosDbService
    {
        private readonly List<Customer> _customers = new();
        private readonly List<Interaction> _interactions = new();

        // Customer operations
        public Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            return Task.FromResult<IEnumerable<Customer>>(_customers);
        }

        public Task<Customer> GetCustomerAsync(string id)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(customer);
        }

        public Task<Customer> AddCustomerAsync(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.Id))
            {
                customer.Id = Guid.NewGuid().ToString();
            }
            customer.CreatedAt = DateTime.UtcNow;
            customer.UpdatedAt = DateTime.UtcNow;
            _customers.Add(customer);
            return Task.FromResult(customer);
        }

        public Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            var index = _customers.FindIndex(c => c.Id == customer.Id);
            if (index >= 0)
            {
                customer.UpdatedAt = DateTime.UtcNow;
                _customers[index] = customer;
            }
            return Task.FromResult(customer);
        }

        public Task DeleteCustomerAsync(string id)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            if (customer != null)
            {
                _customers.Remove(customer);
            }
            return Task.CompletedTask;
        }

        // Interaction operations
        public Task<IEnumerable<Interaction>> GetInteractionsAsync(string customerId)
        {
            var interactions = _interactions.Where(i => i.CustomerId == customerId);
            return Task.FromResult(interactions);
        }

        public Task<Interaction> GetInteractionAsync(string id)
        {
            var interaction = _interactions.FirstOrDefault(i => i.Id == id);
            return Task.FromResult(interaction);
        }

        public Task<Interaction> AddInteractionAsync(Interaction interaction)
        {
            if (string.IsNullOrEmpty(interaction.Id))
            {
                interaction.Id = Guid.NewGuid().ToString();
            }
            interaction.CreatedAt = DateTime.UtcNow;
            interaction.UpdatedAt = DateTime.UtcNow;
            _interactions.Add(interaction);
            return Task.FromResult(interaction);
        }

        public Task<Interaction> UpdateInteractionAsync(Interaction interaction)
        {
            var index = _interactions.FindIndex(i => i.Id == interaction.Id);
            if (index >= 0)
            {
                interaction.UpdatedAt = DateTime.UtcNow;
                _interactions[index] = interaction;
            }
            return Task.FromResult(interaction);
        }

        public Task DeleteInteractionAsync(string id)
        {
            var interaction = _interactions.FirstOrDefault(i => i.Id == id);
            if (interaction != null)
            {
                _interactions.Remove(interaction);
            }
            return Task.CompletedTask;
        }
    }
}
