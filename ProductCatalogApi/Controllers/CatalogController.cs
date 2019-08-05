using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProductCatalogApi.Data;
using ProductCatalogApi.Domain;
using ProductCatalogApi.ViewModels;

namespace ProductCatalogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _catalogContext;
        private readonly IOptionsSnapshot<CatalogSettings> _settings;

        public CatalogController(CatalogContext catalogContext, IOptionsSnapshot<CatalogSettings> settings)
        {
            _catalogContext = catalogContext;
            _settings = settings;
        }

        // GET api/catalog/types
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Types()
        {
            var items = await _catalogContext.CatalogTypes.ToListAsync();
            return Ok(items);
        }

        // GET api/catalog/brands
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Brands()
        {
            var items = await _catalogContext.CatalogBrands.ToListAsync();
            return Ok(items);
        }

        // GET api/catalog/items/5
        [HttpGet]
        [Route("items/{id}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var item = await _catalogContext.Catalogs.SingleOrDefaultAsync(c => c.Id == id);
            if (item != null)
            {
                item.PictureUrl = item.PictureUrl.Replace("http://externalcatalogbaseurltobereplaced", _settings.Value.ExternalCatalogBaseUrl);
                return Ok(item);
            }
            return NotFound();
        }

        // GET api/catalog/items?pageSize=4&pageIndex=2
        [HttpGet]
        [Route("items")]
        public async Task<IActionResult> GetItems(int pageSize = 6, int pageIndex = 0)
        {
            var totalCount = await _catalogContext.Catalogs.LongCountAsync();
            var itemsOnPage = await _catalogContext.Catalogs
                                                     .OrderBy(o => o.Name)
                                                     .Skip(pageSize * pageIndex)
                                                     .Take(pageSize)
                                                     .ToListAsync();

            itemsOnPage = ChangeUrlPlaceHolder(itemsOnPage);
            var model = new PaginatedItemsViewModel<Catalog>(pageSize, pageIndex, totalCount, itemsOnPage);

            return Ok(model);
        }

        // GET api/catalog/items/withname/Wonder?pageSize=4&pageIndex=0
        [HttpGet]
        [Route("items/withname/{name:minlength(1)}")]
        public async Task<IActionResult> GetItems(string name, int pageSize = 6, int pageIndex = 0)
        {
            var totalCount = await _catalogContext.Catalogs
                                                  .Where(c => c.Name.StartsWith(name))
                                                  .LongCountAsync();

            var itemsOnPage = await _catalogContext.Catalogs
                                                   .Where(c => c.Name.StartsWith(name))
                                                   .OrderBy(o => o.Name)
                                                   .Skip(pageSize * pageIndex)
                                                   .Take(pageSize)
                                                   .ToListAsync();

            itemsOnPage = ChangeUrlPlaceHolder(itemsOnPage);
            var model = new PaginatedItemsViewModel<Catalog>(pageSize, pageIndex, totalCount, itemsOnPage);

            return Ok(model);
        }

        // GET api/catalog/items/type/1/band/%20?pageSize=4&pageIndex=0
        [HttpGet]
        [Route("items/type/{catalogTypeId}/brand/{catalogBrandId}")]
        public async Task<IActionResult> GetItems(int? catalogTypeId, int? catalogBrandId, [FromQuery] int pageSize = 6, [FromQuery] int pageIndex = 0)
        {
            //we are not going to database yet, until hit the first where
            //https://stackoverflow.com/questions/1578778/using-iqueryable-with-linq
            var catalogs = (IQueryable<Catalog>)_catalogContext.Catalogs;

            if (catalogTypeId.HasValue)
            {
                catalogs = catalogs.Where(r => r.CatalogTypeId == catalogTypeId);
            }

            if (catalogBrandId.HasValue)
            {
                catalogs = catalogs.Where(r => r.CatalogBrandId == catalogBrandId);
            }

            var totalCount = await catalogs.LongCountAsync();

            var itemsOnPage = await catalogs.OrderBy(o => o.Name)
                                        .Skip(pageSize * pageIndex)
                                        .Take(pageSize)
                                        .ToListAsync();

            itemsOnPage = ChangeUrlPlaceHolder(itemsOnPage);
            var model = new PaginatedItemsViewModel<Catalog>(pageSize, pageIndex, totalCount, itemsOnPage);

            return Ok(model);
        }

        private List<Catalog> ChangeUrlPlaceHolder(List<Catalog> items)
        {
            items.ForEach(x => x.PictureUrl.Replace("http://externalcatalogbaseurltobereplaced", _settings.Value.ExternalCatalogBaseUrl));
            return items;
        }
    }
}