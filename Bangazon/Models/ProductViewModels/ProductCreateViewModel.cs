using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Bangazon.Models.ProductViewModels
{
    public class ProductCreateViewModel
    {
        public Product product
        {
            get; set;
        }

        public List<SelectListItem> productTypes
        {
            get; set;
        } = new List<SelectListItem>();
    }
}
