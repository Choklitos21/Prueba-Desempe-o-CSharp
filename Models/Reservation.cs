using PruebaDsesempeño.Enums;

namespace PruebaDsesempeño.Models;

public class Reservation
{
    public int Id { get; set; }
    public ReservationStatus Status { get; set; }
    
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public int SpaceId { get; set; }
    public Space Space { get; set; }
    
    public ICollection<User> User { get; set; } = new List<User>();
    

}