using DataBase;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DeliveryRepository
{
    private readonly ApplicationDbContext _context;

    public DeliveryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Add a new delivery
    public async Task AddDeliveryAsync(Delivery delivery)
    {
        _context.Deliveries.Add(delivery);
        await _context.SaveChangesAsync();
    }

    // Get a delivery by its ID
    public async Task<Delivery?> GetDeliveryAsync(int id)
    {
        return await _context.Deliveries.FindAsync(id);
    }

    // Get all deliveries
    public async Task<List<Delivery>> GetAllDeliveriesAsync()
    {
        return await _context.Deliveries.ToListAsync();
    }

    // Update an existing delivery
    public async Task UpdateDeliveryAsync(Delivery delivery)
    {
        _context.Deliveries.Update(delivery);
        await _context.SaveChangesAsync();
    }

    // Delete a delivery by its ID
    public async Task DeleteDeliveryAsync(int id)
    {
        var delivery = await _context.Deliveries.FindAsync(id);
        if (delivery != null)
        {
            _context.Deliveries.Remove(delivery);
            await _context.SaveChangesAsync();
        }
    }
}