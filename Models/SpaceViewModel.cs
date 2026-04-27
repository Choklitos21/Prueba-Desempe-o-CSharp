namespace PruebaDsesempeño.Models;

public class SpaceViewModel
{
    public Space Space { get; set; } = new Space();

    public IEnumerable<Space> SpaceList { get; set; } = new List<Space>();
}