using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.ProductTypeViewModels
{
    public class ProductTypesViewModel
    {
        public ProductType ProductType { get; set; }
        public IEnumerable<ProductType> ProductTypes { get; set; }

        public GroupedProducts GroupedProducts { get; set; }
        public IEnumerable<GroupedProducts> TypeWithProducts { get; set; }
    }
}
