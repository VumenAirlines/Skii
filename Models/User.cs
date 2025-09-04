using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Skii.Models;

public class User 
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    [MaxLength(50)] public string GoogleId { get; init; } = string.Empty;

    [MaxLength(320)] 
    public string Email { get; init; } = string.Empty;

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
