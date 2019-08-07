using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Domain.Models
{
    public class CatalogType
    {
        public int id { get; set; }
        public string Type { get; set; }

        public ICollection<CatalogItem> Catalogs { get; private set; }
    }
}
