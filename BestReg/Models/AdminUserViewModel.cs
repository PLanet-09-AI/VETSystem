using BestReg.Data;
using Microsoft.AspNetCore.Identity;

namespace BestReg.Models
{
    public class AdminUserViewModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Roles { get; set; }  // This property holds the roles
    }

}
