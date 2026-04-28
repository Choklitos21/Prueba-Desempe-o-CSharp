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

    public async Task<ResponseService<Reservation>> CreateReservation(int userId, int spaceId, DateTime date, DateTime startTime, DateTime endTime)
    {
        var user = await _context.User.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null) return new ResponseService<Reservation>(null, "User not match", false);
        
        var space = await _context.Space.FirstOrDefaultAsync(x => x.Id == spaceId);
        if (space == null) return new ResponseService<Reservation>(null, "No space Match", false);
        
        var reservationsAvailables = await _context.Reservation
            .Where(r => r.Status != ReservationStatus.Canceled && r.Date == DateTime.SpecifyKind(date, DateTimeKind.Utc))
            .ToListAsync();

        foreach (var reserv in reservationsAvailables)
        {
            if ((startTime >= reserv.StartTime && startTime <= reserv.EndTime) ||
                (endTime >= reserv.StartTime && endTime <= reserv.EndTime) ||
                (reserv.StartTime >= startTime && reserv.StartTime <= endTime) ||
                (reserv.EndTime >= startTime && reserv.EndTime <= endTime))
            {
                return new ResponseService<Reservation>(
                    null,
                    "Reservation cant be made due issues with another reservation time",
                    false
                );
            }
        }

        var newReservation = new Reservation();
        newReservation.User = new List<User>();
        newReservation.User.Add(user);
        newReservation.SpaceId = spaceId;
        newReservation.Space = space;
        newReservation.Status = ReservationStatus.Confirmed;
        newReservation.Date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        newReservation.StartTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);
        newReservation.EndTime = DateTime.SpecifyKind(endTime, DateTimeKind.Utc);
        
        await _context.AddAsync(newReservation);
        await _context.SaveChangesAsync();
        return new ResponseService<Reservation>(
            newReservation,
            "Reservation created",
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
    
    public async Task<ResponseService<Reservation>> UpdateReservation(int Id, ReservationStatus? status, DateTime? date, DateTime? startTime, DateTime? endTime)
    {
        var oldReservation = await _context.Reservation.FirstOrDefaultAsync(x => x.Id == Id);
        
        if (oldReservation != null)
        {
            if (status != null)
            {
                oldReservation.Status = status.Value;
            }
            
            if (date != null)
            {
                oldReservation.Date = DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
            }
            
            if (startTime != null)
            {
                oldReservation.Date = DateTime.SpecifyKind(startTime.Value, DateTimeKind.Utc);
            }
            
            if (endTime != null)
            {
                oldReservation.Date = DateTime.SpecifyKind(endTime.Value, DateTimeKind.Utc);
            }
            
            await _context.SaveChangesAsync();
            return new ResponseService<Reservation>(
                oldReservation,
                "Reservation updated correctly",
                true);
        }
        
        return new ResponseService<Reservation>(
            oldReservation,
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