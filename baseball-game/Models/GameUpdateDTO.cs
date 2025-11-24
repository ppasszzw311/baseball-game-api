using simulator_console.Models;

namespace simulator_console.Models;

public class GameUpdateDTO
{
    public int CurrentInning { get; set; }
    public bool IsTopInning { get; set; }
    public int Outs { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public Player?[] Bases { get; set; } = new Player?[4];
    public string? LastPlayResult { get; set; }
    public string? CurrentBatterName { get; set; }
    public string? CurrentPitcherName { get; set; }
    public bool IsGameOver { get; set; }
}
