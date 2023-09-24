using Microsoft.AspNetCore.Identity;

namespace CouponAPI.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
}