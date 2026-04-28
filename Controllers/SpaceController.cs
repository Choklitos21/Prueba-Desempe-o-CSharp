using Microsoft.AspNetCore.Mvc;
using PruebaDsesempeño.Models;
using PruebaDsesempeño.Services;

namespace PruebaDsesempeño.Controllers;

public class SpaceController: Controller
{
    private readonly SpaceService _spaceService;

    public SpaceController(SpaceService spaceService)
    {
        _spaceService = spaceService;
    }
    
    // Controller to go at main view Index, where the creation form and the table with all spaces can be found
    public async Task<IActionResult> Index()
    {
        var spaces = await _spaceService.GetAllSpaces();
        var viewModel = new SpaceViewModel()
        {
            SpaceList = spaces.Data,
            Space = new Space()
        };
        return View(viewModel);
    }
    
    // Controller method to create a new space, it only calls the service and reload the Index view
    [HttpPost]
    public async Task<IActionResult> Store(Space space)
    {
        if (ModelState.IsValid)
        {
            if (space.Capacity > 20 || space.Capacity <= 0)
            {
                TempData["Success"] = false;
                TempData["Message"] = "Capacity cannot be more than 20 or less than 1";
                return RedirectToAction("Index");
            }
            
            var response = await _spaceService.CreateSpace(space);
        
            TempData["Success"] = response.Success.ToString();
            TempData["Message"] = response.Message;
        }
        
        return RedirectToAction("Index");
    }
    
    // Controller that will send the space data to the Edit view
    public async Task<IActionResult> Edit(int id)
    {
        var space = await _spaceService.FindSpaceById(id);
        
        return View(space.Data);
    }
    
    // Controller that calls the update method inside SpaceService and redirect to the Index view afterward
    public async Task<IActionResult> EditSpace(Space newSpace)
    {
        if (ModelState.IsValid)
        {
            var response = await _spaceService.UpdateSpace(newSpace);
            TempData["Success"] = response.Success.ToString();
            TempData["Message"] = response.Message;
        }

        return RedirectToAction("Index");
    }
    
    // Controller method that calls the delete method inside SpaceService and redirect to the Index view afterward
    [HttpPost]
    public async Task<IActionResult> DeleteSpace(int id)
    {
        var response = await _spaceService.DeleteSpace(id);
        
        TempData["Message"] = response.Success.ToString();
        TempData["Success"] = response.Message;
        
        return RedirectToAction("Index");
    }
}