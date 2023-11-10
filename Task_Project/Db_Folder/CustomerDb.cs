using Microsoft.EntityFrameworkCore;

namespace Task_Project.Db_Folder
{
    public class CustomerDb : DbContext
    {

        public CustomerDb(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Models.Customers> CustomersDetails { get; set; }
    }
}
