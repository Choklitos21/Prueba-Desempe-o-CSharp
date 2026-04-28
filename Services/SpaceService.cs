using Microsoft.EntityFrameworkCore;
using PruebaDsesempeño.Data;
using PruebaDsesempeño.Models;
using PruebaDsesempeño.Response;

namespace PruebaDsesempeño.Services;

public class SpaceService
{
    private readonly AppDbContext _context;

    public SpaceService(AppDbContext context)
    {
        _context = context;
    }

    // Method to retrieve all spaces from the database, if there's none user saved yet, it will return null
    public async Task<ResponseService<List<Space>>> GetAllSpaces()
    {
        try
        {
            var spaces = await _context.Space.ToListAsync();
            return new ResponseService<List<Space>>(
                spaces,
                spaces.Count > 0 ? "Spaces Loaded" : "No Spaces on db yet",
                spaces.Count > 0 ? true : false);
        }
        catch (Exception e)
        {
            return new ResponseService<List<Space>>(
                null,
                "Error " + e.Message,
                false);
        }
    }

    // Create a new space
    // This method first validate if there's any other space with the same name
    // If there's any with the same name, it will return an error message
    public async Task<ResponseService<Space>> CreateSpace(Space space)
    {
        try
        {
            var anySpace = await _context.Space.AnyAsync(s => s.Name.Trim() == space.Name.Trim());

            if (anySpace)
            {
                return new ResponseService<Space>(
                    space,
                    "Space already registered",
                    false
                );
            }

            await _context.AddAsync(space);
            await _context.SaveChangesAsync();
            return new ResponseService<Space>(
                space,
                "Space created",
                true
            );
        }
        catch (Exception e)
        {
            return new ResponseService<Space>(
                space,
                "Error " + e.Message,
                false
            );
        }
        
        
    }

    // Method to find a space by his ID
    // If not found, it will return an error message
    public async Task<ResponseService<Space>> FindSpaceById(int id)
    {
        try
        {
            var space = await _context.Space.FirstOrDefaultAsync(u => u.Id == id);

            if (space == null)
            {
                return new ResponseService<Space>(
                    null,
                    "Space not found",
                    false
                );
            }
        
            return new ResponseService<Space>(
                space,
                "Space found",
                true
            );
        }
        catch (Exception e)
        {
            return new ResponseService<Space>(
                null,
                "Error " + e.Message,
                false
            );
        }
    }
    
    // Method to update a space
    // It can update 1 or more parts of the space, not all is requested
    public async Task<ResponseService<Space>> UpdateSpace(Space newSpace)
    {
        try
        {
            var oldSpace = await _context.Space.FirstOrDefaultAsync(x => x.Id == newSpace.Id);
        
            if (oldSpace != null)
            {
                _context.Entry(oldSpace).CurrentValues.SetValues(newSpace);
                await _context.SaveChangesAsync();
                return new ResponseService<Space>(
                    newSpace,
                    "Space updated correctly",
                    true);
            }
        
            return new ResponseService<Space>(
                newSpace,
                "Space not found",
                false); 
        }
        catch (Exception e)
        {
            return new ResponseService<Space>(
                newSpace,
                "Error " + e.Message,
                false); 
        }
        
        
    }
    
    // Delete a space from the DB
    // This method first validate that the space exists, if so, it will be removed from the DB
    // If not found, it will return an error message
    public async Task<ResponseService<Space>> DeleteSpace(int id)
    {
        try
        {
            var oldSpace = await _context.Space.FirstOrDefaultAsync(x => x.Id == id);
        
            if (oldSpace != null)
            {
                _context.Remove(oldSpace);
                await _context.SaveChangesAsync();
                return new ResponseService<Space>(
                    oldSpace,
                    "Space removed",
                    true);
            }
        
            return new ResponseService<Space>(
                oldSpace,
                "Space not found",
                false);
        }
        catch (Exception e)
        {
            return new ResponseService<Space>(
                null,
                "Error " + e.Message,
                false);
        }
    }
}