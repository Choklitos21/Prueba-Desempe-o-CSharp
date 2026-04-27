namespace PruebaDsesempeño.Models;

public class ReservationViewModel
{
    public Reservation Reservation { get; set; } = new Reservation();

    public IEnumerable<Reservation> ReservationList { get; set; } = new List<Reservation>();
    
    public IEnumerable<User>? UserList { get; set; } = new List<User>();
    
    public IEnumerable<Space>? SpaceList { get; set; } = new List<Space>();
}