using System.ComponentModel.DataAnnotations.Schema;

namespace API.Database;

public class Session : BaseModel
{
    public byte[] Key { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    
    public long UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}