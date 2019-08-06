 
using Microsoft.AspNetCore.Mvc.Rendering;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMvc.Models;

namespace WebMvc.ViewModels
{
    public class CatalogIndexViewModel
    {
        public IEnumerable<CatalogItemDto> CatalogItems { get; set; }
        public IEnumerable<SelectListItem> Brands { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }
        public int? BrandIdApplied { get; set; }
        public int? TypesIdApplied { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}
