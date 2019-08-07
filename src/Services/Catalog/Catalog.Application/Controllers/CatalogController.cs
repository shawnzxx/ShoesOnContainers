using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Application.ViewModels;
using Catalog.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Catalog.Application.Controllers
{
    [Produces("application/json")] //Produce define Accept header type
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogRepository _catalogRepository;
        private readonly IOptionsSnapshot<CatalogSettings> _settings;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(ICatalogRepository catalogRepository, IOptionsSnapshot<CatalogSettings> settings, ILogger<CatalogController> logger)
        {
            _catalogRepository = catalogRepository;
            _settings = settings;
            _logger = logger;
        }

        // GET api/catalog/types
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Types()
        {
            var types = await _catalogRepository.GetTypes();
            return Ok(types);
        }

        // GET api/catalog/brands
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Brands()
        {
            var brands = await _catalogRepository.GetBrands();
            return Ok(brands);
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
            var item = await _catalogRepository.GetItemById(id);
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
            var totalCount = await _catalogRepository.GetItemsTotalCount();
            var itemsOnPage = await _catalogRepository.GetItems(pageSize, pageIndex);

            itemsOnPage = ChangeUrlPlaceHolder(itemsOnPage);
            var model = new PaginatedItemsViewModel<CatalogItem>(pageSize, pageIndex, totalCount, itemsOnPage);

            return Ok(model);
        }

        // GET api/catalog/items/withname/Wonder?pageSize=4&pageIndex=0
        [HttpGet]
        [Route("items/withname/{name:minlength(1)}")]
        public async Task<IActionResult> GetItems(string name, int pageSize = 6, int pageIndex = 0)
        {
            var totalCount = await _catalogRepository.GetItemsTotalCount();
            var itemsOnPage = await _catalogRepository.GetItems(name, pageSize, pageIndex);

            itemsOnPage = ChangeUrlPlaceHolder(itemsOnPage);
            var model = new PaginatedItemsViewModel<CatalogItem>(pageSize, pageIndex, totalCount, itemsOnPage);

            return Ok(model);
        }

        // GET api/catalog/items/type/1/band/%20?pageSize=4&pageIndex=0
        [HttpGet]
        [Route("items/type/{catalogTypeId?}/brand/{catalogBrandId?}")]
        public async Task<IActionResult> GetItems(int? catalogTypeId = null, int? catalogBrandId = null, [FromQuery] int pageSize = 6, [FromQuery] int pageIndex = 0)
        {
            var totalCount = await _catalogRepository.GetItemsTotalCount();

            var itemsOnPage = await _catalogRepository.GetItems(catalogTypeId, catalogBrandId, pageSize, pageIndex);

            itemsOnPage = ChangeUrlPlaceHolder(itemsOnPage);
            var model = new PaginatedItemsViewModel<CatalogItem>(pageSize, pageIndex, totalCount, itemsOnPage);

            return Ok(model);
        }


        [HttpPost]
        [Route("items")]
        public async Task<IActionResult> CreateCatalog([FromBody] CatalogItem catalog)
        {
            try
            {
                var item = _catalogRepository.CreateNewItem(catalog);
                await _catalogRepository.SaveChangesAsync();
                
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
        public async Task<IActionResult> UpdateCatalog([FromBody] CatalogItem catalogToUpdate)
        {
            try
            {
                var item = await _catalogRepository.GetItemById(catalogToUpdate.Id);
                if (item == null)
                {
                    return NotFound(new { Message = $"Catalog item with id {catalogToUpdate.Id} can not be found." });
                }

                item.Name = catalogToUpdate.Name;
                item.Price = catalogToUpdate.Price;
                item.PictureUrl = catalogToUpdate.PictureUrl;
                item.PictureFileName = catalogToUpdate.PictureFileName;
                item.Description = catalogToUpdate.Description;
                item.CatalogTypeId = catalogToUpdate.CatalogTypeId;
                item.CatalogBrandId = catalogToUpdate.CatalogBrandId;

                await _catalogRepository.SaveChangesAsync();

                return CreatedAtRoute("GetItemById", new { id = item.Id }, item);
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

            var item = await _catalogRepository.GetItemById(id);
            if (item == null)
            {
                return NotFound();
            }
            _catalogRepository.DeleteItem(item);
            await _catalogRepository.SaveChangesAsync();
            return NoContent();
        }

        private List<CatalogItem> ChangeUrlPlaceHolder(List<CatalogItem> items)
        {
            items.ForEach(x => x.PictureUrl.Replace("http://externalcatalogbaseurltobereplaced", _settings.Value.ExternalCatalogBaseUrl));
            return items;
        }
    }
}