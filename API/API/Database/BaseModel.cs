using System.Diagnostics.CodeAnalysis;

namespace API.Database;

public class BaseModel
{
    [NotNull]
    public long Id { get; set; }
}