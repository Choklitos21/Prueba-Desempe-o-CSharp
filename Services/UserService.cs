using Microsoft.EntityFrameworkCore;
using PruebaDsesempeño.Data;
using PruebaDsesempeño.Models;
using PruebaDsesempeño.Response;

namespace PruebaDsesempeño.Services;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }
    
    // Method to retrieve all users from the database, if there's none user saved yet, it will return null
    public async Task<ResponseService<List<User>>> GetAllUsers()
    {
        try
        {
            var users = await _context.User.ToListAsync();
            return new ResponseService<List<User>>(
                users,
                users.Count > 0 ? "Users Loaded" : "No users on db yet",
                users.Count > 0 ? true : false);
        }
        catch (Exception e)
        {
            return new ResponseService<List<User>>(
                null,
                "Error " + e.Message,
                false);
        }
    }

    // Create a new user verifying that the C.C or Email are not registered before
    public async Task<ResponseService<User>> CreateUser(User user)
    {
        try
        {
            var anyUser = await _context.User.AnyAsync(u => u.IDCard == user.IDCard || u.Email == user.Email);

            if (anyUser)
            {
                return new ResponseService<User>(
                    user,
                    "User already registered",
                    false
                );
            }

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            return new ResponseService<User>(
                user,
                "User created",
                true
            );
        }
        catch (Exception e)
        {
            return new ResponseService<User>(
                user,
                "Error " + e.Message,
                false
            );
        }
        
    }

    // Try to find a user for his ID, if not found, it will return an error message with null as data
    public async Task<ResponseService<User>> FindUserById(int id)
    {
        try
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return new ResponseService<User>(
                    null,
                    "User not found",
                    false
                );
            }
        
            return new ResponseService<User>(
                user,
                "User found",
                true
            );
        }
        catch (Exception e)
        {
            return new ResponseService<User>(
                null,
                "Error " + e.Message,
                false
            );
        }
        
        
    }
    
    // Method to update an old user with new information
    // It can update 1 or more parts of the user, not all is requested
    public async Task<ResponseService<User>> UpdateUser(User newUser)
    {
        try
        {
            var oldUser = await _context.User.FirstOrDefaultAsync(x => x.Id == newUser.Id);
        
            if (oldUser != null)
            {
                _context.Entry(oldUser).CurrentValues.SetValues(newUser);
                await _context.SaveChangesAsync();
                return new ResponseService<User>(
                    newUser,
                    "User updated correctly",
                    true);
            }
        
            return new ResponseService<User>(
                newUser,
                "User not found",
                false); 
        }
        catch (Exception e)
        {
            return new ResponseService<User>(
                newUser,
                "error " + e.Message,
                false); 
        }
    }
    
    // Delete a user from the DB
    // This method first validate that the user exists, if so, it will be removed from the DB
    // If not found, it will return an error message
    public async Task<ResponseService<User>> DeleteUser(int id)
    {
        try
        {
            var oldUser = await _context.User.FirstOrDefaultAsync(x => x.Id == id);
        
            if (oldUser != null)
            {
                _context.Remove(oldUser);
                await _context.SaveChangesAsync();
                return new ResponseService<User>(
                    oldUser,
                    "User removed",
                    true);
            }
        
            return new ResponseService<User>(
                oldUser,
                "User not found",
                false);
        }
        catch (Exception e)
        {
            return new ResponseService<User>(
                null,
                "Error " + e.Message,
                false);
        }
    }
}