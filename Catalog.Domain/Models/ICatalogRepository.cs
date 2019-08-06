using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.Models
{
    public interface ICatalogRepository
    {
        Task<List<CatalogType>> GetTypes();
        Task<List<CatalogBrand>> GetBrands();

        Task<long> GetItemsTotalCount();
        Task<List<CatalogItem>> GetItems(int pageSize = 6, int pageIndex = 0);
        Task<List<CatalogItem>> GetItems(string itemName, int pageSize = 6, int pageIndex = 0);
        Task<List<CatalogItem>> GetItems(int? catalogTypeId, int? catalogBrandId, int pageSize = 6, int pageIndex = 0);

        Task<CatalogItem> GetItemById(int itemId);

        CatalogItem CreateNewItem(CatalogItem item);
        CatalogItem UpdateItem(CatalogItem item);

        void DeleteItem(CatalogItem item);

        Task<bool> SaveChangesAsync();
    }
}
