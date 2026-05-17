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
        User? GetOrganizerUser(int userId);
    }
}
