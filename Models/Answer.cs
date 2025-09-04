using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Skii.Models;

public class Answer
{ 
    [Key]
    public Guid Id { get; init; }
    public int MinLength { get; set; }
    public int MaxLength { get; set; }
    public bool SkiiOnFirstDay { get; set; } 
    public bool PreferWeekends { get; set; }
    public List<DateSelection> AvailableDates { get; set; } = [];
    
    public Guid UserId { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; init; }
}