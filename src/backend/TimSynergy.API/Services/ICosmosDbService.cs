using TimSynergy.API.Models;

namespace TimSynergy.API.Services
{
    public interface ICosmosDbService
    {
        // Customer operations
        Task<IEnumerable<Customer>> GetCustomersAsync();
        Task<Customer> GetCustomerAsync(string id);
        Task<Customer> AddCustomerAsync(Customer customer);
        Task<Customer> UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(string id);
        
        // Interaction operations
        Task<IEnumerable<Interaction>> GetInteractionsAsync(string customerId);
        Task<Interaction> GetInteractionAsync(string id);
        Task<Interaction> AddInteractionAsync(Interaction interaction);
        Task<Interaction> UpdateInteractionAsync(Interaction interaction);
        Task DeleteInteractionAsync(string id);
    }
}
