using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Request
{
    public class SupplierServiceRequest
    {
        public class CreateSupplierService
        {
            public string ServiceName { get; set; }

            public string ServiceCategory { get; set; }

            public string Description { get; set; }

            public decimal? BasePrice { get; set; }

            public string PriceUnit { get; set; }

            public decimal? MinPrice { get; set; }

            public decimal? MaxPrice { get; set; }

            public string ServiceImages { get; set; }
        }

        public class UpdateSupplierService
        {
            public string ServiceName { get; set; }

            public string ServiceCategory { get; set; }

            public string Description { get; set; }

            public decimal? BasePrice { get; set; }

            public string PriceUnit { get; set; }

            public decimal? MinPrice { get; set; }

            public decimal? MaxPrice { get; set; }

            public string ServiceImages { get; set; }
        }
    }
}
