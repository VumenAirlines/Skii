using System.Text.Json.Serialization;
using Skii.Enums;
using Skii.Models;

namespace Skii.DTOs;

public class CreateAnswerDto
{
    [JsonPropertyName("length_min")] 
   public int MinLength { get; set; }
    [JsonPropertyName("length_max")] 
    public int MaxLength { get; set; }
    [JsonPropertyName("first_day_skiing")] 
    public  bool SkiiOnFirstDay  { get; set; }
    [JsonPropertyName("prefer_weekends")] 
    public bool PreferWeekends { get; set; }
    [JsonPropertyName("availability")] 
    public  Dictionary<DateOnly, DateChoices> AvailableDates { get; set; } = [];
}
