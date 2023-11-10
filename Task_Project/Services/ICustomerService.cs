using Task_Project.Models;

namespace Task_Project.Services
{
    public interface ICustomerService
    {
        Task<Customers> GetCustomerAsync(int id);
        Task<IEnumerable<Customers>> GetAllCustomersAsync();
        Task<Customers> AddCustomerAsync(Customers customer);
        Task<bool> DeleteCustomerAsync(int id);
    }
}
