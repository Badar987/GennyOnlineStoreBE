using Microsoft.AspNetCore.Identity;

namespace GennyOnlineStoreBE.Models
{
    public class ApplicationUsers : IdentityUser
    {
        public string Name { get; set; }
    }
}
