using BestReg.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace BestReg.Models
{
    public class AdminUserViewModel
    {
        public IEnumerable<BestRegUser> Users { get; set; }
        public UserManager<BestRegUser> UserManager { get; set; }
    }

}
