using Skii.Models;

namespace Skii.DTOs;

public class UpdateAnswerParsedDto
{
   
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public bool? SkiiOnFirstDay { get; set; } 
    public bool? PreferWeekends { get; set; }
    public List<DateSelection>? AvailableDates { get; set; } = [];
}