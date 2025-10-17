using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Request
{
    public class FindPartnerRequest
    {
        public class FindSupplierRequest
        {
            public decimal? Budget;
            public string? TargetAudience;
            public string? PackageType;
        }

        public class FindOrganizerRequest
        {
            public string? Category;
            public string? Location;
        }
    }
}
