using Eventra.Models;
using Eventra.Models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace Eventra.Services.Interfaces
{
    public interface IEventService
    {
        List<Event> GetEvents(int? categoryId, string? city, string? search, string? filter);
        EventDetailsViewModel? GetEventDetails(int id);
        Event CreateEvent(Event model, IFormFile? imageFile, int organizerId, string? role);
        List<Category> GetCategories();
        List<Event> GetOrganizedEvents(int organizerId);
        Event? GetEventForOrganizer(int eventId, int organizerId);
        void UpdateEvent(Event model, IFormFile? imageFile);
        void DeleteEvent(int eventId, int organizerId);
        string RegisterForEvent(int eventId, int userId);
        void CancelRegistration(int registrationId, int userId);
        void CancelWaitingList(int entryId, int userId);
        User? GetOrganizerUser(int userId);
        (string result, string? attendeeName, int? attendeeUserId) ProcessCheckIn(int eventId, int organizerId, string qrToken);
        List<EventCheckIn> GetCheckInsForEvent(int eventId);
        string ToggleFavorite(int eventId, int userId);
        HashSet<int> GetFavoriteEventIds(int userId);
        Dictionary<int, int> GetFavoriteCounts(List<int> eventIds);
    }
}
