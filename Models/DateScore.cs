
namespace Skii.Models;

public class DateScore 
{
    public DateOnly Date {get; set;}
    public int Score {get;set;}
    public HashSet<Guid> UserIds {get; set;} = [];
    
}