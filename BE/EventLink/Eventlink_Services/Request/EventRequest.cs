using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eventlink_Services.Request
{
    public class EventRequest
    {
        /// <summary>
        /// Create Event Request - JSON (ORIGINAL)
        /// </summary>
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

            // ✅ Category is optional now
            public string Category { get; set; }

            [Required(ErrorMessage = "Event type is required")]
            public string EventType { get; set; }

            public string TargetAudience { get; set; }
            public string RequiredServices { get; set; }
            public string SponsorshipNeeds { get; set; }
            public string SpecialRequirements { get; set; }

            // ✅ NEW FIELDS FOR OVERVIEW
            /// <summary>
            /// List of event highlights - "What to Expect"
            /// Example: ["50+ Expert Speakers", "Interactive Workshops", "Startup Showcase"]
            /// </summary>
            public List<string>? EventHighlights { get; set; }

            /// <summary>
            /// List of tags/keywords for the event
            /// Example: ["Artificial Intelligence", "Machine Learning", "Blockchain", "Innovation"]
            /// </summary>
            public List<string>? Tags { get; set; }

            /// <summary>
            /// List of target audience segments (replaces single TargetAudience string)
            /// Example: ["Tech executives & software developers", "Entrepreneurs & investors"]
            /// </summary>
            public List<string>? TargetAudienceList { get; set; }

            // Media
            public string? CoverImageUrl { get; set; }
            public List<string>? EventImages { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "View count must be non-negative")]
            public int? ViewCount { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "Interested count must be non-negative")]
            public int? InterestedCount { get; set; }
        }

        /// <summary>
        /// Update Event Request - JSON (ORIGINAL)
        /// </summary>
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

            // ✅ Category is optional now
            public string Category { get; set; }

            [Required(ErrorMessage = "Event type is required")]
            public string EventType { get; set; }

            public string TargetAudience { get; set; }
            public string RequiredServices { get; set; }
            public string SponsorshipNeeds { get; set; }
            public string SpecialRequirements { get; set; }

            // ✅ NEW FIELDS FOR OVERVIEW
            /// <summary>
            /// List of event highlights - "What to Expect"
            /// </summary>
            public List<string>? EventHighlights { get; set; }

            /// <summary>
            /// List of tags/keywords
            /// </summary>
            public List<string>? Tags { get; set; }

            /// <summary>
            /// List of target audience segments
            /// </summary>
            public List<string>? TargetAudienceList { get; set; }

            // Media
            public string? CoverImageUrl { get; set; }
            public List<string>? ExistingImages { get; set; }
            public List<string>? NewImages { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "View count must be non-negative")]
            public int? ViewCount { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "Interested count must be non-negative")]
            public int? InterestedCount { get; set; }
        }

        /// <summary>
        /// ✅ NEW: Create Event Request - FormData (FOR FILE UPLOAD)
        /// </summary>
        public class CreateEventFormRequest
        {
            [Required(ErrorMessage = "Title is required")]
            [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Description is required")]
            public string Description { get; set; }

            [StringLength(500, ErrorMessage = "Short description cannot exceed 500 characters")]
            public string? ShortDescription { get; set; }

            [Required(ErrorMessage = "Event date is required")]
            public DateTime? EventDate { get; set; }

            [Required(ErrorMessage = "End date is required")]
            public DateTime? EndDate { get; set; }

            [Required(ErrorMessage = "Location is required")]
            [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
            public string Location { get; set; }

            [StringLength(500, ErrorMessage = "Venue details cannot exceed 500 characters")]
            public string? VenueDetails { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Total budget must be a positive number")]
            public decimal? TotalBudget { get; set; }

            [Range(1, 100000, ErrorMessage = "Expected attendees must be at least 1")]
            public int? ExpectedAttendees { get; set; }

            public string? Category { get; set; }

            [Required(ErrorMessage = "Event type is required")]
            public string EventType { get; set; }

            public string? TargetAudience { get; set; }
            public string? RequiredServices { get; set; }
            public string? SponsorshipNeeds { get; set; }
            public string? SpecialRequirements { get; set; }

            // ✅ FormData - serialize as comma-separated string from frontend
            /// <summary>
            /// Comma-separated event highlights
            /// Example: "50+ Expert Speakers,Interactive Workshops,Startup Showcase"
            /// </summary>
            public string? EventHighlights { get; set; }

            /// <summary>
            /// Comma-separated tags
            /// Example: "AI,ML,Blockchain,Innovation"
            /// </summary>
            public string? Tags { get; set; }

            /// <summary>
            /// Comma-separated target audience list
            /// Example: "Tech executives,Entrepreneurs,Investors"
            /// </summary>
            public string? TargetAudienceList { get; set; }

            // ✅ File upload for cover image
            public IFormFile? CoverImage { get; set; }

            // ✅ Multiple image uploads
            public List<IFormFile>? EventImageFiles { get; set; }
        }

        /// <summary>
        /// ✅ NEW: Update Event Request - FormData (FOR FILE UPLOAD)
        /// </summary>
        public class UpdateEventFormRequest
        {
            [Required(ErrorMessage = "Title is required")]
            [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Description is required")]
            public string Description { get; set; }

            [StringLength(500, ErrorMessage = "Short description cannot exceed 500 characters")]
            public string? ShortDescription { get; set; }

            [Required(ErrorMessage = "Event date is required")]
            public DateTime? EventDate { get; set; }

            [Required(ErrorMessage = "End date is required")]
            public DateTime? EndDate { get; set; }

            [Required(ErrorMessage = "Location is required")]
            [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
            public string Location { get; set; }

            [StringLength(500, ErrorMessage = "Venue details cannot exceed 500 characters")]
            public string? VenueDetails { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Total budget must be a positive number")]
            public decimal? TotalBudget { get; set; }

            [Range(1, 100000, ErrorMessage = "Expected attendees must be at least 1")]
            public int? ExpectedAttendees { get; set; }

            public string? Category { get; set; }

            [Required(ErrorMessage = "Event type is required")]
            public string EventType { get; set; }

            public string? TargetAudience { get; set; }
            public string? RequiredServices { get; set; }
            public string? SponsorshipNeeds { get; set; }
            public string? SpecialRequirements { get; set; }

            // ✅ FormData - serialize as comma-separated string
            public string? EventHighlights { get; set; }
            public string? Tags { get; set; }
            public string? TargetAudienceList { get; set; }

            // ✅ File upload for cover image (optional - only if changing)
            public IFormFile? CoverImage { get; set; }

            /// <summary>
            /// Comma-separated existing image URLs to keep
            /// Example: "https://...,https://..."
            /// </summary>
            public string? ExistingImages { get; set; }

            // ✅ New image uploads
            public List<IFormFile>? EventImageFiles { get; set; }
        }

        /// <summary>
        /// Request DTO for updating event status (IsPublic and IsFeatured)
        /// </summary>
        public class UpdateEventStatusRequest
        {
            /// <summary>
            /// Set event visibility (null = no change)
            /// </summary>
            public bool? IsPublic { get; set; }

            /// <summary>
            /// Set featured status (null = no change)
            /// </summary>
            public bool? IsFeatured { get; set; }
        }
    }
}