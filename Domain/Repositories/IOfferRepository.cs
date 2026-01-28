//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Domain.Models;

//namespace Domain.Repositories
//{
//    public interface IOfferRepository:IGenericRepository<Offer>
//    {
//        Task<bool> AddAsync(Offer offer);
//        Task<Offer?> GetByIdAsync(Guid id);
//        Task<IEnumerable<Offer>> GetAllAsync(bool onlyActive = false);
//        Task<bool> UpdateAsync(Offer offer);
//        Task<bool> DeleteAsync(Offer offer);

//        Task<bool> RoomsExistAsync(IEnumerable<Guid> roomIds);
//        Task<bool> AnyOverlappingOfferAsync(IEnumerable<Guid> roomIds, DateTime start, DateTime end, Guid? excludeOfferId = null);
//    }
//}