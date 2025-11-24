using System.ComponentModel.DataAnnotations;

namespace simulator_console.Models;

public class Season
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int Year { get; set; }
    public int CurrentDay { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
