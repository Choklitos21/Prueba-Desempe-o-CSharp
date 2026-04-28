using Microsoft.EntityFrameworkCore;
using PruebaDsesempeño.Data;
using PruebaDsesempeño.Enums;
using PruebaDsesempeño.Models;
using PruebaDsesempeño.Response;

namespace PruebaDsesempeño.Services;

public class ReservationService
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    public ReservationService(AppDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    // Method to retrieve all users and spaces from the database, if there are none saved yet, it will return null
    // This method is used to show all users and spaces in the select inputs from the Index view
    public async Task<ResponseService<Dictionary<string, Object>>> GetUsersAndSpaces()
    {
        try
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
        catch (Exception e)
        {
            return new ResponseService<Dictionary<string, Object>>(
                null,
                "Error " + e.Message,
                false
            );
        }
        
        
    }

    // Method to retrieve all reservations from the database, if there's none user saved yet, it will return null
    public async Task<ResponseService<List<Reservation>>> GetAllReservations()
    {
        try
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
        catch (Exception e)
        {
            return new ResponseService<List<Reservation>>(
                null,
                "Error " + e.Message,
                false);
        }
        
        
    }

    // Method to create a new reservation
    // This method first validate that the userId and SpaceId are correct, and they exist
    // If one of them doesn't exist, it will return a error message
    // It also validates that the new reservation doesn't have any scheduling issues with other reservers made previusly
    public async Task<ResponseService<Reservation>> CreateReservation(int userId, int spaceId, DateTime date, DateTime startTime, DateTime endTime)
    {
        try
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
            
            await _emailService.SendAsync(
                user.Email, 
                "Reservation created", 
                "Your reservation has been created"
                );
        
            return new ResponseService<Reservation>(
                newReservation,
                "Reservation created",
                true
                );
        }
        catch (Exception e)
        {
            return new ResponseService<Reservation>(
                null,
                "Error  " + e.Message,
                false
            );
        }
    }

    // Method to find a reservation by his ID
    // If not found, it will return an error message
    public async Task<ResponseService<Reservation>> FindReservationById(int id)
    {
        try
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
        catch (Exception e)
        {
            return new ResponseService<Reservation>(
                null,
                "Error",
                false
            );
        }
    }
    
    // Method to update a reservation
    // It can update the status, date, startTime, endTime only
    // User and Space cannot be changed
    // In case of need to change user, it has to create a new Reservation
    public async Task<ResponseService<Reservation>> UpdateReservation(int Id, ReservationStatus? status, DateTime? date, DateTime? startTime, DateTime? endTime)
    {
        try
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
        catch (Exception e)
        {
            return new ResponseService<Reservation>(
                null,
                "Error   " + e.Message,
                false); 
        }
    }
    
    // Method to change the status of the reservation to Canceled
    // Reservations cannot be deleted, only change to canceled status
    public async Task<ResponseService<Reservation>> CancelReservation(int id)
    {
        try
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
        catch (Exception e)
        {
            return new ResponseService<Reservation>(
                null,
                "Error  " + e.Message,
                false);
        }
    }
}