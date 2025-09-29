using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Request
{
    public class EventRequest
    {
        public class CreateEventRequest
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string ShortDescription { get; set; }
            public DateTime? EventDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string Location { get; set; }
            public string VenueDetails { get; set; }
            public decimal? TotalBudget { get; set; }
            public int? ExpectedAttendees { get; set; }
            public string Category { get; set; }
            public string EventType { get; set; }
            public string TargetAudience { get; set; }
            public string RequiredServices { get; set; }
            public string SponsorshipNeeds { get; set; }
            public string SpecialRequirements { get; set; }
            public bool? IsFeatured { get; set; }
            public string CoverImageUrl { get; set; }
            public string EventImages { get; set; }
            public int? ViewCount { get; set; }
            public int? InterestedCount { get; set; }
        }
        public class UpdateEventRequest
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string ShortDescription { get; set; }
            public DateTime? EventDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string Location { get; set; }
            public string VenueDetails { get; set; }
            public decimal? TotalBudget { get; set; }
            public int? ExpectedAttendees { get; set; }
            public string Category { get; set; }
            public string EventType { get; set; }
            public string TargetAudience { get; set; }
            public string RequiredServices { get; set; }
            public string SponsorshipNeeds { get; set; }
            public string SpecialRequirements { get; set; }
            public bool? IsFeatured { get; set; }
            public string CoverImageUrl { get; set; }
            public string EventImages { get; set; }
            public int? ViewCount { get; set; }
            public int? InterestedCount { get; set; }
        }   
    }
}
