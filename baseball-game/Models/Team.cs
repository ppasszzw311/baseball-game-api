namespace baseball_game.Models;

public class Team
{
    public int TeamId { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
    public DateTime CreatedAt { get; set; }
}