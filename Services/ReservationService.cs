using Microsoft.EntityFrameworkCore;
using PruebaDsesempeño.Data;
using PruebaDsesempeño.Enums;
using PruebaDsesempeño.Models;
using PruebaDsesempeño.Response;

namespace PruebaDsesempeño.Services;

public class ReservationService
{
    private readonly AppDbContext _context;

    public ReservationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ResponseService<Dictionary<string, Object>>> GetUsersAndSpaces()
    {
        Dictionary<string, object> usersAndSpaces = new Dictionary<string, object>();
        usersAndSpaces["Users"] = await _context.User.ToListAsync();
        usersAndSpaces["Spaces"] = await _context.Space.ToListAsync();
        
        return new ResponseService<Dictionary<string, Object>>(
            usersAndSpaces,
            usersAndSpaces.Count > 0 ? "Users Loaded" : "NoUsers",
            usersAndSpaces.Count > 0 ? true : false
            );
    }

    public async Task<ResponseService<List<Reservation>>> GetAllReservations()
    {
        var reservations = await _context.Reservation
            .Include(r => r.User)
            .Include(r => r.Space)
            .ToListAsync();
        return new ResponseService<List<Reservation>>(
            reservations,
            reservations.Count > 0 ? "Reservations Loaded" : "No reservations on db yet",
            reservations.Count > 0 ? true : false);
    }

    public async Task<ResponseService<Reservation>> CreateReservation(Reservation reservation)
    {
        var anyReservation = await _context.Reservation.AnyAsync(r => r.Id == reservation.Id);

        if (anyReservation)
        {
            return new ResponseService<Reservation>(
                reservation,
                "User already registered",
                false
            );
        }

        await _context.AddAsync(reservation);
        await _context.SaveChangesAsync();
        return new ResponseService<Reservation>(
            reservation,
            "User created",
            true
            );
    }

    public async Task<ResponseService<Reservation>> FindReservationById(int id)
    {
        var reservation = await _context.Reservation.FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
        {
            return new ResponseService<Reservation>(
                null,
                "User not found",
                false
            );
        }
        
        return new ResponseService<Reservation>(
            reservation,
            "User found",
            true
        );
    }
    
    public async Task<ResponseService<Reservation>> UpdateReservation(Reservation newReservation)
    {
        var oldReservation = await _context.Reservation.FirstOrDefaultAsync(x => x.Id == newReservation.Id);
        
        if (oldReservation != null)
        {
            _context.Entry(oldReservation).CurrentValues.SetValues(newReservation);
            await _context.SaveChangesAsync();
            return new ResponseService<Reservation>(
                newReservation,
                "Reservation updated correctly",
                true);
        }
        
        return new ResponseService<Reservation>(
            newReservation,
            "Reservation not found",
            false); 
    }
    
    public async Task<ResponseService<Reservation>> CancelReservation(int id)
    {
        var oldReservation = await _context.Reservation.FirstOrDefaultAsync(x => x.Id == id);
        
        if (oldReservation != null)
        {
            oldReservation.Status = ReservationStatus.Canceled;
            await _context.SaveChangesAsync();
            return new ResponseService<Reservation>(
                oldReservation,
                "Reservation canceled",
                true);
        }
        
        return new ResponseService<Reservation>(
            oldReservation,
            "Reservation not found",
            false);
    }
}