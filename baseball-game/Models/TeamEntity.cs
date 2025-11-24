using System.ComponentModel.DataAnnotations;

namespace simulator_console.Models;

public class TeamEntity
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string Color { get; set; } = "#000000";
}
