using System.ComponentModel.DataAnnotations;

namespace API.Database;

public class User : BaseModel
{
    [MinLength(2)]
    [MaxLength(32)]
    public string UserName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [MinLength(2)]
    [MaxLength(24)]
    public string? NickName { get; set; }

    public string PasswordHash { get; set; }
}