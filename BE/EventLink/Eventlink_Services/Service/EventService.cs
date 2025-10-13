using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static EventLink_Repositories.Models.Event;
using static Eventlink_Services.Request.EventRequest;

namespace Eventlink_Services.Service
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly CloudinaryService _cloudinaryService;
        public EventService(IEventRepository eventRepository, CloudinaryService cloudinaryService)
        {
            _eventRepository = eventRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task Create(Guid userId, CreateEventRequest request)
        {
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                OrganizerId = userId,
                Title = request.Title,
                Description = request.Description,
                Location = request.Location,
                EventDate = request.EventDate,
                EndDate = request.EndDate,
                EventType = request.EventType,
                IsPublic = true,
                IsFeatured = true,
                Category = request.Category,
                ExpectedAttendees = request.ExpectedAttendees,
                TotalBudget = request.TotalBudget,
                Status = EventStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // ✅ Upload ảnh bìa (cover)
            if (request.CoverImageUrl != null)
            {
                var coverUrl = await _cloudinaryService.UploadImageAsync(request.CoverImageUrl);
                newEvent.CoverImageUrl = coverUrl; // Lưu URL vào DB
            }

            if (request.EventImages != null && request.EventImages.Any())
            {
                var urls = new List<string>();
                foreach (var image in request.EventImages)
                {
                    var url = await _cloudinaryService.UploadImageAsync(image);
                    urls.Add(url);
                }

                // Lưu danh sách URL (serialize sang JSON)
                newEvent.EventImages = JsonSerializer.Serialize(urls);
            }

            await _eventRepository.AddAsync(newEvent);
        }

        //public Task<List<EventResponse>> GetActiveEventsAsync()
        //{
        //    return _eventRepository.GetActiveEventsAsync();
        //}

        public async Task<List<EventResponse>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return events.Select(e => new EventResponse
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Location = e.Location,
                EventDate = e.EventDate,
                EndDate = e.EndDate,
                EventType = e.EventType,
                IsPublic = e.IsPublic,
                Status = e.Status,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).ToList();
        }

        public async Task<EventResponse> GetEventByIdAsync(Guid id)
        {
            var @event = await _eventRepository.GetEventByIdAsync(id);
            return new EventResponse
            {
                Id = @event.Id,
                Title = @event.Title,
                Description = @event.Description,
                Location = @event.Location,
                EventDate = @event.EventDate,
                EndDate = @event.EndDate,
                EventType = @event.EventType,
                IsPublic = @event.IsPublic,
                Status = @event.Status,
                CreatedAt = @event.CreatedAt,
                UpdatedAt = @event.UpdatedAt
            };
        }

        public async Task<Event> GetEventById(Guid id)
        {
            return await _eventRepository.GetEventByIdAsync(id);
        }

        public async Task<List<EventResponse>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var events = await _eventRepository.GetEventsByDateRangeAsync(startDate, endDate);
            return events.Select(e => new EventResponse
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Location = e.Location,
                EventDate = e.EventDate,
                EndDate = e.EndDate,
                EventType = e.EventType,
                IsPublic = e.IsPublic,
                Status = e.Status,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).ToList();
        }

        public async Task<List<EventResponse>> GetEventsByLocationAsync(string location)
        {
            var events = await _eventRepository.GetEventsByLocationAsync(location);
            return events.Select(e => new EventResponse
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Location = e.Location,
                EventDate = e.EventDate,
                EndDate = e.EndDate,
                EventType = e.EventType,
                IsPublic = e.IsPublic,
                Status = e.Status,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).ToList();
        }

        public async Task<List<EventResponse>> GetEventsByOrganizerIdAsync(Guid organizerId)
        {
            var events = await _eventRepository.GetEventsByOrganizerIdAsync(organizerId);
            return events.Select(e => new EventResponse
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Location = e.Location,
                EventDate = e.EventDate,
                EndDate = e.EndDate,
                EventType = e.EventType,
                IsPublic = e.IsPublic,
                Status = e.Status,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).ToList();
        }

        public Task<List<EventResponse>> GetEventsByTypeAsync(string eventType)
        {
            var events =  _eventRepository.GetEventsByTypeAsync(eventType);
            return events.ContinueWith(t => t.Result.Select(e => new EventResponse
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Location = e.Location,
                EventDate = e.EventDate,
                EndDate = e.EndDate,
                EventType = e.EventType,
                IsPublic = e.IsPublic,
                Status = e.Status,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).ToList());
        }

        public void Remove(Event @event)
        {
            _eventRepository.Remove(@event);
        }

        public async Task<List<EventResponse>> SearchEvents(string name, string location, DateTime? startDate, DateTime? endDate, string eventType)
        {
            var events = await _eventRepository.SearchEvents(name, location, startDate, endDate, eventType);
            return events.Select(e => new EventResponse
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Location = e.Location,
                EventDate = e.EventDate,
                EndDate = e.EndDate,
                EventType = e.EventType,
                IsPublic = e.IsPublic,
                Status = e.Status,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).ToList();
        }

        public async Task Update(Guid id, UpdateEventRequest request)
        {
            var existingEvent = await _eventRepository.GetEventByIdAsync(id);
            if (existingEvent == null)
                throw new Exception("Event not found or access denied.");

            // --- Cập nhật thông tin cơ bản ---
            existingEvent.Title = request.Title;
            existingEvent.Description = request.Description;
            existingEvent.Location = request.Location;
            existingEvent.EventDate = request.EventDate;
            existingEvent.EndDate = request.EndDate;
            existingEvent.EventType = request.EventType;
            existingEvent.UpdatedAt = DateTime.UtcNow;

            // --- Lấy danh sách ảnh hiện tại từ DB ---
            var existingImages = new List<string>();
            if (!string.IsNullOrEmpty(existingEvent.EventImages))
            {
                try
                {
                    existingImages = JsonSerializer.Deserialize<List<string>>(existingEvent.EventImages) ?? new List<string>();
                }
                catch
                {
                    // fallback nếu dữ liệu cũ không phải JSON hợp lệ
                    existingImages = existingEvent.EventImages.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                }
            }

            // --- Nếu có ảnh mới upload ---
            if (request.NewImages != null && request.NewImages.Any())
            {
                foreach (var image in request.NewImages)
                {
                    var newUrl = await _cloudinaryService.UploadImageAsync(image);
                    existingImages.Add(newUrl); // chỉ thêm mới, không xóa cái cũ
                }
            }

            // --- Nếu người dùng chọn xóa ảnh cũ ---
            if (request.ExistingImages != null && request.ExistingImages.Any())
            {
                existingImages = existingImages
                    .Where(img => request.ExistingImages.Contains(img))
                    .ToList();
            }

            // --- Ghi lại danh sách ảnh ---
            existingEvent.EventImages = JsonSerializer.Serialize(existingImages);

            _eventRepository.Update(existingEvent);
        }

        public async Task UpdateStatus(Guid id, string status)
        {
            var existingEvent = await _eventRepository.GetEventByIdAsync(id);
            if (existingEvent == null)
                throw new Exception("Event not found or access denied.");
            existingEvent.Status = status;
            _eventRepository.Update(existingEvent);
        }
    }
}
