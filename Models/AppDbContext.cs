using Microsoft.EntityFrameworkCore;
using GennyOnlineStoreBE.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GennyOnlineStoreBE.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUsers>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
                
        }
    }
}
