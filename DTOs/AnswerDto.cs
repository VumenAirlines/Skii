using Skii.Models;

namespace Skii.DTOs;

public record AnswerDto(
    Guid Id,
    int MinLength,
    int MaxLength ,
    bool SkiiOnFirstDay, 
    bool PreferWeekends,
    List<DateSelection> AvailableDates);  
