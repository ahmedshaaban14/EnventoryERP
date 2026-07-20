using InventoryERP.Application.Interfaces;
using InventoryERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryERP.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IInventoryDbContext _context;

    public CustomerService(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Where(x => x.IsActive && x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Customer?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Where(x => x.IsActive && x.Code == code)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(customer.Code))
        {
            customer.Code = await GenerateCodeAsync(cancellationToken);
        }

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);
        return customer;
    }

    public async Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        var existing = await GetByIdAsync(customer.Id, cancellationToken);
        if (existing == null)
            throw new InvalidOperationException("Customer not found");

        existing.Name = customer.Name;
        existing.Phone = customer.Phone;
        existing.Address = customer.Address;
        existing.Car = customer.Car;
        existing.CreditLimit = customer.CreditLimit;
        existing.Notes = customer.Notes;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await GetByIdAsync(id, cancellationToken);
        if (customer != null)
        {
            customer.IsActive = false;
            customer.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<string> GenerateCodeAsync(CancellationToken cancellationToken)
    {
        var last = await _context.Customers
            .Where(x => x.Code.StartsWith("CUS-"))
            .OrderByDescending(x => x.Code)
            .Select(x => x.Code)
            .FirstOrDefaultAsync(cancellationToken);

        var number = 1;
        if (!string.IsNullOrWhiteSpace(last))
        {
            var numberStr = last.Replace("CUS-", string.Empty);
            if (int.TryParse(numberStr, out var n))
            {
                number = n + 1;
            }
        }

        return $"CUS-{number:D6}";
    }
}
