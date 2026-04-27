using PruebaDsesempeño.Enums;

namespace PruebaDsesempeño.Models;

public class Space
{
    public int Id { get; set; }
    public string Name { get; set; }
    public SpaceType Type { get; set; }
    public int Capacity { get; set; }
    
    public IEnumerable<Reservation>? Reservation { get; set; } = new List<Reservation>();
}