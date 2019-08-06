using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebMvc.Models;

namespace WebMvc.Services
{
    public interface ICatalogService
    {
        Task<CatalogDto> GetCatalogItems(int page, int take, int? brand, int? type);

        Task<IEnumerable<SelectListItem>> GetBrands();

        Task<IEnumerable<SelectListItem>> GetTypes();
    }
}
