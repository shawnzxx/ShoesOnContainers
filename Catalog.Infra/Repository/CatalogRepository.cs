using Catalog.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infra.Repository
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly CatalogContext _dbContext;

        public CatalogRepository(CatalogContext dbContext)
        {
            _dbContext = dbContext;
            //https://github.com/aspnet/EntityFrameworkCore/issues/7064
            //((DbContext)dbContext).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public CatalogItem CreateNewItem(CatalogItem item)
        {
            var dbSet = _dbContext.Catalogs.Add(item);
            return dbSet.Entity;
        }

        public void DeleteItem(CatalogItem item)
        {
            _dbContext.Catalogs.Remove(item);
        }

        public async Task<List<CatalogBrand>> GetBrands()
        {
            return await _dbContext.CatalogBrands.ToListAsync();
        }

        public async Task<CatalogItem> GetItemById(int itemId)
        {
            return await _dbContext.Catalogs.SingleOrDefaultAsync(c => c.Id == itemId);
        }

        public async Task<long> GetItemsTotalCount()
        {
            return await _dbContext.Catalogs.LongCountAsync();
        }

        public async Task<List<CatalogItem>> GetItems(int pageSize = 6, int pageIndex = 0)
        {
            var items = await _dbContext.Catalogs.OrderBy(o => o.Name)
                                                     .Skip(pageSize * pageIndex)
                                                     .Take(pageSize)
                                                     .ToListAsync();
            return items;
        }

        public async Task<List<CatalogItem>> GetItems(string itemName, int pageSize = 6, int pageIndex = 0)
        {
            var items = await _dbContext.Catalogs.Where(c => c.Name.StartsWith(itemName))
                                          .OrderBy(o => o.Name)
                                          .Skip(pageSize * pageIndex)
                                          .Take(pageSize)
                                          .ToListAsync();
            return items;
        }

        public async Task<List<CatalogItem>> GetItems(int? catalogTypeId, int? catalogBrandId, int pageSize = 6, int pageIndex = 0)
        {
            //we are not going to database yet, until hit the first where
            //https://stackoverflow.com/questions/1578778/using-iqueryable-with-linq
            var catalogs = (IQueryable<CatalogItem>)_dbContext.Catalogs;

            if (catalogTypeId.HasValue)
            {
                catalogs = catalogs.Where(r => r.CatalogTypeId == catalogTypeId);
            }

            if (catalogBrandId.HasValue)
            {
                catalogs = catalogs.Where(r => r.CatalogBrandId == catalogBrandId);
            }
            
            var items = await catalogs.OrderBy(o => o.Name)
                                        .Skip(pageSize * pageIndex)
                                        .Take(pageSize)
                                        .ToListAsync();
            return items;
        }

        public async Task<List<CatalogType>> GetTypes()
        {
            return await _dbContext.CatalogTypes.ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            // return true if 1 or more entities were changed
            return (await _dbContext.SaveChangesAsync() > 0);
        }

        public CatalogItem UpdateItem(CatalogItem item)
        {
            var dbSet = _dbContext.Catalogs.Update(item);
            return dbSet.Entity;
        }
    }
}
