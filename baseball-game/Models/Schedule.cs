using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace simulator_console.Models;

public class Schedule
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SeasonId { get; set; } = string.Empty;
    public int Day { get; set; }
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public bool IsPlayed { get; set; } = false;
    public int? GameRecordId { get; set; }

    [ForeignKey("SeasonId")]
    public Season Season { get; set; } = null!;
}
