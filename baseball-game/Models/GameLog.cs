using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace simulator_console.Models;

public class GameLog
{
    [Key]
    public int Id { get; set; }
    public int GameId { get; set; }
    public string LogMessage { get; set; } = string.Empty;

    [ForeignKey("GameId")]
    public GameRecord Game { get; set; } = null!;
}
