using Microsoft.AspNetCore.Mvc;
using PruebaDsesempeño.Enums;
using PruebaDsesempeño.Models;
using PruebaDsesempeño.Services;

namespace PruebaDsesempeño.Controllers;

public class ReservationController: Controller
{
    private readonly ReservationService _reservationService;

    public ReservationController(ReservationService reservationService)
    {
        _reservationService = reservationService;
    }
    
    // Controller to go at main view Index, where the creation form and the table with all reservations can be found
    public async Task<IActionResult> Index()
    {
        var reservations = await _reservationService.GetAllReservations();
        var usersAndSpaces = await _reservationService.GetUsersAndSpaces();
        
        var viewModel = new ReservationViewModel()
        {
            ReservationList = reservations.Data,
            Reservation = new Reservation(),
            UserList = (List<User>)usersAndSpaces.Data["Users"],
            SpaceList = (List<Space>)usersAndSpaces.Data["Spaces"]
        };
        return View(viewModel);
    }
    
    // Controller method to create a new reservation, it only calls the service and reload the Index view
    [HttpPost]
    public async Task<IActionResult> Store(int userId, int spaceId, DateTime date, DateTime startTime, DateTime endTime)
    {
        var response = await _reservationService.CreateReservation(userId, spaceId, date, startTime, endTime);

        TempData["Success"] = response.Success.ToString();
        TempData["Message"] = response.Message;

        return RedirectToAction("Index");
    }
    
    // Controller that will send the reservation data to the Edit view
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _reservationService.FindReservationById(id);
        
        return View(user.Data);
    }
    
    // Controller that calls the update method inside ReservationService and redirect to the Index view afterward
    public async Task<IActionResult> EditReservation(int Id, ReservationStatus? Status, DateTime? Date, DateTime? StartTime, DateTime? EndTime)
    {
        var response = await _reservationService.UpdateReservation(Id, Status, Date, StartTime, EndTime);
        TempData["Success"] = response.Success.ToString();
        TempData["Message"] = response.Message;
            
        return RedirectToAction("Index");
    }
    
    // Controller method that calls the delete method inside ReservationService and redirect to the Index view afterward
    [HttpPost]
    public async Task<IActionResult> CancelReservation(int id)
    {
        var response = await _reservationService.CancelReservation(id);
        
        TempData["Success"] = response.Message;
        TempData["Message"] = response.Success.ToString();
        
        return RedirectToAction("Index");
    }
}