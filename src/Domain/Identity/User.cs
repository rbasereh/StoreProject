using Microsoft.AspNetCore.Identity;

namespace TP.Domain.Identity;

public class User : IdentityUser<int>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public string ProfileUrl { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime Created { get; set; }
}

