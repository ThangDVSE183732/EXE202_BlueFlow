using EventLink_Repositories.DBContext;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eventlink_Services.Service
{
    public class PartnershipMatchingService : IPartnershipMatchingService
    {
        private readonly EventLinkDBContext _context;
        private readonly ILogger<PartnershipMatchingService> _logger;

        public PartnershipMatchingService(
            EventLinkDBContext context,
            ILogger<PartnershipMatchingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get User Role from database
        /// </summary>
        public async Task<(User user, string role)> GetUserRoleAsync(Guid userId)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return (null, null);
            }

            // Role is stored directly in User table
            return (user, user.Role ?? "User");
        }

        /// <summary>
        /// Find Sponsor matches for Organizer
        /// </summary>
        public async Task<List<SponsorMatchResult>> FindSponsorMatchesAsync(Guid organizerId)
        {
            // 1. Get Organizer's events
            var events = await _context.Events
                .Where(e => e.OrganizerId == organizerId)
                .ToListAsync();

            if (!events.Any())
            {
                return new List<SponsorMatchResult>();
            }

            // 2. Get all active Sponsor partnerships with BrandProfiles
            var sponsorPartnerships = await _context.Partnerships
                .Where(p => p.PartnerType == "Sponsor" && 
                           (p.Status == "Pending" || p.Status == "Ongoing"))
                .Join(_context.Users,
                    p => p.PartnerId,
                    u => u.Id,
                    (p, u) => new { Partnership = p, User = u })
                .Join(_context.BrandProfiles,
                    pu => pu.User.Id,
                    bp => bp.UserId,
                    (pu, bp) => new { pu.Partnership, pu.User, BrandProfile = bp })
                .ToListAsync();

            if (!sponsorPartnerships.Any())
            {
                return new List<SponsorMatchResult>();
            }

            // 3. Calculate match scores
            var matches = new List<SponsorMatchResult>();

            foreach (var evt in events)
            {
                foreach (var sponsor in sponsorPartnerships)
                {
                    var match = CalculateMatchScore(evt, sponsor.Partnership, sponsor.BrandProfile);
                    if (match.Score >= 40) // Minimum threshold
                    {
                        matches.Add(match);
                    }
                }
            }

            // 4. Sort by score and return top 5
            return matches
                .OrderByDescending(m => m.Score)
                .Take(5)
                .ToList();
        }

        /// <summary>
        /// Find Organizer matches for Sponsor
        /// </summary>
        public async Task<List<OrganizerMatchResult>> FindOrganizerMatchesAsync(Guid sponsorId)
        {
            // 1. Get Sponsor's brand profile
            var brandProfile = await _context.BrandProfiles
                .Where(bp => bp.UserId == sponsorId)
                .FirstOrDefaultAsync();

            if (brandProfile == null)
            {
                return new List<OrganizerMatchResult>();
            }

            // 2. Get all active Organizer partnerships with Events
            var organizerPartnerships = await _context.Partnerships
                .Where(p => p.PartnerType == "Organizer" && 
                           (p.Status == "Pending" || p.Status == "Ongoing"))
                .Include(p => p.Event)
                .ToListAsync();

            if (!organizerPartnerships.Any())
            {
                return new List<OrganizerMatchResult>();
            }

            // 3. Calculate match scores
            var matches = new List<OrganizerMatchResult>();

            foreach (var partnership in organizerPartnerships)
            {
                if (partnership.Event == null) continue;

                var match = CalculateMatchScore(brandProfile, partnership, partnership.Event);
                if (match.Score >= 40) // Minimum threshold
                {
                    matches.Add(match);
                }
            }

            // 4. Sort by score and return top 5
            return matches
                .OrderByDescending(m => m.Score)
                .Take(5)
                .ToList();
        }

        /// <summary>
        /// Calculate match score between Event and Sponsor Partnership
        /// </summary>
        private SponsorMatchResult CalculateMatchScore(Event evt, Partnership partnership, BrandProfile brandProfile)
        {
            int score = 0;
            var reasons = new List<string>();

            // High Priority: Industry Match (30 points)
            if (!string.IsNullOrWhiteSpace(evt.Category) && !string.IsNullOrWhiteSpace(brandProfile.Industry))
            {
                var eventKeywords = evt.Category.ToLowerInvariant()
                    .Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(k => k.Length > 2)
                    .ToList();

                var sponsorKeywords = brandProfile.Industry.ToLowerInvariant()
                    .Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(k => k.Length > 2)
                    .ToList();

                var commonKeywords = eventKeywords.Intersect(sponsorKeywords).ToList();
                if (commonKeywords.Any())
                {
                    score += 30;
                    reasons.Add($"Ngành {brandProfile.Industry} khớp với loại sự kiện {evt.Category}");
                }
            }

            // High Priority: Tags/Keywords Match (30 points)
            var eventTags = (evt.Tags ?? "").ToLowerInvariant()
                .Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(t => t.Length > 2)
                .ToList();

            var sponsorTags = (brandProfile.Tags ?? "").ToLowerInvariant()
                .Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(t => t.Length > 2)
                .ToList();

            var commonTags = eventTags.Intersect(sponsorTags).ToList();
            if (commonTags.Any())
            {
                score += 30;
                reasons.Add($"Keywords \"{string.Join(", ", commonTags)}\" trùng khớp với focus sự kiện");
            }

            // Medium Priority: Budget Compatible (20 points)
            if (evt.TotalBudget.HasValue && evt.TotalBudget.Value > 0 && 
                partnership.ProposedBudget.HasValue && partnership.ProposedBudget.Value > 0)
            {
                var ratio = Math.Min(evt.TotalBudget.Value, partnership.ProposedBudget.Value) / 
                           Math.Max(evt.TotalBudget.Value, partnership.ProposedBudget.Value);
                if (ratio >= 0.7m)
                {
                    score += 20;
                    reasons.Add($"Ngân sách tương thích ({evt.TotalBudget.Value:N0} vs {partnership.ProposedBudget.Value:N0})");
                }
            }

            // Medium Priority: Location Match (20 points)
            if (!string.IsNullOrWhiteSpace(evt.Location) && !string.IsNullOrWhiteSpace(brandProfile.Location))
            {
                var eventLoc = evt.Location.ToLowerInvariant();
                var sponsorLoc = brandProfile.Location.ToLowerInvariant();
                if (eventLoc == sponsorLoc || eventLoc.Contains(sponsorLoc) || sponsorLoc.Contains(eventLoc))
                {
                    score += 20;
                    reasons.Add($"Cùng khu vực {brandProfile.Location}");
                }
            }

            // Low Priority: Semantic Match (10 points)
            var eventDesc = (evt.Description ?? "").ToLowerInvariant();
            var sponsorDesc = (brandProfile.OurMission ?? brandProfile.AboutUs ?? "").ToLowerInvariant();
            if (eventDesc.Length > 20 && sponsorDesc.Length > 20)
            {
                var eventWords = eventDesc.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length > 4)
                    .ToList();
                var sponsorWords = sponsorDesc.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length > 4)
                    .ToList();
                var commonWords = eventWords.Intersect(sponsorWords).ToList();
                if (commonWords.Count >= 3)
                {
                    score += 10;
                    reasons.Add("Mission/Description có themes tương tự");
                }
            }

            return new SponsorMatchResult
            {
                Partnership = partnership,
                BrandProfile = brandProfile,
                Event = evt,
                Score = score,
                Reasons = reasons,
                Stars = GetStars(score)
            };
        }

        /// <summary>
        /// Calculate match score between Brand Profile and Organizer Partnership
        /// </summary>
        private OrganizerMatchResult CalculateMatchScore(BrandProfile brandProfile, Partnership partnership, Event evt)
        {
            int score = 0;
            var reasons = new List<string>();

            // Similar logic but reversed
            // High Priority: Industry Match (30 points)
            if (!string.IsNullOrWhiteSpace(evt.Category) && !string.IsNullOrWhiteSpace(brandProfile.Industry))
            {
                var eventKeywords = evt.Category.ToLowerInvariant()
                    .Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(k => k.Length > 2)
                    .ToList();

                var sponsorKeywords = brandProfile.Industry.ToLowerInvariant()
                    .Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(k => k.Length > 2)
                    .ToList();

                var commonKeywords = eventKeywords.Intersect(sponsorKeywords).ToList();
                if (commonKeywords.Any())
                {
                    score += 30;
                    reasons.Add($"Sự kiện về {evt.Category} khớp với ngành {brandProfile.Industry} của bạn");
                }
            }

            // High Priority: Tags Match (30 points)
            var eventTags = (evt.Tags ?? "").ToLowerInvariant()
                .Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(t => t.Length > 2)
                .ToList();

            var sponsorTags = (brandProfile.Tags ?? "").ToLowerInvariant()
                .Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(t => t.Length > 2)
                .ToList();

            var commonTags = eventTags.Intersect(sponsorTags).ToList();
            if (commonTags.Any())
            {
                score += 30;
                reasons.Add($"Keywords \"{string.Join(", ", commonTags)}\" trong event match với tags \"{string.Join(", ", sponsorTags)}\"");
            }

            // Medium Priority: Budget Compatible (20 points)
            if (evt.TotalBudget.HasValue && evt.TotalBudget.Value > 0 && 
                partnership.ProposedBudget.HasValue && partnership.ProposedBudget.Value > 0)
            {
                var ratio = Math.Min(evt.TotalBudget.Value, partnership.ProposedBudget.Value) / 
                           Math.Max(evt.TotalBudget.Value, partnership.ProposedBudget.Value);
                if (ratio >= 0.7m)
                {
                    score += 20;
                    reasons.Add($"Ngân sách tương thích ({evt.TotalBudget.Value:N0} vs {partnership.ProposedBudget.Value:N0})");
                }
            }

            // Medium Priority: Location Match (20 points)
            if (!string.IsNullOrWhiteSpace(evt.Location) && !string.IsNullOrWhiteSpace(brandProfile.Location))
            {
                var eventLoc = evt.Location.ToLowerInvariant();
                var sponsorLoc = brandProfile.Location.ToLowerInvariant();
                if (eventLoc == sponsorLoc || eventLoc.Contains(sponsorLoc) || sponsorLoc.Contains(eventLoc))
                {
                    score += 20;
                    reasons.Add($"Cùng khu vực {evt.Location}");
                }
            }

            // Low Priority: Semantic Match (10 points)
            var eventDesc = (evt.Description ?? "").ToLowerInvariant();
            var sponsorDesc = (brandProfile.OurMission ?? brandProfile.AboutUs ?? "").ToLowerInvariant();
            if (eventDesc.Length > 20 && sponsorDesc.Length > 20)
            {
                var eventWords = eventDesc.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length > 4)
                    .ToList();
                var sponsorWords = sponsorDesc.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length > 4)
                    .ToList();
                var commonWords = eventWords.Intersect(sponsorWords).ToList();
                if (commonWords.Count >= 3)
                {
                    score += 10;
                    reasons.Add($"Mission của event align với {brandProfile.OurMission?.Substring(0, Math.Min(50, brandProfile.OurMission?.Length ?? 0))}...");
                }
            }

            return new OrganizerMatchResult
            {
                Partnership = partnership,
                Event = evt,
                Score = score,
                Reasons = reasons,
                Stars = GetStars(score)
            };
        }

        /// <summary>
        /// Get stars rating from score
        /// </summary>
        private string GetStars(int score)
        {
            if (score >= 80) return "⭐⭐⭐⭐⭐";
            if (score >= 60) return "⭐⭐⭐⭐";
            if (score >= 40) return "⭐⭐⭐";
            return "⭐⭐";
        }
    }

    // DTOs
    public class SponsorMatchResult
    {
        public Partnership Partnership { get; set; }
        public BrandProfile BrandProfile { get; set; }
        public Event Event { get; set; }
        public int Score { get; set; }
        public List<string> Reasons { get; set; }
        public string Stars { get; set; }
    }

    public class OrganizerMatchResult
    {
        public Partnership Partnership { get; set; }
        public Event Event { get; set; }
        public int Score { get; set; }
        public List<string> Reasons { get; set; }
        public string Stars { get; set; }
    }
}

