//using Domain.Models;
//using Infrastructure.Repositories;
//using System;
//using System.Collections.Generic;
//using Domain.Repositories;
//using System.Linq;
//using System.Threading.Tasks;
//using Domain.Models;
//using Domain.Repositories;
//using HotelDemo.Persistence;
//using Microsoft.EntityFrameworkCore;

//namespace Infrastructure.Repositories
//{
//    public class OfferRepository : GenericRepository<Offer>,IOfferRepository
//    {
//        private readonly ApplicationDbContext context;
//        private readonly DbSet<Offer> _genericRepository;

//        // instance from the constext
//        public OfferRepository(ApplicationDbContext context):base(context)
//        {
//            this.context = context;
//            _genericRepository = context.Set<Offer>();
//        }

//        public async Task<bool> AddAsync(Offer offer)
//        {
//            var res = await _genericRepository.AddAsync(offer);

//            return res!=null;
//        }

//        public async Task<bool> DeleteAsync(Offer offer)
//        {
//            //var res = await _genericRepository.Delete(offer.Id);
//            //return res;
//            return true;
//        }

//        // Done
//        public async Task<IEnumerable<Offer>> GetAllAsync(bool onlyActive = false)
//        {
//            //Try to add projection to load RoomOffers
//            //var query = await _genericRepository.GetAll();

//            //if (onlyActive)
//            //    query = query.Where(o => o.IsActive);

//            //return await query.ToListAsync();
//            return ;
//        }

//        public async Task<Offer?> GetByIdAsync(Guid id)
//        {
//            //Try to add projection to load RoomOffers
//            var query = await _genericRepository.GetbyId(id);
//            return await query.FirstOrDefaultAsync().ConfigureAwait(false);
//        }

//        public async Task<bool> UpdateAsync(Offer offer)
//        {

//            var res = await _genericRepository.
//                            UpdateIncludeAsync(offer, nameof(Offer.Title),
//                                                      nameof(Offer.Description),
//                                                      nameof(Offer.DiscountPercentage),
//                                                      nameof(Offer.StartDate),
//                                                      nameof(Offer.EndDate),
//                                                      nameof(Offer.IsActive));
//            return res;
//        }


//        public async Task<bool> RoomsExistAsync(IEnumerable<Guid> roomIds)
//        {

//            throw new NotImplementedException();
//        }

//        public async Task<bool> AnyOverlappingOfferAsync(IEnumerable<Guid> roomIds, DateTime start, DateTime end, Guid? excludeOfferId = null)
//        {
//            //var ids = roomIds?.ToList() ?? new List<Guid>();
//            //if (!ids.Any()) return false;

//            //var q = _context.RoomOffers
//            //    .Include(ro => ro.Offer)
//            //    .Where(ro => ids.Contains(ro.RoomId) && !ro.IsDeleted && ro.Offer.IsActive);

//            //if (excludeOfferId.HasValue)
//            //    q = q.Where(ro => ro.OfferId != excludeOfferId.Value);

//            //var overlapping = await q
//            //    .Select(ro => ro.Offer)
//            //    .Where(o => !(end <= o.StartDate || start >= o.EndDate))
//            //    .AnyAsync();

//            //return overlapping;
//            var ids = roomIds?.ToList() ?? new List<Guid>();
//            if (!ids.Any()) return false;
//            throw new NotImplementedException();
//        }
//    }
//}