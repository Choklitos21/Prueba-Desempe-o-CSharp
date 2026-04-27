namespace PruebaDsesempeño.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string IDCard { get; set; }
    public int Phone { get; set; }
    public string Email { get; set; }
    
    public IEnumerable<Reservation>? Reservation { get; set; } = new List<Reservation>();
}