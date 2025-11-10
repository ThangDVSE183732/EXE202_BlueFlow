using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Eventlink_Services.Request.PartnershipRequest;

namespace Eventlink_Services.Service
{
    public class PartnershipService : IPartnershipService
    {
        private readonly IPartnershipRepository _partnershipRepository;
        private readonly IUserRepository _userRepository;
        private readonly CloudinaryService _cloudinaryService;

        public PartnershipService(
            IPartnershipRepository partnershipRepository, 
            IUserRepository userRepository,
            CloudinaryService cloudinaryService)
        {
            _partnershipRepository = partnershipRepository;
            _userRepository = userRepository;
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Create partnership from FormData with file upload
        /// </summary>
        public async Task<Partnership> CreateAsync(Guid organizerId, CreatePartnershipFormRequest request)
        {

            var partnership = new Partnership
            {
                Id = Guid.NewGuid(),
                EventId = request.EventId,
                PartnerId = request.PartnerId,
                PartnerType = request.PartnerType,
                InitialMessage = request.InitialMessage,
                ProposedBudget = request.ProposedBudget,
                ServiceDescription = request.ServiceDescription,
                PreferredContactMethod = request.PreferredContactMethod,
                OrganizerContactInfo = request.OrganizerContactInfo,
                StartDate = request.StartDate,
                DeadlineDate = request.DeadlineDate,
                PartnershipImage = request.PartnershipImageFile,
                Status = PartnershipStatus.Ongoing,
                IsMark = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _partnershipRepository.AddAsync(partnership);
            return partnership;
        }

        /// <summary>
        /// Create partnership from JSON request (backward compatibility)
        /// </summary>
        public async Task<Partnership> CreateAsync(Guid organizerId, CreatePartnershipRequest request)
        {
            var partnership = new Partnership
            {
                Id = Guid.NewGuid(),
                EventId = request.EventId,
                PartnerId = request.PartnerId,
                PartnerType = request.PartnerType,
                InitialMessage = request.InitialMessage,
                ProposedBudget = request.ProposedBudget,
                ServiceDescription = request.ServiceDescription,
                PreferredContactMethod = request.PreferredContactMethod,
                OrganizerContactInfo = request.OrganizerContactInfo,
                StartDate = request.StartDate,
                DeadlineDate = request.DeadlineDate,
                PartnershipImage = request.PartnershipImage,
                IsMark = false,
                Status = PartnershipStatus.Ongoing,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _partnershipRepository.AddAsync(partnership);
            return partnership;
        }

        /// <summary>
        /// Get all partnerships in the system with event and partner details
        /// </summary>
        public async Task<IEnumerable<PartnershipResponse>> GetAllPartnershipsAsync()
        {
            var partnerships = await _partnershipRepository.GetAllPartnershipsAsync();

            return partnerships.Select(p => MapToPartnershipResponse(p));
        }

        /// <summary>
        /// Get all partnerships with event and partner details for a specific event
        /// </summary>
        public async Task<IEnumerable<PartnershipResponse>> GetPartnershipsByEventAsync(Guid eventId)
        {
            var partnerships = await _partnershipRepository.GetPartnershipsByEventAsync(eventId);

            return partnerships.Select(p => MapToPartnershipResponse(p));
        }

        public async Task UpdateAsync(Guid partnershipId, UpdatePartnershipRequest request)
        {
            var partnership = await _partnershipRepository.GetByIdAsync(partnershipId);

            if (partnership == null)
                throw new Exception("Partnership not found.");

            // Update fields
            if (request.AgreedBudget.HasValue)
                partnership.AgreedBudget = request.AgreedBudget;
            
            if (!string.IsNullOrEmpty(request.OrganizerNotes))
                partnership.OrganizerNotes = request.OrganizerNotes;
            
            // ✅ Update PartnershipImage
            if (!string.IsNullOrEmpty(request.PartnershipImage))
                partnership.PartnershipImage = request.PartnershipImage;
            
            // ✅ CHANGED: Update IsMark instead of SharedNotes
            if (request.IsMark.HasValue)
                partnership.IsMark = request.IsMark;
            
            if (request.CompletionDate.HasValue)
                partnership.CompletionDate = request.CompletionDate;

            partnership.UpdatedAt = DateTime.UtcNow;

            await _partnershipRepository.UpdateAsync(partnership);
        }

        public async Task<Partnership> UpdateStatusAsync(Guid partnershipId, UpdatePartnershipStatusRequest request)
        {
            var partnership = await _partnershipRepository.GetByIdAsync(partnershipId);
            if (partnership == null)
                throw new Exception("Partnership not found.");

            // ✅ Validate all 4 statuses
            var validStatuses = new[] { 
                PartnershipStatus.Pending, 
                PartnershipStatus.Ongoing,
                PartnershipStatus.Completed,
                PartnershipStatus.Cancelled
            };
            
            if (!validStatuses.Contains(request.Status))
            {
                throw new ArgumentException($"Invalid status. Allowed values: {string.Join(", ", validStatuses)}");
            }

            // ✅ REMOVED: Status transition validation - Allow free status changes
            // User can change status freely between all statuses

            partnership.Status = request.Status;
            partnership.OrganizerResponse = request.OrganizerResponse;
            partnership.UpdatedAt = DateTime.UtcNow;

            // ✅ Update PartnershipImage if provided
            if (!string.IsNullOrEmpty(request.PartnershipImage))
                partnership.PartnershipImage = request.PartnershipImage;

            // ✅ Handle status-specific logic
            switch (request.Status)
            {
                case PartnershipStatus.Ongoing:
                    // When partnership moves to Ongoing (accepted)
                    partnership.AgreedBudget = partnership.ProposedBudget;
                    partnership.StartDate = partnership.StartDate ?? DateTime.UtcNow;
                    
                    var partner = await _userRepository.GetActiveUserByIdAsync(partnership.PartnerId);
                    if(partner != null)
                    {
                        partnership.PartnerContactInfo = partner.Email;
                    }
                    break;
                    
                case PartnershipStatus.Completed:
                    // Mark completion date
                    partnership.CompletionDate = DateTime.UtcNow;
                    break;
                    
                case PartnershipStatus.Cancelled:
                    // Log cancellation in OrganizerResponse if not provided
                    if (string.IsNullOrEmpty(partnership.OrganizerResponse))
                    {
                        partnership.OrganizerResponse = "Partnership cancelled";
                    }
                    break;
                    
                case PartnershipStatus.Pending:
                    // Reset to pending - clear some fields if needed
                    // partnership.AgreedBudget = null; // Optional: reset fields
                    break;
            }

            await _partnershipRepository.UpdateAsync(partnership);
            return partnership;
        }

        /// <summary>
        /// Toggle partnership status between Ongoing and Pending by EventId
        /// ✅ MINIMAL: Only updates Status field - NO UpdatedAt, NO side effects
        /// </summary>
        public async Task<Partnership> TogglePartnershipStatusByEventAsync(Guid eventId)
        {
            var partnerships = await _partnershipRepository.GetPartnershipsByEventAsync(eventId);
            var partnership = partnerships.FirstOrDefault();

            if (partnership == null)
                throw new Exception("No partnership found for this event.");

            // ✅ Toggle logic: Ongoing ↔ Pending
            var currentStatus = partnership.Status ?? PartnershipStatus.Pending;
            var newStatus = currentStatus == PartnershipStatus.Ongoing 
                ? PartnershipStatus.Pending 
                : PartnershipStatus.Ongoing;

            // ✅ ONLY update Status - NOTHING ELSE
            partnership.Status = newStatus;

            await _partnershipRepository.UpdateAsync(partnership);
            return partnership;
        }

        /// <summary>
        /// Toggle partnership status between Ongoing and Pending by PartnerId
        /// ✅ NEW: Toggle status for a specific partner's partnership
        /// </summary>
        public async Task<Partnership> TogglePartnershipStatusByPartnerAsync(Guid partnerId)
        {
            var partnerships = await _partnershipRepository.GetPartnershipsByPartnerAsync(partnerId);
            var partnership = partnerships.FirstOrDefault();

            if (partnership == null)
                throw new Exception("No partnership found for this partner.");

            // ✅ Toggle logic: Ongoing ↔ Pending
            var currentStatus = partnership.Status ?? PartnershipStatus.Pending;
            var newStatus = currentStatus == PartnershipStatus.Ongoing 
                ? PartnershipStatus.Pending 
                : PartnershipStatus.Ongoing;

            // ✅ ONLY update Status
            partnership.Status = newStatus;

            await _partnershipRepository.UpdateAsync(partnership);
            return partnership;
        }

        /// <summary>
        /// Validate status transition rules (DISABLED - Allow free transitions)
        /// </summary>
        private void ValidateStatusTransition(string currentStatus, string newStatus)
        {
            // ✅ DISABLED: Allow free status transitions
            // No validation - users can change status freely
            return;
        }

        /// <summary>
        /// Get all partnerships without an assigned event (EventId is null)
        /// ✅ NEW: For partnerships marketplace - not yet assigned to any event
        /// </summary>
        public async Task<IEnumerable<PartnershipResponse>> GetUnassignedPartnershipsAsync()
        {
            var partnerships = await _partnershipRepository.GetUnassignedPartnershipsAsync();

            return partnerships.Select(p => MapToPartnershipResponse(p));
        }

        /// <summary>
        /// Assign a partnership to an event
        /// ✅ NEW: Organizer can assign unassigned partnership to their event
        /// </summary>
        public async Task<Partnership> AssignPartnershipToEventAsync(Guid partnershipId, Guid eventId)
        {
            var partnership = await _partnershipRepository.GetByIdAsync(partnershipId);
            if (partnership == null)
                throw new Exception("Partnership not found.");

            // ✅ Assign event to partnership
            partnership.EventId = eventId;
            partnership.UpdatedAt = DateTime.UtcNow;

            await _partnershipRepository.UpdateAsync(partnership);
            return partnership;
        }

        #region Helper Methods

        /// <summary>
        /// Map Partnership entity to PartnershipResponse DTO
        /// </summary>
        private PartnershipResponse MapToPartnershipResponse(Partnership p)
        {
            return new PartnershipResponse
            {
                // Partnership Info
                Id = p.Id,
                EventId = p.EventId,
                PartnerId = p.PartnerId,
                PartnerType = p.PartnerType,
                InitialMessage = p.InitialMessage,
                OrganizerResponse = p.OrganizerResponse,
                ProposedBudget = p.ProposedBudget,
                AgreedBudget = p.AgreedBudget,
                ServiceDescription = p.ServiceDescription,
                Status = p.Status,
                OrganizerContactInfo = p.OrganizerContactInfo,
                PartnerContactInfo = p.PartnerContactInfo,
                PreferredContactMethod = p.PreferredContactMethod,
                ExternalWorkspaceUrl = p.ExternalWorkspaceUrl,
                StartDate = p.StartDate,
                DeadlineDate = p.DeadlineDate,
                CompletionDate = p.CompletionDate,
                OrganizerNotes = p.OrganizerNotes,
                PartnershipImage = p.PartnershipImage,
                IsMark = p.IsMark,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,

                // ✅ Event Info - ALL FIELDS
                Event = p.Event != null ? new EventBasicInfo
                {
                    Id = p.Event.Id,
                    OrganizerId = p.Event.OrganizerId,
                    
                    // Basic Info
                    Title = p.Event.Title,
                    Description = p.Event.Description,
                    ShortDescription = p.Event.ShortDescription,
                    EventDate = p.Event.EventDate,
                    EndDate = p.Event.EndDate,
                    Location = p.Event.Location,
                    VenueDetails = p.Event.VenueDetails,
                    
                    // Budget & Attendees
                    TotalBudget = p.Event.TotalBudget,
                    ExpectedAttendees = p.Event.ExpectedAttendees,
                    
                    // Classification
                    Category = p.Event.Category,
                    EventType = p.Event.EventType,
                    TargetAudience = p.Event.TargetAudience,
                    
                    // Requirements
                    RequiredServices = p.Event.RequiredServices,
                    SponsorshipNeeds = p.Event.SponsorshipNeeds,
                    SpecialRequirements = p.Event.SpecialRequirements,
                    
                    // Status & Visibility
                    Status = p.Event.Status,
                    IsPublic = p.Event.IsPublic,
                    IsFeatured = p.Event.IsFeatured,
                    
                    // Media
                    CoverImageUrl = p.Event.CoverImageUrl,
                    EventImages = p.Event.EventImages,
                    
                    // Engagement Metrics
                    ViewCount = p.Event.ViewCount,
                    InterestedCount = p.Event.InterestedCount,
                    AverageRating = p.Event.AverageRating,
                    ReviewCount = p.Event.ReviewCount,
                    
                    // Overview Fields (JSON arrays)
                    EventHighlights = p.Event.EventHighlights,
                    Tags = p.Event.Tags,
                    TargetAudienceList = p.Event.TargetAudienceList,
                    
                    // Timestamps
                    CreatedAt = p.Event.CreatedAt,
                    UpdatedAt = p.Event.UpdatedAt
                } : null,

                // Partner Info - ✅ Simplified to only include Id, FullName, AvatarUrl + BrandProfile
                Partner = p.Partner != null ? new PartnerBasicInfo
                {
                    Id = p.Partner.Id,
                    FullName = p.Partner.FullName,
                    AvatarUrl = p.Partner.AvatarUrl,
                    // ✅ NEW: Map BrandProfile if exists
                    BrandProfile = p.Partner.BrandProfiles != null && p.Partner.BrandProfiles.Any()
                        ? new BrandProfileBasicInfo
                        {
                            Id = p.Partner.BrandProfiles.First().Id,
                            BrandName = p.Partner.BrandProfiles.First().BrandName,
                            BrandLogo = p.Partner.BrandProfiles.First().BrandLogo,
                            Industry = p.Partner.BrandProfiles.First().Industry,
                            Location = p.Partner.BrandProfiles.First().Location,
                            IsPublic = p.Partner.BrandProfiles.First().IsPublic,
                            HasPartnership = p.Partner.BrandProfiles.First().HasPartnership
                        }
                        : null
                } : null
            };
        }

        #endregion
    }
}
