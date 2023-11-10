using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task_Project.Models;
using Task_Project.Services;

namespace Task_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _customerService.GetCustomerAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromBody] Customers customer)
        {
            var addedCustomer = await _customerService.AddCustomerAsync(customer);
            return CreatedAtAction(nameof(GetCustomer), new { id = addedCustomer.Id }, addedCustomer);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var success = await _customerService.DeleteCustomerAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
