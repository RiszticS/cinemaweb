using Microsoft.AspNetCore.Identity;

namespace Cinema.DataAccess.Models;

public class UserRole: IdentityRole
{
    public UserRole() { }
    public UserRole(string role): base(role) { }
}