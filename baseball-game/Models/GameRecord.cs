using System.ComponentModel.DataAnnotations;

namespace simulator_console.Models;

public class GameRecord
{
    [Key]
    public int Id { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public string Winner { get; set; } = string.Empty;
    public DateTime PlayedAt { get; set; } = DateTime.Now;
    
    public List<GameLog> Logs { get; set; } = new();
}
