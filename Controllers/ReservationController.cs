using Microsoft.AspNetCore.Mvc;
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
    
    [HttpPost]
    public async Task<IActionResult> Store(Reservation reservation)
    {
        if (ModelState.IsValid)
        {
            var response = await _reservationService.CreateReservation(reservation);
        
            TempData["Success"] = response.Success.ToString();
            TempData["Message"] = response.Message;
        }

        return RedirectToAction("Index");
    }
    
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _reservationService.FindReservationById(id);
        
        return View(user.Data);
    }
    
    public async Task<IActionResult> EditReservation(Reservation newReservation)
    {
        if (ModelState.IsValid)
        {
            var response = await _reservationService.UpdateReservation(newReservation);
            TempData["Success"] = response.Success.ToString();
            TempData["Message"] = response.Message;
        }

        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> CancelReservation(int id)
    {
        var response = await _reservationService.CancelReservation(id);
        
        TempData["Success"] = response.Message;
        TempData["Message"] = response.Success.ToString();
        
        return RedirectToAction("Index");
    }
}