using Domain.Models;
using Domain.Repositories;
using HotelDemo.Persistence;

namespace Infrastructure.Repositories;

public class FacilityRepository : IFacilityRepository
{
    private readonly ApplicationDbContext _context;

    public FacilityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddFacilityAsync(Facility facility)
    {
        await _context.Facilities.AddAsync(facility);
        return await _context.SaveChangesAsync() > 0;
    }


    public IQueryable<Facility> GetAllFacilities()
    {
        return _context.Facilities;
    }

    public IQueryable<Facility> GetFacilityById(int facilityId)
    {
        return _context.Facilities.Where(f => facilityId == (int)f.Id);
    }

    public async Task<bool> UpdateFacility(Facility facility)
    {
        _context.Update(facility);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteFacility(Facility facility)
    {
        _context.Facilities.Remove(facility);
        return await _context.SaveChangesAsync() > 0;
    }

}
