﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Domain.Models
{
    public class CatalogItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PictureFileName { get; set; }
        public string PictureUrl { get; set; }

        public int CatalogTypeId { get; set; }
        public int CatalogBrandId { get; set; }

        //navigation properties
        public CatalogBrand CatalogBrand { get; set; }
        public CatalogType CatalogType { get; set; }
    }
}
