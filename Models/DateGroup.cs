namespace Skii.Models;

public class DateGroup
{
    public DateOnly Date { get; set; }
    public HashSet<Guid> UserIds { get; set; } = [];
}