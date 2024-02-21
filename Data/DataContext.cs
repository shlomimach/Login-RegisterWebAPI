using Microsoft.EntityFrameworkCore;
using LoginAndRegister.Models;

namespace LoginAndRegister.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
          : base(options)
        {

        }

        public DbSet<User> Users { get; set; }


    }

}
