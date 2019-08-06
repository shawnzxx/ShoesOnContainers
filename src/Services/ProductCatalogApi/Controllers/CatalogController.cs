using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductCatalogApi.Data;
using ProductCatalogApi.Domain;
using ProductCatalogApi.ViewModels;

namespace ProductCatalogApi.Controllers
{
    [Produces("application/json")] //Produce define Accept header type
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _catalogContext;
        private readonly IOptionsSnapshot<CatalogSettings> _settings;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(CatalogContext catalogContext, IOptionsSnapshot<CatalogSettings> settings, ILogger<CatalogController> logger)
        {
            _catalogContext = catalogContext;
            _settings = settings;
            _logger = logger;
            //https://github.com/aspnet/EntityFrameworkCore/issues/7064
            ((DbContext)catalogContext).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
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
        [Route("items/{id}", Name = "GetItemById")]
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


        [HttpPost]
        [Route("items")]
        public async Task<IActionResult> CreateCatalog([FromBody] Catalog catalog)
        {
            try
            {
                var item = new Catalog
                {
                    CatalogBrandId = catalog.CatalogBrandId,
                    CatalogTypeId = catalog.CatalogTypeId,
                    Name = catalog.Name,
                    Description = catalog.Description,
                    PictureFileName = catalog.PictureFileName,
                    Price = catalog.Price,
                    PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/15"
                };

                _catalogContext.Catalogs.Add(item);
                await _catalogContext.SaveChangesAsync();

                return CreatedAtRoute("GetItemById", new { id = item.Id }, item);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            }
            
        }

        [HttpPut]
        [Route("items")]
        public async Task<IActionResult> UpdateCatalog([FromBody] Catalog catalogToUpdate)
        {
            try
            {
                var catalogItem = await _catalogContext.Catalogs.SingleOrDefaultAsync(i => i.Id == catalogToUpdate.Id);
                if (catalogItem == null)
                {
                    return NotFound(new { Message = $"Catalog item with id {catalogToUpdate.Id} can not be found." });
                }

                catalogItem = catalogToUpdate;
                _catalogContext.Catalogs.Update(catalogItem);
                await _catalogContext.SaveChangesAsync();

                return CreatedAtRoute("GetItemById", new { id = catalogToUpdate.Id }, catalogToUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            }

        }

        [HttpDelete]
        [Route("items/{id}")]
        public async Task<IActionResult> DeleteCatalog(int id) {

            var catalogItem = await _catalogContext.Catalogs.SingleOrDefaultAsync(i => i.Id == id);
            if (catalogItem == null)
            {
                return NotFound();
            }
            _catalogContext.Remove(catalogItem);
            await _catalogContext.SaveChangesAsync();
            return NoContent();
        }

        private List<Catalog> ChangeUrlPlaceHolder(List<Catalog> items)
        {
            items.ForEach(x => x.PictureUrl.Replace("http://externalcatalogbaseurltobereplaced", _settings.Value.ExternalCatalogBaseUrl));
            return items;
        }
    }
}