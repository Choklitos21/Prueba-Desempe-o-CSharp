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

    public async Task<ResponseService<List<User>>> GetAllUsers()
    {
        var users = await _context.User.ToListAsync();
        return new ResponseService<List<User>>(
            users,
            users.Count > 0 ? "Users Loaded" : "No users on db yet",
            users.Count > 0 ? true : false);
    }

    public async Task<ResponseService<User>> CreateUser(User user)
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
}