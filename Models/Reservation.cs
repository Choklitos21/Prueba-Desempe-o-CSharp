namespace PruebaDsesempeño.Models;

public class Reservation
{
    public int Id { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public int SpaceId { get; set; }
    public Space Space { get; set; }
    
    public IEnumerable<User> User { get; set; } = new List<User>();
    

}