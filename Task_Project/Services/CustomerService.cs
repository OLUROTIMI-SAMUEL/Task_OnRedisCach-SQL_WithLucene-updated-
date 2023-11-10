using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Task_Project.Db_Folder;
using Task_Project.Models;

namespace Task_Project.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomerDb _dbContext;
        private readonly IDistributedCache _cache;

        public CustomerService(CustomerDb dbContext, IDistributedCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Customers> GetCustomerAsync(int id)
        {
            var cachedCustomer = await _cache.GetStringAsync($"Customer_{id}");
            if (cachedCustomer != null)
            {
                return JsonConvert.DeserializeObject<Customers>(cachedCustomer);
            }

            var customer = await _dbContext.CustomersDetails.FindAsync(id);

            if (customer != null)
            {
                // Cache the customer for 30 seconds
                await _cache.SetStringAsync($"Customer_{id}", JsonConvert.SerializeObject(customer), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });
            }

            return customer;
        }

        public async Task<IEnumerable<Customers>> GetAllCustomersAsync()
        {
            var cachedCustomers = await _cache.GetStringAsync("AllCustomers");
            if (cachedCustomers != null)
            {
                return JsonConvert.DeserializeObject<IEnumerable<Customers>>(cachedCustomers);
            }

            var customers = await _dbContext.CustomersDetails.ToListAsync();

            if (customers.Any())
            {
                // Cache all customers for 30 seconds
                await _cache.SetStringAsync("AllCustomers", JsonConvert.SerializeObject(customers), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });
            }

            return customers;
        }

        public async Task<Customers> AddCustomerAsync(Customers customer)
        {
            // Add the customer to the database
            await _dbContext.CustomersDetails.AddAsync(customer);
            await _dbContext.SaveChangesAsync();

            // Cache the added customer for 30 seconds
            await _cache.SetStringAsync($"Customer_{customer.Id}", JsonConvert.SerializeObject(customer), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            });

            // Invalidate the cache for all customers
            await _cache.RemoveAsync("AllCustomers");

            return customer;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _dbContext.CustomersDetails.FindAsync(id);

            if (customer == null)
            {
                return false; // Customer not found
            }

            // Remove the customer from the database
            _dbContext.CustomersDetails.Remove(customer);
            await _dbContext.SaveChangesAsync();

            // Remove the customer from the cache
            await _cache.RemoveAsync($"Customer_{id}");

            // Invalidate the cache for all customers
            await _cache.RemoveAsync("AllCustomers");

            return true;
        }
    }
}
