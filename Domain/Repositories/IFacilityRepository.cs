using Domain.Models;

namespace Domain.Repositories;

public interface IFacilityRepository
{
    IQueryable<Facility> GetAllFacilities();
    IQueryable<Facility> GetFacilityById(int facilityId);
    Task<bool> AddFacilityAsync(Facility facility);
    Task<bool> UpdateFacility(Facility facility);
    Task<bool> DeleteFacility(Facility facility);
}
