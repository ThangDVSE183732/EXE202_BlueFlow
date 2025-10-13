using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Request
{
    public class EventRequest
    {
        public class CreateEventRequest
        {
            [Required(ErrorMessage = "Title is required")]
            [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Description is required")]
            public string Description { get; set; }

            [StringLength(500, ErrorMessage = "Short description cannot exceed 500 characters")]
            public string ShortDescription { get; set; }

            [Required(ErrorMessage = "Event date is required")]
            public DateTime? EventDate { get; set; }

            [Required(ErrorMessage = "End date is required")]
            public DateTime? EndDate { get; set; }

            [Required(ErrorMessage = "Location is required")]
            [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
            public string Location { get; set; }

            [StringLength(500, ErrorMessage = "Venue details cannot exceed 500 characters")]
            public string VenueDetails { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Total budget must be a positive number")]
            public decimal? TotalBudget { get; set; }

            [Range(1, 100000, ErrorMessage = "Expected attendees must be at least 1")]
            public int? ExpectedAttendees { get; set; }

            [Required(ErrorMessage = "Category is required")]
            public string Category { get; set; }

            [Required(ErrorMessage = "Event type is required")]
            public string EventType { get; set; }

            public string TargetAudience { get; set; }
            public string RequiredServices { get; set; }
            public string SponsorshipNeeds { get; set; }
            public string SpecialRequirements { get; set; }

            //public bool? IsFeatured { get; set; }

            public IFormFile CoverImageUrl { get; set; }

            public List<IFormFile> EventImages { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "View count must be non-negative")]
            public int? ViewCount { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "Interested count must be non-negative")]
            public int? InterestedCount { get; set; }
        }
        public class UpdateEventRequest
        {
            [Required(ErrorMessage = "Title is required")]
            [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Description is required")]
            public string Description { get; set; }

            [StringLength(500, ErrorMessage = "Short description cannot exceed 500 characters")]
            public string ShortDescription { get; set; }

            [Required(ErrorMessage = "Event date is required")]
            public DateTime? EventDate { get; set; }

            [Required(ErrorMessage = "End date is required")]
            public DateTime? EndDate { get; set; }

            [Required(ErrorMessage = "Location is required")]
            [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
            public string Location { get; set; }

            [StringLength(500, ErrorMessage = "Venue details cannot exceed 500 characters")]
            public string VenueDetails { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Total budget must be a positive number")]
            public decimal? TotalBudget { get; set; }

            [Range(1, 100000, ErrorMessage = "Expected attendees must be at least 1")]
            public int? ExpectedAttendees { get; set; }

            [Required(ErrorMessage = "Category is required")]
            public string Category { get; set; }

            [Required(ErrorMessage = "Event type is required")]
            public string EventType { get; set; }

            public string TargetAudience { get; set; }
            public string RequiredServices { get; set; }
            public string SponsorshipNeeds { get; set; }
            public string SpecialRequirements { get; set; }

            //public bool? IsFeatured { get; set; }

            [Url(ErrorMessage = "Cover image must be a valid URL")]
            public string CoverImageUrl { get; set; }

            // ảnh cũ người dùng muốn giữ lại
            public List<string> ExistingImages { get; set; }

            // ảnh mới người dùng upload thêm
            public List<IFormFile> NewImages { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "View count must be non-negative")]
            public int? ViewCount { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "Interested count must be non-negative")]
            public int? InterestedCount { get; set; }
        }   
    }
}
