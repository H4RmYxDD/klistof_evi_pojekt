﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    [PrimaryKey(nameof(ProductId), nameof(CategoryId))]
    public class ProductCategory
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }

    }
}
