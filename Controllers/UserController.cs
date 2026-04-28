using Microsoft.AspNetCore.Mvc;
using PruebaDsesempeño.Models;
using PruebaDsesempeño.Response;
using PruebaDsesempeño.Services;

namespace PruebaDsesempeño.Controllers;

public class UserController: Controller
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }
    
    // Controller to go at main view Index, where the creation form and the table with all users can be found
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllUsers();
        var viewModel = new UserViewModel()
        {
            UserList = users.Data,
            User = new User()
        };
        return View(viewModel);
    }
    
    // Controller method to create a new user, it only calls the service and reload the Index view
    [HttpPost]
    public async Task<IActionResult> Store(User user)
    {
        if (ModelState.IsValid)
        {
            var response = await _userService.CreateUser(user);
        
            TempData["Success"] = response.Success.ToString();
            TempData["Message"] = response.Message;
        }

        return RedirectToAction("Index");
    }
    
    // Controller that will send the user data to the Edit view
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userService.FindUserById(id);
        
        return View(user.Data);
    }
    
    // Controller that calls the update method inside UserService and redirect to the Index view afterward
    public async Task<IActionResult> EditUser(User newUser)
    {
        if (ModelState.IsValid)
        {
            var response = await _userService.UpdateUser(newUser);
            TempData["Success"] = response.Success.ToString();
            TempData["Message"] = response.Message;
        }

        return RedirectToAction("Index");
    }
    
    // Controller method that calls the delete method inside UserService and redirect to the Index view afterward
    [HttpPost]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var response = await _userService.DeleteUser(id);
        
        TempData["Success"] = response.Message;
        TempData["Message"] = response.Success.ToString();
        
        return RedirectToAction("Index");
    }
}