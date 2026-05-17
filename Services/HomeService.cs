using Eventra.Models.ViewModels;
using Eventra.Repositories.Interfaces;
using Eventra.Services.Interfaces;

namespace Eventra.Services
{
    public class HomeService : IHomeService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IReviewRepository _reviewRepository;

        public HomeService(IEventRepository eventRepository, IReviewRepository reviewRepository)
        {
            _eventRepository = eventRepository;
            _reviewRepository = reviewRepository;
        }

        public HomeViewModel GetHomeViewModel()
        {
            return new HomeViewModel
            {
                FeaturedEvents = _eventRepository
                    .QueryApproved()
                    .Where(e => e.EventDate.Date >= DateTime.Today)
                    .OrderByDescending(e => e.CreatedAt)
                    .Take(6)
                    .ToList(),
                LatestReviews = _reviewRepository
                    .QueryApproved()
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(3)
                    .ToList()
            };
        }
    }
}
