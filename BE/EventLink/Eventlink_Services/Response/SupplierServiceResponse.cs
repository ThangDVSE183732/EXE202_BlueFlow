using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Response
{
    public class SupplierServiceResponse
    {
        public Guid Id { get; set; }

        public Guid SupplierId { get; set; }

        public string ServiceName { get; set; }

        public string ServiceCategory { get; set; }

        public string Description { get; set; }

        public decimal? BasePrice { get; set; }

        public string PriceUnit { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string ServiceImages { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
