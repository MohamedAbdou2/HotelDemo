using Domain.Repositories;

namespace Application.Helper
{
    public class CustomerContext
    {
        private readonly CurrentUser _currentUser;
        private readonly IGenericRepository<Domain.Models.Customer> _customerRepo;

        public CustomerContext(CurrentUser currentUser, IGenericRepository<Domain.Models.Customer> customerRepo)
        {
            _currentUser = currentUser;
            _customerRepo = customerRepo;
        }

        public Guid GetCustomerId()
        {
            var userId = _currentUser.GetUserId();
            return _customerRepo.GetAll(x => x.UserId == userId)
                                .Select(x => x.Id)
                                .FirstOrDefault();
        }
    }
}
