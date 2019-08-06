using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Domain.Models
{
    public class CatalogBrand
    {
        public int id { get; set; }
        public string Brand { get; set; }

        public ICollection<CatalogItem> Catalogs { get; private set; }
    }
}
