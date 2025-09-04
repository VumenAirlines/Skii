using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Skii.Enums;

namespace Skii.Models;

public class DateSelection
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid AnswerId { get; init; }
    public DateOnly Date { get; set; }
    public DateChoices? Choice { get; set; }
    [ForeignKey("AnswerId")]
    [JsonIgnore]
    public Answer? Answer { get; init; }
}