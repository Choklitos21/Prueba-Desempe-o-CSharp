namespace PruebaDsesempeño.Models;

public class UserViewModel
{
    public User User { get; set; } = new User();

    public IEnumerable<User> UserList { get; set; } = new List<User>();
}